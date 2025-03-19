using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.MessageStore;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel.Download;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PodcastUtilities.AndroidLogic.Services.Download
{
    public class DownloaderEvents
    {
        public EventHandler<Tuple<ISyncItem, int>> UpdateItemProgressEvent;
        public EventHandler<Tuple<ISyncItem, Status, string>> UpdateItemStatusEvent;
        public EventHandler<string> DisplayMessageEvent;
        public EventHandler CompleteEvent;
        public EventHandler ExceptionEvent;
    }

    public interface IDownloader
	{
        string NotifcationTitle { get; }
        string NotifcationText { get; }
        string NotifcationAccessibilityText { get; }

        bool IsDownloading { get; }
        void CancelAll();
        void SetDownloadingItems(List<DownloadRecyclerItem> allItems);
        void DownloadAllItems();
        List<DownloadRecyclerItem> GetDownloadItems();
        DownloaderEvents GetDownloaderEvents();
    }

    public class Downloader : IDownloader
    {
        private ILogger Logger;
        private ITaskPool TaskPool;
        private IApplicationControlFileProvider ApplicationControlFileProvider;
        private INetworkHelper NetworkHelper;
        private ISyncItemToEpisodeDownloaderTaskConverter Converter;
        private ICrashReporter CrashReporter;
        private IAnalyticsEngine AnalyticsEngine;
        private IStatusAndProgressMessageStore MessageStore;
        private IByteConverter ByteConverter;
        private IFileSystemHelper FileSystemHelper;
        private IResourceProvider ResourceProvider;
        private IMessageStoreInserter MessageStoreInserter;

        // this is just in case we try and start a download before we have completed the last one
        private bool DownloadingInProgress = false;
        // these are the recycler items to download its maintained by the UI - in response to events
        private List<DownloadRecyclerItem> AllItems = null;
        // these are our published events
        private DownloaderEvents Events = new DownloaderEvents();

        // do not make this anything other than private
        private object SyncLock = new object();
        // do not make this anything other than private
        private object MessageSyncLock = new object();

        public Downloader(
            ILogger logger,
            ITaskPool taskPool,
            IApplicationControlFileProvider applicationControlFileProvider,
            INetworkHelper networkHelper,
            ISyncItemToEpisodeDownloaderTaskConverter converter,
            ICrashReporter crashReporter,
            IAnalyticsEngine analyticsEngine,
            IStatusAndProgressMessageStore messageStore,
            IByteConverter byteConverter,
            IFileSystemHelper fileSystemHelper,
            IResourceProvider resourceProvider,
            IMessageStoreInserter messageStoreInserter)
        {
            Logger = logger;
            TaskPool = taskPool;
            ApplicationControlFileProvider = applicationControlFileProvider;
            NetworkHelper = networkHelper;
            Converter = converter;
            CrashReporter = crashReporter;
            AnalyticsEngine = analyticsEngine;
            MessageStore = messageStore;
            ByteConverter = byteConverter;
            FileSystemHelper = fileSystemHelper;
            ResourceProvider = resourceProvider;
            MessageStoreInserter = messageStoreInserter;
        }
        public DownloaderEvents GetDownloaderEvents()
        {
            return Events;
        }

        public bool IsDownloading
        {
            get
            {
                lock (SyncLock)
                {
                    return DownloadingInProgress;
                }
            }
        }

        public string NotifcationTitle
        {
            get
            {
                if (CountItemsToDownload() < 1)
                {
                    return ResourceProvider.GetString(Resource.String.download_notification_title_complete);
                }
                return ResourceProvider.GetString(Resource.String.download_notification_title_in_progress);
            }
        }

        public string NotifcationText
        {
            get
            {
                var fmt = ResourceProvider.GetString(Resource.String.download_notification_status_fmt);
                (int toDownload, int complete, int errored) = GetItemCounts();
                return string.Format(fmt, toDownload, complete, errored);
            }
        }

        public string NotifcationAccessibilityText
        {
            get
            {
                return NotifcationText;
            }
        }

        public void CancelAll()
        {
            Logger.Debug(() => $"Downloader:CancelAll");
            TaskPool.CancelAllTasks();
            lock (SyncLock)
            {
                DownloadingInProgress = false;
                AllItems = null;
            }
        }

        public List<DownloadRecyclerItem> GetDownloadItems()
        {
            lock (SyncLock)
            {
                return AllItems;
            }
        }

        private int CountItemsToDownload()
        {
            if (AllItems == null || AllItems.Count < 1)
            {
                return 0;
            }
            // only count items that have been seleected and that have not errored
            return AllItems.Where(recyclerItem => recyclerItem.Selected && recyclerItem.DownloadStatus != Status.Error).Count();
        }

        private Tuple<int,int,int> GetItemCounts()
        {
            if (AllItems == null || AllItems.Count < 1)
            {
                return Tuple.Create(0,0,0);
            }
            int toDownload = 0;
            int complete = 0;
            int errored = 0;
            AllItems.ToList().ForEach(item =>
            {
                if (item.DownloadStatus == Status.Error)
                {
                    errored++;
                }
                else if (item.ProgressPercentage == 100 || item.DownloadStatus == Status.Complete)
                {
                    complete++;
                }
                else if (item.Selected)
                {
                    toDownload++;
                }
            });
            return Tuple.Create(toDownload, complete, errored);
        }

        public void SetDownloadingItems(List<DownloadRecyclerItem> allItems)
        {
            Logger.Debug(() => $"Downloader:StartDownloadingItems = {allItems?.Count}");
            lock (SyncLock)
            {
                if (DownloadingInProgress)
                {
                    Logger.Warning(() => $"Downloader:SetDownloadingItems - downloading in progress - ignoring");
                    return;
                }
                AllItems = allItems;
                DownloadingInProgress = false;
            }
        }

        // this must not be run on the UI thread - it blocks
        public void DownloadAllItems()
        {
            lock (SyncLock)
            {
                if (AllItems == null)
                {
                    Logger.Warning(() => $"Downloader:StartDownloadingItems - all items not set - ignoring");
                    return;
                }
                if (DownloadingInProgress)
                {
                    Logger.Warning(() => $"Downloader:StartDownloadingItems - already downloading - ignoring");
                    return;
                }
                DownloadingInProgress = true;
            }

            try
            {
                var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
                int numberOfConnections = controlFile.GetMaximumNumberOfConcurrentDownloads();
                NetworkHelper.SetNetworkConnectionLimit(numberOfConnections);
                NetworkHelper.SetApplicationDefaultCertificateValidator();      // ignore SSL errors

                // the TaskPool needs a list of ISyncItems
                List<ISyncItem> AllEpisodesToDownload = new List<ISyncItem>(AllItems.Count);
                AllItems.Where(recyclerItem => recyclerItem.Selected).ToList().ForEach(item => AllEpisodesToDownload.Add(item.SyncItem));

                IEpisodeDownloader[] downloadTasks = Converter.ConvertItemsToTasks(AllEpisodesToDownload, DownloadStatusUpdate, DownloadProgressUpdate);
                Logger.Debug(() => $"Downloader: Number of tasks = {downloadTasks.Length}");
                foreach (var task in downloadTasks)
                {
                    Logger.Debug(() => $"Downloader:Download to: {task.SyncItem.DestinationPath}");
                }

                // run them in a task pool
                TaskPool.RunAllTasks(numberOfConnections, downloadTasks);
                Logger.Debug(() => $"Downloader:Download tasks complete");
            }
            catch (Exception ex)
            {
                Logger.LogException(() => $"Downloader:DownloadAllItems", ex);
                CrashReporter.LogNonFatalException(ex);

                Events.DisplayMessageEvent?.Invoke(this, ex.Message);
            }
            finally
            {
                Logger.Debug(() => $"Downloader:finally");
                DownloadingInProgress = false;

                Events.CompleteEvent?.Invoke(this, null);
            }
        }

        private void DownloadProgressUpdateOld(object sender, ProgressEventArgs e)
        {
            lock (MessageSyncLock)
            {
                ISyncItem syncItem = e.UserState as ISyncItem;
                if (e.ProgressPercentage % 10 == 0)
                {
                    // only do every 10%
                    var line = string.Format("{0} ({1} of {2}) {3}%", syncItem.EpisodeTitle,
                                                    DisplayFormatter.RenderFileSize(e.ItemsProcessed),
                                                    DisplayFormatter.RenderFileSize(e.TotalItemsToProcess),
                                                    e.ProgressPercentage);
                    Logger.Debug(() => line);
                    MessageStore.StoreMessage(syncItem.Id, line);
                    if (e.ProgressPercentage == 100)
                    {
                        AnalyticsEngine.DownloadEpisodeEvent(ByteConverter.BytesToMegabytes(e.TotalItemsToProcess));
                    }
                    Events.UpdateItemProgressEvent?.Invoke(this, Tuple.Create(syncItem, e.ProgressPercentage));
                }
                var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
                if (IsDestinationDriveFull(controlFile.GetSourceRoot(), controlFile.GetFreeSpaceToLeaveOnDownload()))
                {
                    TaskPool?.CancelAllTasks();
                }
            }
        }

        private void DownloadProgressUpdate(object sender, ProgressEventArgs e)
        {
            var update = MessageStoreInserter.InsertProgress(e);
            if (update != null)
            {
                Events.UpdateItemProgressEvent?.Invoke(this, update);
            }
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            if (IsDestinationDriveFull(controlFile.GetSourceRoot(), controlFile.GetFreeSpaceToLeaveOnDownload()))
            {
                TaskPool?.CancelAllTasks();
            }
        }

        private bool IsDestinationDriveFull(string root, long freeSpaceToLeaveInMb)
        {
            var freeMb = ByteConverter.BytesToMegabytes(FileSystemHelper.GetAvailableFileSystemSizeInBytes(root));
            if (freeMb < freeSpaceToLeaveInMb)
            {
                var message = string.Format("Destination drive is full leaving {0:#,0.##} MB free", freeMb);
                Events.DisplayMessageEvent?.Invoke(this, message);
                Logger.Debug(() => message);
                return true;
            }
            return false;
        }

        private void DownloadStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            var update = MessageStoreInserter.InsertStatus(e);
            if (update != null)
            {
                // we need to signal the UI
                Events.UpdateItemStatusEvent?.Invoke(this, update);
            }
            if (e.Exception != null)
            {
                Events.ExceptionEvent?.Invoke(this, null);
            }
        }

        private void DownloadStatusUpdateOld(object sender, StatusUpdateEventArgs e)
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            bool verbose = controlFile?.GetDiagnosticOutput() == DiagnosticOutputLevel.Verbose;
            ISyncItem item = null;
            string id = "NONE";
            if (e.UserState != null && e.UserState is ISyncItem)
            {
                item = e.UserState as ISyncItem;
                id = item.Id.ToString();
            }

            if (e.MessageLevel == StatusUpdateLevel.Verbose && !verbose)
            {
                // log the status update to the logger but not to the UI
                Logger.Verbose(() => $"Downloader:StatusUpdate ID {id}, {e.Message}, Complete {e.IsTaskCompletedSuccessfully}");
                return;
            }

            // keep all the message together
            lock (MessageSyncLock)
            {
                if (e.Exception != null)
                {
                    Logger.LogException(() => $"Downloader:StatusUpdate ID {id}, {e.Message} -> ", e.Exception);
                    CrashReporter.LogNonFatalException(e.Message, e.Exception);
                    if (item != null)
                    {
                        MessageStore.StoreMessage(item.Id, e.Message);
                        MessageStore.StoreMessage(item.Id, e.Exception.ToString());
                        Events.UpdateItemStatusEvent?.Invoke(this, Tuple.Create(item, Status.Error, e.Message));
                    }
                    else
                    {
                        // its just a message - its not attached to a ISyncItem
                        MessageStore.StoreMessage(Guid.Empty, e.Message);
                        MessageStore.StoreMessage(Guid.Empty, e.Exception.ToString());
                    }
                    Events.ExceptionEvent?.Invoke(this, null);
                }
                else
                {
                    Logger.Debug(() => $"Downloader:StatusUpdate ID {id}, {e.Message}, Complete {e.IsTaskCompletedSuccessfully}");
                    Status status = (e.IsTaskCompletedSuccessfully ? Status.Complete : Status.Information);
                    if (status == Status.Complete)
                    {
                        AnalyticsEngine.DownloadEpisodeCompleteEvent();
                    }
                    if (item != null)
                    {
                        // we are updating the UI as we have a ISyncItem
                        MessageStore.StoreMessage(item.Id, e.Message);
                        Events.UpdateItemStatusEvent?.Invoke(this, Tuple.Create(item, status, e.Message));
                    }
                    else
                    {
                        // its just a message - its not attached to a ISyncItem
                        MessageStore.StoreMessage(Guid.Empty, e.Message);
                    }
                }
            }
        }

    }
}
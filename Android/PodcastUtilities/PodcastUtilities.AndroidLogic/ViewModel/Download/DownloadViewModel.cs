﻿using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PodcastUtilities.AndroidLogic.ViewModel.Download
{
    public class DownloadViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> Title;
            public EventHandler<int> StartProgress;
            public EventHandler<int> UpdateProgress;
            public EventHandler EndProgress;
            public EventHandler<List<DownloadRecyclerItem>> SetSyncItems;
            public EventHandler<Tuple<ISyncItem, int>> UpdateItemProgress;
            public EventHandler<Tuple<ISyncItem, Status, string>> UpdateItemStatus;
            public EventHandler<string> DisplayMessage;
            public EventHandler StartDownloading;
            public EventHandler<string> EndDownloading;
            public EventHandler Exit;
            public EventHandler NavigateToDisplayLogs;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IApplicationControlFileProvider ApplicationControlFileProvider;
        private IFileSystemHelper FileSystemHelper;
        private IByteConverter ByteConverter;
        private IEpisodeFinder PodcastEpisodeFinder;
        private ISyncItemToEpisodeDownloaderTaskConverter Converter;
        private ITaskPool TaskPool;
        private ICrashReporter CrashReporter;
        private IAnalyticsEngine AnalyticsEngine;
        private IStatusAndProgressMessageStore MessageStore;

        private List<DownloadRecyclerItem> AllItems = new List<DownloadRecyclerItem>(20);
        private bool StartedFindingPodcasts = false;
        private bool CompletedFindingPodcasts = false;
        private bool DownloadingInProgress = false;
        private bool ExitRequested = false;
        private int FeedCount = 0;

        // do not make this anything other than private
        private object SyncLock = new object();
        // do not make this anything other than private
        private object MessageSyncLock = new object();

        public DownloadViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            IFileSystemHelper fileSystemHelper,
            IByteConverter byteConverter,
            IEpisodeFinder podcastEpisodeFinder,
            ISyncItemToEpisodeDownloaderTaskConverter converter,
            ITaskPool taskPool,
            ICrashReporter crashReporter,
            IAnalyticsEngine analyticsEngine, 
            IStatusAndProgressMessageStore messageStore) : base(app)
        {
            ApplicationContext = app;
            Logger = logger;
            ResourceProvider = resProvider;
            ApplicationControlFileProvider = appControlFileProvider;
            FileSystemHelper = fileSystemHelper;
            ByteConverter = byteConverter;
            PodcastEpisodeFinder = podcastEpisodeFinder;
            Converter = converter;
            TaskPool = taskPool;
            CrashReporter = crashReporter;
            AnalyticsEngine = analyticsEngine;
            MessageStore = messageStore;
        }

        public void Initialise()
        {
            Logger.Debug(() => $"DownloadViewModel:Initialise");
            Observables.Title?.Invoke(this, ResourceProvider.GetString(Resource.String.download_activity_title));
        }

        public bool RequestExit()
        {
            Logger.Debug(() => $"DownloadViewModel:RequestExit");
            if (DownloadingInProgress)
            {
                Logger.Debug(() => $"DownloadViewModel:RequestExit - download in progress");
                TaskPool.CancelAllTasks();
                ExitRequested = true;
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.download_activity_cancelling));
                return false;
            }
            return true;
        }

        public bool ActionSelected(int itemId)
        {
            Logger.Debug(() => $"DownloadViewModel:ActionSelected = {itemId}");
            if (itemId == Resource.Id.action_display_logs)
            {
                Observables.NavigateToDisplayLogs?.Invoke(this, null);
                return true;
            }
            return false;
        }

        public bool IsActionAvailable(int itemId)
        {
            Logger.Debug(() => $"DownloadViewModel:isActionAvailable = {itemId}");
            if (itemId == Resource.Id.action_display_logs)
            {
                return true;
            }
            return false;
        }

        public void FindEpisodesToDownload()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            Logger.Debug(() => $"DownloadViewModel:FindEpisodesToDownload");
            if (controlFile == null)
            {
                Logger.Warning(() => $"DownloadViewModel:FindEpisodesToDownload - no control file");
                return;
            }

            lock (SyncLock)
            {
                if (StartedFindingPodcasts)
                {
                    Logger.Warning(() => $"DownloadViewModel:FindEpisodesToDownload - ignoring, already initialised");
                    if (CompletedFindingPodcasts)
                    {
                        Observables.SetSyncItems?.Invoke(this, AllItems);
                        SetTitle();
                    }
                    else
                    {
                        Observables.StartProgress?.Invoke(this, FeedCount);
                    }
                    return;
                }
                StartedFindingPodcasts = true;
                MessageStore.Reset();
            }

            foreach (var item in controlFile.GetPodcasts())
            {
                FeedCount++;
            }

            Observables.StartProgress?.Invoke(this, FeedCount);

            // find the episodes to download
            AllItems.Clear();
            int count = 0;
            foreach (var podcastInfo in controlFile.GetPodcasts())
            {
                var episodesInThisFeed = PodcastEpisodeFinder.FindEpisodesToDownload(
                    controlFile.GetSourceRoot(),
                    controlFile.GetRetryWaitInSeconds(),
                    podcastInfo,
                    controlFile.GetDiagnosticRetainTemporaryFiles());
                foreach (var episode in episodesInThisFeed)
                {
                    var line = $"{episode.Id}, {episode.EpisodeTitle}";
                    Logger.Debug(() => $"DownloadViewModel:FindEpisodesToDownload {line}");
                    MessageStore.StoreMessage(episode.Id, line);
                    var item = new DownloadRecyclerItem()
                    {
                        SyncItem = episode,
                        ProgressPercentage = 0,
                        Podcast = podcastInfo,
                        Selected = true
                    };
                    AllItems.Add(item);
                }
                count++;
                AnalyticsEngine.DownloadFeedEvent(episodesInThisFeed.Count);
                Observables.UpdateProgress?.Invoke(this, count);
            }
            CompletedFindingPodcasts = true;
            Observables.EndProgress?.Invoke(this, null);
            Observables.SetSyncItems?.Invoke(this, AllItems);
            SetTitle();
        }

        private int GetItemsSelectedCount()
        {
            if (AllItems.Count < 1)
            {
                return 0;
            }
            return AllItems.Where(recyclerItem => recyclerItem.Selected).Count();
        }

        private void SetTitle()
        {
            var title = ResourceProvider.GetQuantityString(Resource.Plurals.download_activity_title_after_load, GetItemsSelectedCount());
            Logger.Debug(() => $"DownloadViewModel:SetTitle - {title}");
            Observables.Title?.Invoke(this, title);
        }

        internal void SelectionChanged(int position)
        {
            SetTitle();
        }

        public void DownloadAllPodcasts()
        {
            Logger.Debug(() => $"DownloadViewModel:DownloadAllPodcasts");

            if (AllItems.Count < 1)
            {
                Observables.DisplayMessage?.Invoke(this, "Nothing to download");
                return;
            }

            lock (SyncLock)
            {
                if (DownloadingInProgress)
                {
                    Logger.Debug(() => $"DownloadViewModel:DownloadAllPodcasts - already in progress - ignored");
                    return;
                }
                DownloadingInProgress = true;
            }

            Observables.StartDownloading?.Invoke(this, null);

            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            int numberOfConnections = controlFile.GetMaximumNumberOfConcurrentDownloads();
            System.Net.ServicePointManager.DefaultConnectionLimit = numberOfConnections;

            List<ISyncItem> AllEpisodesToDownload = new List<ISyncItem>(AllItems.Count);
            AllItems.Where(recyclerItem => recyclerItem.Selected).ToList().ForEach(item => AllEpisodesToDownload.Add(item.SyncItem));

            IEpisodeDownloader[] downloadTasks = Converter.ConvertItemsToTasks(AllEpisodesToDownload, DownloadStatusUpdate, DownloadProgressUpdate);
            foreach (var task in downloadTasks)
            {
                Logger.Debug(() => $"DownloadViewModel:Download to: {task.SyncItem.DestinationPath}");
            }

            // run them in a task pool
            TaskPool.RunAllTasks(numberOfConnections, downloadTasks);
            Logger.Debug(() => $"DownloadViewModel:Download tasks complete");
            DownloadingInProgress = false;
        }

        public void DownloadComplete()
        {
            Observables.EndDownloading?.Invoke(this, ResourceProvider.GetString(Resource.String.download_activity_complete));
            if (ExitRequested)
            {
                Observables.Exit?.Invoke(this, null);
            }
        }

        void DownloadProgressUpdate(object sender, ProgressEventArgs e)
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
                    Observables.UpdateItemProgress?.Invoke(this, Tuple.Create(syncItem, e.ProgressPercentage));
                }
                var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
                if (IsDestinationDriveFull(controlFile.GetSourceRoot(), controlFile.GetFreeSpaceToLeaveOnDownload()))
                {
                    TaskPool?.CancelAllTasks();
                }
            }
        }

        private bool IsDestinationDriveFull(string root, long freeSpaceToLeaveInMb)
        {
            var freeMb = ByteConverter.BytesToMegabytes(FileSystemHelper.GetAvailableFileSystemSizeInBytes(root));
            if (freeMb < freeSpaceToLeaveInMb)
            {
                var message = string.Format("Destination drive is full leaving {0:#,0.##} MB free", freeMb);
                Observables.DisplayMessage?.Invoke(this, message);
                Logger.Debug(() => message);
                return true;
            }
            return false;
        }

        void DownloadStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            bool _verbose = false;
            if (e.MessageLevel == StatusUpdateLevel.Verbose && !_verbose)
            {
                return;
            }

            lock (MessageSyncLock)
            {
                // keep all the message together
                ISyncItem item = null;
                string id = "NONE";
                if (e.UserState != null && e.UserState is ISyncItem)
                {
                    item = e.UserState as ISyncItem;
                    id = item.Id.ToString();
                }
                if (e.Exception != null)
                {
                    Logger.LogException(() => $"DownloadViewModel:StatusUpdate ID {id} -> ", e.Exception);
                    CrashReporter.LogNonFatalException(e.Exception);
                    if (item != null)
                    {
                        MessageStore.StoreMessage(item.Id, e.Message);
                        MessageStore.StoreMessage(item.Id, e.Exception.ToString());
                        Observables.UpdateItemStatus?.Invoke(this, Tuple.Create(item, Status.Error, e.Message));
                    }
                }
                else
                {
                    Logger.Debug(() => $"DownloadViewModel:StatusUpdate ID {id}, {e.Message}, Complete {e.IsTaskCompletedSuccessfully}");
                    Status status = (e.IsTaskCompletedSuccessfully ? Status.Complete : Status.Information);
                    if (status == Status.Complete)
                    {
                        AnalyticsEngine.DownloadEpisodeCompleteEvent();
                    }
                    if (item != null)
                    {
                        MessageStore.StoreMessage(item.Id, e.Message);
                        Observables.UpdateItemStatus?.Invoke(this, Tuple.Create(item, status, e.Message));
                    }
                }
            }
        }

    }
}
﻿using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Settings;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            public EventHandler<Tuple<string, string, string, string>> ExitPrompt;
            public EventHandler<Tuple<string, string, string, string>> CellularPrompt;
            public EventHandler<string> SetEmptyText;
            public EventHandler<bool> TestMode;
            public EventHandler<string> DisplayErrorMessage;
            public EventHandler HideErrorMessage;
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
        private INetworkHelper NetworkHelper;
        private IUserSettings UserSettings;

        private List<DownloadRecyclerItem> AllItems = new List<DownloadRecyclerItem>(20);
        private bool StartedFindingPodcasts = false;
        private bool CompletedFindingPodcasts = false;
        private bool DownloadingInProgress = false;
        private bool ExitRequested = false;
        private int FeedCount = 0;
        private bool TestMode = false;

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
            IStatusAndProgressMessageStore messageStore,
            INetworkHelper networkHelper, 
            IUserSettings userSettings) : base(app)
        {
            ApplicationContext = app;
            Logger = logger;
            Logger.Debug(() => $"DownloadViewModel:ctor");
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
            NetworkHelper = networkHelper;
            UserSettings = userSettings;
        }

        public void Initialise(bool test)
        {
            Logger.Debug(() => $"DownloadViewModel:Initialise - {test}");
            TestMode = test;
            if (TestMode)
            {
                Observables.Title?.Invoke(this, ResourceProvider.GetString(Resource.String.download_activity_test_title));
            }
            else
            {
                Observables.Title?.Invoke(this, ResourceProvider.GetString(Resource.String.download_activity_title));
            }
            Observables.TestMode?.Invoke(this, TestMode);

            NetworkHelper.SetApplicationDefaultCertificateValidator();      // ignore SSL errors

            Observables.HideErrorMessage?.Invoke(this, null);
            PodcastEpisodeFinder.StatusUpdate += this.DownloadStatusUpdate;
        }

        public bool RequestExit()
        {
            Logger.Debug(() => $"DownloadViewModel:RequestExit");
            if (DownloadingInProgress)
            {
                Logger.Debug(() => $"DownloadViewModel:RequestExit - download in progress");
                Observables.ExitPrompt?.Invoke(this, 
                    Tuple.Create(
                        ResourceProvider.GetString(Resource.String.dialog_title),
                        ResourceProvider.GetString(Resource.String.download_activity_exit_prompt),
                        ResourceProvider.GetString(Resource.String.download_activity_exit_ok),
                        ResourceProvider.GetString(Resource.String.download_activity_exit_cancel)
                    )
                );
                return false;
            }
            return true;
        }

        public void CancelAllJobsAndExit()
        {
            Logger.Debug(() => $"DownloadViewModel:CancelAllJobsAndExit");
            TaskPool.CancelAllTasks();
            ExitRequested = true;
            Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.download_activity_cancelling));
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

        public void FindEpisodesToDownload(string folderSelected)
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            Logger.Debug(() => $"DownloadViewModel:FindEpisodesToDownload, folder = {folderSelected}");
            if (controlFile == null)
            {
                Logger.Warning(() => $"DownloadViewModel:FindEpisodesToDownload - no control file");
                return;
            }

            var noItemsText = ResourceProvider.GetString(Resource.String.finding_podcasts_progress);
            Observables.SetEmptyText?.Invoke(this, noItemsText);
            var activeConnectionType = NetworkHelper.ActiveNetworkType;
            Logger.Debug(() => $"DownloadViewModel:FindEpisodesToDownload Active = {activeConnectionType}");
            if (activeConnectionType == INetworkHelper.NetworkType.None)
            {
                noItemsText = ResourceProvider.GetString(Resource.String.no_network_text);
                Observables.SetEmptyText?.Invoke(this, noItemsText);
                return;
            }

            lock (SyncLock)
            {
                if (StartedFindingPodcasts)
                {
                    Logger.Warning(() => $"DownloadViewModel:FindEpisodesToDownload - ignoring, already initialised");
                    if (CompletedFindingPodcasts)
                    {
                        RefreshUI(folderSelected);
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
                if (string.IsNullOrEmpty(folderSelected) || item.Folder == folderSelected)
                {
                    FeedCount++;
                }
            }

            Observables.StartProgress?.Invoke(this, FeedCount);

            // find the episodes to download
            AllItems.Clear();
            int count = 0;
            foreach (var podcastInfo in controlFile.GetPodcasts())
            {
                if (string.IsNullOrEmpty(folderSelected) || podcastInfo.Folder == folderSelected)
                {
                    SetFindingText(folderSelected, podcastInfo.Folder);
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
                            Selected = true,
                            AllowSelection = !TestMode
                        };
                        AllItems.Add(item);
                    }
                    count++;
                    if (string.IsNullOrEmpty(folderSelected))
                    {
                        AnalyticsEngine.DownloadFeedEvent(episodesInThisFeed.Count);
                    }
                    else
                    {
                        // a specific feed
                        if (TestMode)
                        {
                            AnalyticsEngine.TestSpecificFeedEvent(episodesInThisFeed.Count, folderSelected);
                        }
                        else
                        {
                            AnalyticsEngine.DownloadSpecificFeedEvent(episodesInThisFeed.Count, folderSelected);
                        }
                    }
                    Observables.UpdateProgress?.Invoke(this, count);
                }
            }
            CompletedFindingPodcasts = true;
            Observables.EndProgress?.Invoke(this, null);
            RefreshUI(folderSelected);
            PodcastEpisodeFinder.StatusUpdate -= this.DownloadStatusUpdate;
        }

        private void SetFindingText(string folderSelected, string thisFolder)
        {
            if (string.IsNullOrEmpty(folderSelected))
            {
                // we are looking in all feeds
                var titleLine1 = ResourceProvider.GetString(Resource.String.finding_podcasts_all_title1);
                var titleLine2 = ResourceProvider.GetQuantityString(Resource.Plurals.finding_podcasts_all_title2, GetItemsSelectedCount());
                var titleLine3 = ResourceProvider.GetString(Resource.String.finding_podcasts_all_title3);
                var emptyMessage = $"{titleLine1}\n{titleLine2}\n{titleLine3}\n{thisFolder}";
                Observables.SetEmptyText?.Invoke(this, emptyMessage);
            }
            else
            {
                // we are looking in a specific folder
                var titleLine1 = ResourceProvider.GetQuantityString(Resource.Plurals.finding_podcasts_title1, GetItemsSelectedCount());
                var titleLine2 = ResourceProvider.GetString(Resource.String.finding_podcasts_title2);
                var emptyMessage = $"{titleLine1}\n{titleLine2}\n{thisFolder}";
                Observables.SetEmptyText?.Invoke(this, emptyMessage);
            }
        }

        private void SetNoDownloads(string folderSelected)
        {
            var noDownloadsText = ResourceProvider.GetString(Resource.String.no_downloads_text);
            var emptyMessage = (string.IsNullOrEmpty(folderSelected)) ? noDownloadsText : $"{folderSelected}\n{noDownloadsText}";
            Observables.SetEmptyText?.Invoke(this, emptyMessage);
        }

        private void RefreshUI(string folderSelected)
        {
            if (AllItems.Count < 1)
            {
                SetNoDownloads(folderSelected);
            }
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
            string title = "";
            if (TestMode)
            {
                title = ResourceProvider.GetQuantityString(Resource.Plurals.download_activity_test_title_after_load, GetItemsSelectedCount());
            } else
            {
                title = ResourceProvider.GetQuantityString(Resource.Plurals.download_activity_title_after_load, GetItemsSelectedCount());
            }
            Logger.Debug(() => $"DownloadViewModel:SetTitle - {title}");
            Observables.Title?.Invoke(this, title);
        }

        internal void SelectionChanged(int position)
        {
            SetTitle();
        }

        private bool NetworkConnectionTypeAllowedPromptNeeded(INetworkHelper.NetworkType activeConnectionType, IUserSettings.DownloadNetworkType requiredConnectionType)
        {
            if (activeConnectionType == INetworkHelper.NetworkType.Cellular && requiredConnectionType == IUserSettings.DownloadNetworkType.WIFI)
            {
                Logger.Debug(() => $"DownloadViewModel:NetworkConnectionTypeAllowedPromptNeeded - on cellular");
                Observables.CellularPrompt?.Invoke(this,
                    Tuple.Create(
                        ResourceProvider.GetString(Resource.String.dialog_title),
                        ResourceProvider.GetString(Resource.String.download_activity_cellular_prompt),
                        ResourceProvider.GetString(Resource.String.download_activity_cellular_ok),
                        ResourceProvider.GetString(Resource.String.download_activity_cellular_cancel)
                    )
                );
                return true;
            }
            return false;
        }

        public Task DownloadAllPodcastsWithNetworkCheck()
        {
            Logger.Debug(() => $"DownloadViewModel:DownloadAllPodcastsWithNetworkCheck");

            var activeConnectionType = NetworkHelper.ActiveNetworkType;
            var requiredConnectionType = UserSettings.DownloadNetworkNeeded;
            Logger.Debug(() => $"DownloadViewModel:DownloadAllPodcastsWithNetworkCheck Active = {activeConnectionType}, Needed = {requiredConnectionType}");

            if (activeConnectionType == INetworkHelper.NetworkType.None)
            {
                // no internet
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.no_network_text));
                return null;
            }

            if (NetworkConnectionTypeAllowedPromptNeeded(activeConnectionType, requiredConnectionType))
            {
                // a prompt was needed
                return null;
            }

            return DownloadAllPodcastsWithoutNetworkCheck();
        }

        public Task DownloadAllPodcastsWithoutNetworkCheck()
        {
            return Task.Run(() =>
                {
                    DownloadAllPodcasts();
                }
            );
        }

        // dont run this on the UI thread
        private void DownloadAllPodcasts()
        {
            Logger.Debug(() => $"DownloadViewModel:DownloadAllPodcasts");

            if (AllItems.Count < 1)
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.no_downloads_text));
                return;
            }

            lock (SyncLock)
            {
                if (DownloadingInProgress)
                {
                    Logger.Warning(() => $"DownloadViewModel:DownloadAllPodcasts - already in progress - ignored");
                    return;
                }
                DownloadingInProgress = true;
            }

            try
            {
                Observables.StartDownloading?.Invoke(this, null);

                var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
                int numberOfConnections = controlFile.GetMaximumNumberOfConcurrentDownloads();
                NetworkHelper.SetNetworkConnectionLimit(numberOfConnections);
                NetworkHelper.SetApplicationDefaultCertificateValidator();      // ignore SSL errors

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
            }
            catch (Exception ex)
            {
                Logger.LogException(() => $"DownloadViewModel:DownloadAllPodcasts", ex);
                CrashReporter.LogNonFatalException(ex);
                Observables.DisplayMessage?.Invoke(this, ex.Message);
            }
            finally
            {
                Observables.EndDownloading?.Invoke(this, ResourceProvider.GetString(Resource.String.download_activity_complete));
                if (ExitRequested)
                {
                    Observables.Exit?.Invoke(this, null);
                }
                DownloadingInProgress = false;
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
                Logger.Verbose(() => $"DownloadViewModel:StatusUpdate ID {id}, {e.Message}, Complete {e.IsTaskCompletedSuccessfully}");
                return;
            }

            lock (MessageSyncLock)
            {
                // keep all the message together
                if (e.Exception != null)
                {
                    Logger.LogException(() => $"DownloadViewModel:StatusUpdate ID {id}, {e.Message} -> ", e.Exception);
                    CrashReporter.LogNonFatalException(e.Message, e.Exception);
                    if (item != null)
                    {
                        MessageStore.StoreMessage(item.Id, e.Message);
                        MessageStore.StoreMessage(item.Id, e.Exception.ToString());
                        Observables.UpdateItemStatus?.Invoke(this, Tuple.Create(item, Status.Error, e.Message));
                    }
                    else
                    {
                        // its just a message - its not attached to a ISyncItem
                        MessageStore.StoreMessage(Guid.Empty, e.Message);
                        MessageStore.StoreMessage(Guid.Empty, e.Exception.ToString());
                    }
                    Observables.DisplayErrorMessage?.Invoke(this, null);        // use the canned in message
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
                        // we are updating the UI as we have a ISyncItem
                        MessageStore.StoreMessage(item.Id, e.Message);
                        Observables.UpdateItemStatus?.Invoke(this, Tuple.Create(item, status, e.Message));
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
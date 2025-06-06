﻿using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.MessageStore;
using PodcastUtilities.AndroidLogic.Services.Download;
using PodcastUtilities.AndroidLogic.Settings;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PodcastUtilities.AndroidLogic.ViewModel.Download
{
    public class DownloadViewModel : AndroidViewModel, ILifecycleObserver, IDownloadServiceConnectionListener
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
            public EventHandler NavigateToDisplayLogs;
            public EventHandler<Tuple<string, string, string, string>> KillPrompt;
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
        private IEpisodeFinder PodcastEpisodeFinder;
        private IAnalyticsEngine AnalyticsEngine;
        private IStatusAndProgressMessageStore MessageStore;
        private INetworkHelper NetworkHelper;
        private IUserSettings UserSettings;
        private IDownloadServiceController DownloadServiceController;
        private IMessageStoreInserter MessageStoreInserter;
        private IPermissionChecker PermissionChecker;

        private List<DownloadRecyclerItem> AllItems = new List<DownloadRecyclerItem>(20);
        private bool StartedFindingPodcasts = false;
        private bool CompletedFindingPodcasts = false;
        private int FeedCount = 0;
        private bool TestMode = false;
        private bool FromHud = false;
        private string FolderSelected = null;

        private IDownloadService DownloadService = null;
        private DownloaderEvents DownloaderEvents = null;

        // do not make this anything other than private
        private object SyncLock = new object();

        public DownloadViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            IEpisodeFinder podcastEpisodeFinder,
            IAnalyticsEngine analyticsEngine,
            IStatusAndProgressMessageStore messageStore,
            INetworkHelper networkHelper,
            IUserSettings userSettings,
            IDownloadServiceController downloadServiceController,
            IMessageStoreInserter messageStoreInserter,
            IPermissionChecker permissionChecker) : base(app)
        {
            ApplicationContext = app;
            Logger = logger;
            Logger.Debug(() => $"DownloadViewModel:ctor");
            ResourceProvider = resProvider;
            ApplicationControlFileProvider = appControlFileProvider;
            PodcastEpisodeFinder = podcastEpisodeFinder;
            AnalyticsEngine = analyticsEngine;
            MessageStore = messageStore;
            NetworkHelper = networkHelper;
            UserSettings = userSettings;
            DownloadServiceController = downloadServiceController;
            MessageStoreInserter = messageStoreInserter;
            PermissionChecker = permissionChecker;
        }

        // only public for tests
        public void ConnectServiceForUnitTests(IDownloadService service)
        {
            Logger.Debug(() => $"DownloadViewModel:ConnectServiceForUnitTests isDownloading - {service?.IsDownloading}");
            DownloadService = service;
            DownloaderEvents = service?.GetDownloaderEvents();
            AttachToDownloaderEvents();
        }

        public void ConnectService(IDownloadService service)
        {
            ConnectServiceForUnitTests(service);

            DiscoverStartModeFromService();
        }

        // only public for tests
        public Task DiscoverStartModeFromService()
        {
            Logger.Debug(() => $"DownloadViewModel:DiscoverStartModeFromService");
            bool findEpisodesOnLoad = false;
            if (TestMode)
            {
                // in test mode - so lets test
                findEpisodesOnLoad = true;
            }
            if (!(DownloadService?.IsDownloading ?? false))
            {
                // no download in progress - so lets look for podcasts
                findEpisodesOnLoad = true;
            }
            if (FromHud)
            {
                // if the user tapped on the notification then dont go finding episodes
                findEpisodesOnLoad = false;
                // if there are no downloads then lets stop the service
                if (!(DownloadService?.IsDownloading ?? false))
                {
                    Logger.Debug(() => $"DownloadViewModel:DiscoverStartModeFromService - tidy up service");
                    DownloadServiceController.StopService();
                    DownloadService.KillNotification();
                }
            }

            if (findEpisodesOnLoad)
            {
                // clear the notification as we are now finding again
                DownloadServiceController.StopService();
                DownloadService.KillNotification();
                // we must not do this on the UI thread
                return FindEpisodesToDownloadInBackground(FolderSelected);
            }
            else
            {
                // there already was a download running
                // setup the UI as if the items had been found
                AllItems = DownloadService?.GetItems();
                RefreshUI(FolderSelected);
                Observables.StartDownloading?.Invoke(this, null);   // EndDownloading observable is triggered from the Downloader events
                if (FromHud && !(DownloadService?.IsDownloading ?? false))
                {
                    // we forced the display of the download - but there is nothing to do
                    Observables.EndDownloading?.Invoke(this, ResourceProvider.GetString(Resource.String.download_activity_complete));
                }
            }
            return null;
        }

        // public for testing
        public Task FindEpisodesToDownloadInBackground(string folder)
        {
            return Task.Run(() => FindEpisodesToDownload(folder));
        }

        private void AttachToDownloaderEvents()
        {
            if (DownloaderEvents == null)
            {
                return;
            }
            DownloaderEvents.DisplayMessageEvent += DownloaderServiceDisplayMessage;
            DownloaderEvents.UpdateItemProgressEvent += DownloaderServiceUpdateItemProgress;
            DownloaderEvents.UpdateItemStatusEvent += DownloaderServiceUpdateItemStatus;
            DownloaderEvents.CompleteEvent += DownloaderServiceComplete;
            DownloaderEvents.ExceptionEvent += DownloaderServiceException;
        }

        private void DetachFromDownloaderEvents()
        {
            if (DownloaderEvents == null)
            {
                return;
            }
            DownloaderEvents.DisplayMessageEvent -= DownloaderServiceDisplayMessage;
            DownloaderEvents.UpdateItemProgressEvent -= DownloaderServiceUpdateItemProgress;
            DownloaderEvents.UpdateItemStatusEvent -= DownloaderServiceUpdateItemStatus;
            DownloaderEvents.CompleteEvent -= DownloaderServiceComplete;
        }

        // mostly we just bubble the events from the service back to the UI
        private void DownloaderServiceDisplayMessage(object sender, string message)
        {
            Logger.Debug(() => $"DownloadViewModel:DownloaderServiceEvent:Message: {message}");
            Observables.DisplayMessage?.Invoke(this, message);
        }

        private void DownloaderServiceUpdateItemStatus(object sender, Tuple<ISyncItem, Status, string> status)
        {
            Logger.Debug(() => $"DownloadViewModel:DownloaderServiceEvent:Status: {status?.Item2}");
            Observables.UpdateItemStatus?.Invoke(this, status);
        }

        private void DownloaderServiceUpdateItemProgress(object sender, Tuple<ISyncItem, int> progress)
        {
            Logger.Debug(() => $"DownloadViewModel:DownloaderServiceEvent:Progres: {progress.Item2}");
            Observables.UpdateItemProgress?.Invoke(this, progress);
        }

        private void DownloaderServiceComplete(object sender, EventArgs e)
        {
            Logger.Debug(() => $"DownloadViewModel:DownloaderServiceEvent:Complete");
            Observables.EndDownloading?.Invoke(this, ResourceProvider.GetString(Resource.String.download_activity_complete));
        }

        private void DownloaderServiceException(object sender, EventArgs e)
        {
            Logger.Debug(() => $"DownloadViewModel:DownloaderServiceEvent:Exception");
            Observables.DisplayErrorMessage?.Invoke(this, null);        // use the canned in message
        }

        public void DisconnectService()
        {
            Logger.Debug(() => $"DownloadViewModel:DisconnectService");
        }

        public void Initialise(bool onNew, bool test, string folder, bool fromHud)
        {
            Logger.Debug(() => $"DownloadViewModel:Initialise - {onNew}, {test}, {fromHud}, {folder}");
            if (!onNew)
            {
                DownloadServiceController.BindToService(this);
            }
            TestMode = test;
            FolderSelected = folder;
            FromHud = fromHud;
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
            if (!onNew)
            {
                PodcastEpisodeFinder.StatusUpdate += this.DownloadStatusUpdate;
            }
            if (onNew)
            {
                DiscoverStartModeFromService();
            }
        }

        public void Finalise()
        {
            Logger.Debug(() => $"DownloadViewModel:Finalise");
            DetachFromDownloaderEvents();
            DownloadServiceController.UnbindFromService();
        }

        public void RequestKillDownloads()
        {
            Logger.Debug(() => $"DownloadViewModel:RequestKillDownloads");
            Observables.KillPrompt?.Invoke(this,
                Tuple.Create(
                    ResourceProvider.GetString(Resource.String.dialog_title),
                    ResourceProvider.GetString(Resource.String.download_activity_kill_prompt),
                    ResourceProvider.GetString(Resource.String.download_activity_kill_ok),
                    ResourceProvider.GetString(Resource.String.download_activity_kill_cancel)
                )
            );
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
                if (!StartedFindingPodcasts)
                {
                    return true;
                }
                if (CompletedFindingPodcasts)
                {
                    return true;
                }
                return false;
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
            AllItems = new List<DownloadRecyclerItem>(20);
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
                            AllowSelection = !TestMode,
                            DownloadStatus = Status.OK
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
            if (FromHud)
            {
                Observables.SetEmptyText?.Invoke(this, ResourceProvider.GetString(Resource.String.no_downloads_from_hud_text));
                return;
            }
            var noDownloadsText = ResourceProvider.GetString(Resource.String.no_downloads_text);
            var emptyMessage = (string.IsNullOrEmpty(folderSelected)) ? noDownloadsText : $"{folderSelected}\n{noDownloadsText}";
            Observables.SetEmptyText?.Invoke(this, emptyMessage);
        }

        private void RefreshUI(string folderSelected)
        {
            var itemCount = AllItems?.Count ?? 0;
            if (itemCount < 1)
            {
                SetNoDownloads(folderSelected);
            }
            Observables.SetSyncItems?.Invoke(this, AllItems);
            SetTitle();
        }

        private int GetItemsSelectedCount()
        {
            return AllItems?.Where(recyclerItem => recyclerItem.Selected).Count() ?? 0;
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

        private void DownloadAllPodcasts()
        {
            Logger.Debug(() => $"DownloadViewModel:DownloadAllPodcasts");

            if (AllItems?.Count < 1)
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.no_downloads_text));
                return;
            }

            if (!PermissionChecker.HasPostNotifcationPermission(ApplicationContext))
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.download_no_notification_permission));
                // but we will carry on
            }

            Observables.StartDownloading?.Invoke(this, null);   // EndDownloading observable is triggered from the Downloader events

            foreach (var item in AllItems)
            {
                if (item?.Selected ?? false)
                {
                    // clear any errors that may have happened before, but only for utems we will try to download again
                    Observables.UpdateItemStatus?.Invoke(this, Tuple.Create(item.SyncItem, Status.OK, ""));
                }
            }
            Observables.HideErrorMessage?.Invoke(this, null);

            DownloadServiceController.StartService();           // we need to stop the service from disappearing when we unbind
            DownloadService?.StartDownloads(AllItems);
        }

        public void CancelAllDownloads()
        {
            DownloadService.CancelDownloads();
            DownloadServiceController.StopService();
        }

        // the status update from the episode finder
        private void DownloadStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            var update = MessageStoreInserter.InsertStatus(e);
            if (update != null)
            {
                // we need to signal the UI
                Observables.UpdateItemStatus?.Invoke(this, update);
            }
            if (e.Exception != null)
            {
                Observables.DisplayErrorMessage?.Invoke(this, null);        // use the canned in message
            }
        }

    }
}
﻿using Android.App;
using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.MessageStore;
using PodcastUtilities.AndroidLogic.Services.Download;
using PodcastUtilities.AndroidLogic.Settings;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel.Download;
using PodcastUtilities.AndroidTests.Helpers;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Download
{
    public class DownloadViewModelBase
    {
        protected const string SOURCE_ROOT = "/sdcard/sourceroot";
        protected const int RETRY_TIME = 123;
        protected const int MAX_DOWNLOADS = 13;
        protected const bool DIAGS = false;
        protected const string PODCAST_FOLDER_1 = "folder1";
        protected const string PODCAST_FOLDER_2 = "folder2";
        protected const int FREE_DISK_SPACE_MB = 10;

        protected Guid EPISODE_1_ID = Guid.Parse("72288452-3CD8-4BB4-BBF5-854C290CB499");
        protected const string EPISODE_1_TITLE = PODCAST_FOLDER_1 + "-episode1";

        protected Guid EPISODE_2_ID = Guid.Parse("C7CE55F0-0897-49A3-A84B-F3E8AF2668F3");
        protected const string EPISODE_2_TITLE = PODCAST_FOLDER_1 + "-episode2";

        protected Guid EPISODE_3_ID = Guid.Parse("F714C6E1-B3D4-4DE3-B72F-6E358869427E");
        protected const string EPISODE_3_TITLE = PODCAST_FOLDER_2 + "-episode1";

        protected Guid EPISODE_4_ID = Guid.Parse("11438380-868D-4A77-B411-A9171CD9298F");
        protected const string EPISODE_4_TITLE = PODCAST_FOLDER_2 + "-episode2";

        protected Guid EPISODE_5_ID = Guid.Parse("AFB9DD0B-C208-4066-9DFE-21726AABE2AF");
        protected const string EPISODE_5_TITLE = PODCAST_FOLDER_2 + "-episode3";

        protected DownloadViewModel ViewModel;

        public class ObservedResultsGroup
        {
            public string LastSetTitle;
            public string LastSetEmptyText;
            public List<int> StartProgress;
            public int StartProgressCount;
            public List<int> UpdateProgress;
            public int UpdateProgressCount;
            public int EndProgressCount;
            public List<DownloadRecyclerItem> LastDownloadItems;
            public string LastDisplayMessage;
            public string LastDisplayErrorMessage;
            public int DisplayErrorMessageCount;
            public string LastCellularPromptTitle;
            public string LastCellularPromptBody;
            public string LastCellularPromptCancel;
            public string LastCellularPromptOk;
            public string LastEndDownloadingMessage;
            public int EndDownloadingCount;
            public int StartDownloadingCount;
            public ISyncItem LastUpdatePercentageItem;
            public int LastUpdatePercentage;
            public string LastUpdateStatusMessage;
            public Status LastUpdateStatus;
            public ISyncItem LastUpdateStatusItem;
            public string LastKillPromptTitle;
            public string LastKillPromptMessage;
            public string LastKillPromptOk;
            public string LastKillPromptCancel;
        }
        protected ObservedResultsGroup ObservedResults = new ObservedResultsGroup();


        // mocks
        protected Application MockApplication;
        protected ILogger MockLogger;
        protected IResourceProvider MockResourceProvider;
        protected IApplicationControlFileProvider MockApplicationControlFileProvider;
        protected IAnalyticsEngine MockAnalyticsEngine;
        protected IStatusAndProgressMessageStore MockStatusAndProgressMessageStore;
        protected IReadWriteControlFile MockControlFile;
        protected IApplicationControlFileFactory MockApplicationControlFileFactory;
        protected IEpisodeFinder MockPodcastEpisodeFinder;
        protected INetworkHelper MockNetworkHelper;
        protected IUserSettings MockUserSettings;
        protected IDownloadServiceController MockDownloadServiceController;
        protected IMessageStoreInserter MockMessageStoreInserter;
        protected IPermissionChecker MockPermissionChecker;
        protected IDownloadService MockDownloaderService;

        protected PodcastInfoMocker podcast1Mocker;
        protected PodcastInfoMocker podcast2Mocker;

        // reals
        protected IByteConverter ByteConverter = new ByteConverter();
        protected DownloaderEvents Events = new DownloaderEvents();

        protected void ResetObservedResults()
        {
            ObservedResults.LastSetTitle = null;
            ObservedResults.LastSetEmptyText = null;
            ObservedResults.StartProgress = new List<int>(10);
            ObservedResults.StartProgressCount = 0;
            ObservedResults.UpdateProgress = new List<int>(10);
            ObservedResults.UpdateProgressCount = 0;
            ObservedResults.EndProgressCount = 0;
            ObservedResults.LastDownloadItems = null;
            ObservedResults.LastDisplayMessage = null;
            ObservedResults.DisplayErrorMessageCount = 0;
            ObservedResults.LastDisplayErrorMessage = null;
            ObservedResults.LastCellularPromptTitle = null;
            ObservedResults.LastCellularPromptBody = null;
            ObservedResults.LastCellularPromptOk = null;
            ObservedResults.LastCellularPromptCancel = null;
            ObservedResults.LastEndDownloadingMessage = null;
            ObservedResults.StartDownloadingCount = 0;
            ObservedResults.EndDownloadingCount = 0;
            ObservedResults.LastUpdatePercentageItem = null;
            ObservedResults.LastUpdatePercentage = 0;
            ObservedResults.LastUpdateStatusItem = null;
            ObservedResults.LastUpdateStatusMessage = null;
            ObservedResults.LastUpdateStatus = Status.OK;
            ObservedResults.LastKillPromptTitle = null;
            ObservedResults.LastKillPromptMessage = null;
            ObservedResults.LastKillPromptOk = null;
            ObservedResults.LastKillPromptCancel = null;
        }

        private void SetupResources()
        {
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.download_activity_title)).Returns("Mocked Title");
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.no_downloads_text)).Returns("No downloads");
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.no_network_text)).Returns("No network");
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.download_activity_cancelling)).Returns("cancelling");
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.dialog_title)).Returns("dialog title");
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.download_activity_kill_prompt)).Returns("kill message");
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.download_activity_kill_ok)).Returns("kill ok");
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.download_activity_kill_cancel)).Returns("kill cancel");
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.download_activity_complete)).Returns("downloads complete");
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.download_activity_cellular_prompt)).Returns("cellular prompt");
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.download_activity_cellular_ok)).Returns("cellular ok");
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.download_activity_cellular_cancel)).Returns("cellular cancel");
            A.CallTo(() => MockResourceProvider.GetQuantityString(Resource.Plurals.download_activity_title_after_load, A<int>.Ignored))
                                   .ReturnsLazily((int id, int number) => "download episodes count == " + number.ToString());
        }

        protected void SetupMockControlFileFor2Podcasts(DiagnosticOutputLevel level = DiagnosticOutputLevel.None)
        {
            podcast1Mocker = new PodcastInfoMocker()
                .ApplyFolder(PODCAST_FOLDER_1);
            podcast2Mocker = new PodcastInfoMocker()
                .ApplyFolder(PODCAST_FOLDER_2);

            var podcasts = new List<IPodcastInfo>(2)
            {
                podcast1Mocker.GetMockedPodcastInfo(),
                podcast2Mocker.GetMockedPodcastInfo()
            };

            MockControlFile = new ControlFileMocker()
                .ApplySourceRoot(SOURCE_ROOT)
                .ApplyRetryWaitInSeconds(RETRY_TIME)
                .ApplyDiagnosticRetainTemporaryFiles(DIAGS)
                .ApplyMaximumNumberOfConcurrentDownloads(MAX_DOWNLOADS)
                .ApplyFreeSpaceToLeaveOnDownload(FREE_DISK_SPACE_MB - 1)
                .ApplyDiagnosticOutput(level)
                .ApplyPodcasts(podcasts)
                .GetMockedControlFile();

            A.CallTo(() => MockApplicationControlFileProvider.GetApplicationConfiguration()).Returns(MockControlFile);
        }

        protected void SetupEpisodesFor2Podcasts()
        {
            var syncItemMocker1 = new SyncItemMocker().ApplyId(EPISODE_1_ID).ApplyEpisodeTitle(EPISODE_1_TITLE);
            var syncItemMocker2 = new SyncItemMocker().ApplyId(EPISODE_2_ID).ApplyEpisodeTitle(EPISODE_2_TITLE);
            A.CallTo(() => MockPodcastEpisodeFinder.FindEpisodesToDownload(SOURCE_ROOT, RETRY_TIME, podcast1Mocker.GetMockedPodcastInfo(), DIAGS))
                .Returns(
                    new List<ISyncItem>() 
                    { 
                        syncItemMocker1.GetMockedSyncItem(), 
                        syncItemMocker2.GetMockedSyncItem() 
                    }
                );


            var syncItemMocker3 = new SyncItemMocker().ApplyId(EPISODE_3_ID).ApplyEpisodeTitle(EPISODE_3_TITLE);
            var syncItemMocker4 = new SyncItemMocker().ApplyId(EPISODE_4_ID).ApplyEpisodeTitle(EPISODE_4_TITLE);
            var syncItemMocker5 = new SyncItemMocker().ApplyId(EPISODE_5_ID).ApplyEpisodeTitle(EPISODE_5_TITLE);
            A.CallTo(() => MockPodcastEpisodeFinder.FindEpisodesToDownload(SOURCE_ROOT, RETRY_TIME, podcast2Mocker.GetMockedPodcastInfo(), DIAGS))
                .Returns(
                    new List<ISyncItem>()
                    {
                        syncItemMocker3.GetMockedSyncItem(),
                        syncItemMocker4.GetMockedSyncItem(),
                        syncItemMocker5.GetMockedSyncItem()
                    }
                );
        }

        protected void SetupFoundEpisodes(IPodcastInfo podcastInfo, List<ISyncItem> episodes)
        {
            A.CallTo(() => MockPodcastEpisodeFinder.FindEpisodesToDownload(
                SOURCE_ROOT,
                RETRY_TIME,
                podcastInfo,
                DIAGS)
            ).Returns(episodes);
        }

        [SetUp]
        public void Setup()
        {
            ResetObservedResults();

            MockApplication = A.Fake<Application>();
            A.CallTo(() => MockApplication.PackageName).Returns("com.andrewandderek.podcastutilities");
            MockLogger = A.Fake<ILogger>();
            MockResourceProvider = A.Fake<IResourceProvider>();
            MockApplicationControlFileProvider = A.Fake<IApplicationControlFileProvider>();
            MockAnalyticsEngine = A.Fake<IAnalyticsEngine>();
            MockStatusAndProgressMessageStore = A.Fake<IStatusAndProgressMessageStore>();
            MockApplicationControlFileFactory = A.Fake<IApplicationControlFileFactory>();
            MockPodcastEpisodeFinder = A.Fake<IEpisodeFinder>();
            MockNetworkHelper = A.Fake<INetworkHelper>();
            A.CallTo(() => MockNetworkHelper.ActiveNetworkType).Returns(INetworkHelper.NetworkType.Wifi);
            MockUserSettings = A.Fake<IUserSettings>();
            MockDownloadServiceController = A.Fake<IDownloadServiceController>();
            MockMessageStoreInserter = A.Fake<IMessageStoreInserter>();
            MockPermissionChecker = A.Fake<IPermissionChecker>();
            MockDownloaderService = A.Fake<IDownloadService>();
            A.CallTo(() => MockDownloaderService.GetDownloaderEvents()).Returns(Events);

            SetupResources();

            ViewModel = new DownloadViewModel(
                MockApplication,
                MockLogger,
                MockResourceProvider,
                MockApplicationControlFileProvider,
                MockPodcastEpisodeFinder,
                MockAnalyticsEngine,
                MockStatusAndProgressMessageStore,
                MockNetworkHelper,
                MockUserSettings,
                MockDownloadServiceController,
                MockMessageStoreInserter,
                MockPermissionChecker
            );
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.SetEmptyText += SetEmptyText;
            ViewModel.Observables.StartProgress += StartProgress;
            ViewModel.Observables.UpdateProgress += UpdateProgress;
            ViewModel.Observables.EndProgress += EndProgress;
            ViewModel.Observables.SetSyncItems += SetSyncItems;
            ViewModel.Observables.DisplayMessage += DisplayMessage;
            ViewModel.Observables.DisplayErrorMessage += DisplayErrorMessage;
            ViewModel.Observables.CellularPrompt += CellularPrompt;
            ViewModel.Observables.StartDownloading += StartDownloading;
            ViewModel.Observables.EndDownloading += EndDownloading;
            ViewModel.Observables.UpdateItemProgress += UpdateItemProgress;
            ViewModel.Observables.UpdateItemStatus += UpdateItemStatus;
            ViewModel.Observables.KillPrompt += KillPrompt;
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel.Observables.Title -= SetTitle;
            ViewModel.Observables.SetEmptyText -= SetEmptyText;
            ViewModel.Observables.StartProgress -= StartProgress;
            ViewModel.Observables.UpdateProgress -= UpdateProgress;
            ViewModel.Observables.EndProgress -= EndProgress;
            ViewModel.Observables.SetSyncItems -= SetSyncItems;
            ViewModel.Observables.DisplayMessage -= DisplayMessage;
            ViewModel.Observables.DisplayErrorMessage -= DisplayErrorMessage;
            ViewModel.Observables.CellularPrompt -= CellularPrompt;
            ViewModel.Observables.StartDownloading -= StartDownloading;
            ViewModel.Observables.EndDownloading -= EndDownloading;
            ViewModel.Observables.UpdateItemProgress -= UpdateItemProgress;
            ViewModel.Observables.UpdateItemStatus -= UpdateItemStatus;
            ViewModel.Observables.KillPrompt -= KillPrompt;
        }

        private void DisplayErrorMessage(object sender, string message)
        {
            ObservedResults.LastDisplayErrorMessage = message;
            ObservedResults.DisplayErrorMessageCount++;
        }

        private void KillPrompt(object sender, Tuple<string, string, string, string> prompt)
        {
            (ObservedResults.LastKillPromptTitle, ObservedResults.LastKillPromptMessage, ObservedResults.LastKillPromptOk, ObservedResults.LastKillPromptCancel) = prompt;
        }

        private void UpdateItemStatus(object sender, Tuple<ISyncItem, Status, string> item)
        {
            (ObservedResults.LastUpdateStatusItem, ObservedResults.LastUpdateStatus, ObservedResults.LastUpdateStatusMessage) = item;
        }

        private void UpdateItemProgress(object sender, Tuple<ISyncItem, int> item)
        {
            (ObservedResults.LastUpdatePercentageItem, ObservedResults.LastUpdatePercentage) = item;
        }

        private void StartDownloading(object sender, EventArgs e)
        {
            ObservedResults.StartDownloadingCount++;
        }

        private void EndDownloading(object sender, string message)
        {
            ObservedResults.EndDownloadingCount++;
            ObservedResults.LastEndDownloadingMessage = message;
        }

        private void CellularPrompt(object sender, Tuple<string, string, string, string> prompt)
        {
            (
                ObservedResults.LastCellularPromptTitle,
                ObservedResults.LastCellularPromptBody,
                ObservedResults.LastCellularPromptOk,
                ObservedResults.LastCellularPromptCancel
            ) = prompt;
        }

        private void DisplayMessage(object sender, string message)
        {
            ObservedResults.LastDisplayMessage = message;
        }

        private void StartProgress(object sender, int max)
        {
            ObservedResults.StartProgress.Add(max);
            ObservedResults.StartProgressCount++;
        }

        private void UpdateProgress(object sender, int position)
        {
            ObservedResults.UpdateProgress.Add(position);
            ObservedResults.UpdateProgressCount++;
        }

        private void EndProgress(object sender, EventArgs e)
        {
            ObservedResults.EndProgressCount++;
        }

        private void SetSyncItems(object sender, List<DownloadRecyclerItem> items)
        {
            ObservedResults.LastDownloadItems = items;
        }

        private void SetEmptyText(object sender, string text)
        {
            ObservedResults.LastSetEmptyText = text;
        }

        private void SetTitle(object sender, string title)
        {
            ObservedResults.LastSetTitle = title;
        }
    }

}
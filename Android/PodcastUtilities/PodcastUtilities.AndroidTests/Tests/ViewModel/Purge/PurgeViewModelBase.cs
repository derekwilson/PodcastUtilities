using Android.App;
using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel.Purge;
using PodcastUtilities.AndroidTests.Helpers;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Purge
{
    public class PurgeViewModelBase
    {
        protected const string SOURCE_ROOT = "/sdcard/sourceroot";
        protected const string PODCAST_FOLDER_1 = "folder1";
        protected const string PODCAST_FOLDER_2 = "folder2";

        protected PurgeViewModel ViewModel = null!;

        public class ObservedResultsGroup
        {
            public string? LastSetTitle;
            public List<int>? StartProgress;
            public int StartProgressCount;
            public List<int>? UpdateProgress;
            public int UpdateProgressCount;
            public int EndProgressCount;
            public List<PurgeRecyclerItem>? LastPurgeItems;
            public string? LastDisplayMessage;
        }
        protected ObservedResultsGroup ObservedResults = new ObservedResultsGroup();


        // mocks
        protected Application MockApplication = null!;
        protected ILogger MockLogger = null!;
        protected IFileUtilities MockFileUtilities = null!;
        protected IResourceProvider MockResourceProvider = null!;
        protected IEpisodePurger MockEpisodePurger = null!;
        protected IApplicationControlFileProvider MockApplicationControlFileProvider = null!;
        protected IReadWriteControlFile MockControlFile = null!;
        protected ICrashReporter MockCrashReporter = null!;
        protected IAnalyticsEngine MockAnalyticsEngine = null!;

        protected PodcastInfoMocker podcast1Mocker = null!;
        protected PodcastInfoMocker podcast2Mocker = null!;

        protected void ResetObservedResults()
        {
            ObservedResults.LastSetTitle = null;
            ObservedResults.StartProgress = new List<int>(10);
            ObservedResults.StartProgressCount = 0;
            ObservedResults.UpdateProgress = new List<int>(10);
            ObservedResults.UpdateProgressCount = 0;
            ObservedResults.EndProgressCount = 0;
            ObservedResults.LastPurgeItems = null;
            ObservedResults.LastDisplayMessage = null;
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
                .ApplyPodcasts(podcasts)
                .GetMockedControlFile();

            A.CallTo(() => MockApplicationControlFileProvider.GetApplicationConfiguration()).Returns(MockControlFile);
        }

        protected void SetupMockPurger()
        {
            var file1 = new FileInfoMocker()
                .ApplyFullName("File1Full")
                .ApplyName("File1");
            var file2 = new FileInfoMocker()
                .ApplyFullName("File2Full")
                .ApplyName("File2");
            var files = new List<IFileInfo>(2)
            {
                // these are not sorted correctly
                file2.GetMockedFileInfo(),
                file1.GetMockedFileInfo()
            };
            var dir1 = new DirectoryInfoMocker()
                .ApplyFullName("Dir1Full");
            var dir2 = new DirectoryInfoMocker()
                .ApplyFullName("Dir2Full");

            A.CallTo(() => MockEpisodePurger.FindEpisodesToPurge(SOURCE_ROOT, podcast1Mocker.GetMockedPodcastInfo())).Returns(files);
            A.CallTo(() => MockEpisodePurger.FindEmptyFoldersToDelete(SOURCE_ROOT, podcast1Mocker.GetMockedPodcastInfo(), A<IList<IFileInfo>>.Ignored)).Returns(dir1.GetMockedDirectoryInfoAsList());
            A.CallTo(() => MockEpisodePurger.FindEmptyFoldersToDelete(SOURCE_ROOT, podcast2Mocker.GetMockedPodcastInfo(), A<IList<IFileInfo>>.Ignored)).Returns(dir2.GetMockedDirectoryInfoAsList());
        }

        private void SetupResources()
        {
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.purge_activity_title)).Returns("Mocked Title");
            A.CallTo(() => MockResourceProvider.GetQuantityString(Resource.Plurals.purge_activity_title_after_load, A<int>.Ignored))
                                   .ReturnsLazily((int id, int number) => "item count == " + number.ToString());
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.error_delete_item)).Returns("Error {0}");
        }

        [SetUp]
        public void Setup()
        {
            ResetObservedResults();

            MockApplication = A.Fake<Application>();
            A.CallTo(() => MockApplication.PackageName).Returns("com.andrewandderek.podcastutilities");
            MockLogger = A.Fake<ILogger>();
            MockResourceProvider = A.Fake<IResourceProvider>();
            MockFileUtilities = A.Fake<IFileUtilities>();
            MockApplicationControlFileProvider = A.Fake<IApplicationControlFileProvider>();
            MockCrashReporter = A.Fake<ICrashReporter>();
            MockAnalyticsEngine = A.Fake<IAnalyticsEngine>();
            MockEpisodePurger = A.Fake<IEpisodePurger>();

            SetupResources();

            ViewModel = new PurgeViewModel(
                MockApplication,
                MockLogger,
                MockResourceProvider,
                MockApplicationControlFileProvider,
                MockEpisodePurger,
                MockFileUtilities,
                MockCrashReporter,
                MockAnalyticsEngine
            );
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.StartProgress += StartProgress;
            ViewModel.Observables.UpdateProgress += UpdateProgress;
            ViewModel.Observables.EndProgress += EndProgress;
            ViewModel.Observables.SetPurgeItems += SetPurgeItems;
            ViewModel.Observables.DisplayMessage += DisplayMessage;
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel.Observables.Title -= SetTitle;
            ViewModel.Observables.StartProgress -= StartProgress;
            ViewModel.Observables.UpdateProgress -= UpdateProgress;
            ViewModel.Observables.EndProgress -= EndProgress;
            ViewModel.Observables.SetPurgeItems -= SetPurgeItems;
            ViewModel.Observables.DisplayMessage -= DisplayMessage;
        }

        private void SetTitle(object? sender, string text)
        {
            ObservedResults.LastSetTitle = text;
        }

        private void StartProgress(object? sender, int max)
        {
            ObservedResults.StartProgress?.Add(max);
            ObservedResults.StartProgressCount++;
        }

        private void UpdateProgress(object? sender, int position)
        {
            ObservedResults.UpdateProgress?.Add(position);
            ObservedResults.UpdateProgressCount++;
        }

        private void EndProgress(object? sender, EventArgs e)
        {
            ObservedResults.EndProgressCount++;
        }

        private void SetPurgeItems(object? sender, List<PurgeRecyclerItem> items)
        {
            ObservedResults.LastPurgeItems = items;
        }

        private void DisplayMessage(object? sender, string message)
        {
            ObservedResults.LastDisplayMessage = message;
        }

    }
}
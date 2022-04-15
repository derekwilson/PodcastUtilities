using Android.App;
using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.ViewModel.Main;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.Common.Playlists;
using System;
using System.Collections.Generic;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.AndroidTests.Helpers;
using PodcastUtilities.AndroidLogic.CustomViews;
using Android.Views;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Main
{
    public class MainViewModelBase
    {
        protected const string PATH1 = "/path/to/file1";
        protected const string PATH2 = "/path/to/file2";
        private const int BYTES_PER_MB = 1024 * 1024;

        protected MainViewModel ViewModel;

        public class ObservedResultsGroup
        {
            public string LastSetTitle;
            public string LastSetCacheRoot;
            public string LastSetFeedHeading;
            public List<PodcastFeedRecyclerItem> LastSetFeedItems;
            public int ShowNoDriveMessageCount;
        }
        protected ObservedResultsGroup ObservedResults = new ObservedResultsGroup();


        // mocks
        protected Application MockApplication;
        protected ILogger MockLogger;
        protected IResourceProvider MockResourceProvider;
        protected IFileSystemHelper MockFileSystemHelper;
        protected IApplicationControlFileProvider MockApplicationControlFileProvider;
        protected ICrashReporter MockCrashReporter;
        protected IAnalyticsEngine MockAnalyticsEngine;
        protected IGenerator MockPlaylistGenerator;
        protected IReadWriteControlFile MockControlFile;
        protected IDriveVolumeInfoViewFactory MockDriveVolumeInfoViewFactory;
        protected IDriveVolumeInfoView MockDriveVolumeInfoView;

        // reals
        protected IByteConverter ByteConverter = new ByteConverter();

        protected void ResetObservedResults()
        {
            ObservedResults.LastSetTitle = null;
            ObservedResults.LastSetCacheRoot = null;
            ObservedResults.LastSetFeedHeading = null;
            ObservedResults.LastSetFeedItems = null;
            ObservedResults.ShowNoDriveMessageCount = 0;
        }

        private void SetupResources()
        {
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.main_activity_title)).Returns("Mocked Title");
            A.CallTo(() => MockResourceProvider.GetQuantityString(Resource.Plurals.feed_list_heading, A<int>.Ignored))
                                   .ReturnsLazily((int id, int number) => "feed count == " + number.ToString());
        }

        protected void SetupControlFileFor1Podcast()
        {
            var podcastMocker = new PodcastInfoMocker().ApplyFolder("folder1");
            var controlFileMocker = new ControlFileMocker();
            MockControlFile = controlFileMocker
                .ApplySourceRoot("/sdcard/sourceroot")
                .ApplyPodcasts(podcastMocker.GetMockedPodcastInfoAsList())
                .GetMockedControlFile();

            A.CallTo(() => MockApplicationControlFileProvider.GetApplicationConfiguration()).Returns(MockControlFile);
        }

        protected void SetupFileSystem()
        {
            var MockFile1 = A.Fake<Java.IO.File>();
            A.CallTo(() => MockFile1.AbsolutePath).Returns(PATH1);
            var MockFile2 = A.Fake<Java.IO.File>();
            A.CallTo(() => MockFile2.AbsolutePath).Returns(PATH2);

            Java.IO.File[] files = new Java.IO.File[]  {
                MockFile1,
                MockFile2
            };
            A.CallTo(() => MockFileSystemHelper.GetApplicationExternalFilesDirs()).Returns(files);

            A.CallTo(() => MockFileSystemHelper.GetAvailableFileSystemSizeInBytes(PATH1)).Returns(100 * BYTES_PER_MB);
            A.CallTo(() => MockFileSystemHelper.GetTotalFileSystemSizeInBytes(PATH1)).Returns(200 * BYTES_PER_MB);
            A.CallTo(() => MockFileSystemHelper.GetAvailableFileSystemSizeInBytes(PATH2)).Returns(2000 * BYTES_PER_MB);
            A.CallTo(() => MockFileSystemHelper.GetTotalFileSystemSizeInBytes(PATH2)).Returns(4000L * BYTES_PER_MB);
        }

        [SetUp]
        public void Setup()
        {
            ResetObservedResults();

            MockApplication = A.Fake<Application>();
            MockLogger = A.Fake<ILogger>();
            MockResourceProvider = A.Fake<IResourceProvider>();
            MockFileSystemHelper = A.Fake<IFileSystemHelper>();
            MockApplicationControlFileProvider = A.Fake<IApplicationControlFileProvider>();
            MockCrashReporter = A.Fake<ICrashReporter>();
            MockAnalyticsEngine = A.Fake<IAnalyticsEngine>();
            MockPlaylistGenerator = A.Fake<IGenerator>();
            MockDriveVolumeInfoViewFactory = A.Fake<IDriveVolumeInfoViewFactory>();
            MockDriveVolumeInfoView = A.Fake<IDriveVolumeInfoView>();
            A.CallTo(() => MockDriveVolumeInfoViewFactory.GetNewView(MockApplication)).Returns(MockDriveVolumeInfoView);

            ByteConverter = new ByteConverter();

            SetupResources();

            ViewModel = new MainViewModel(
                MockApplication, 
                MockLogger, 
                MockResourceProvider, 
                MockApplicationControlFileProvider, 
                MockFileSystemHelper, 
                ByteConverter,
                MockCrashReporter,
                MockAnalyticsEngine,
                MockPlaylistGenerator,
                MockDriveVolumeInfoViewFactory
            );
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.SetCacheRoot += SetCacheRoot;
            ViewModel.Observables.SetFeedItems += SetFeedItems;
            ViewModel.Observables.ShowNoDriveMessage += ShowNoDriveMessage;
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel.Observables.Title -= SetTitle;
            ViewModel.Observables.SetCacheRoot -= SetCacheRoot;
            ViewModel.Observables.SetFeedItems -= SetFeedItems;
            ViewModel.Observables.ShowNoDriveMessage -= ShowNoDriveMessage;
        }

        private void SetFeedItems(object sender, Tuple<string, List<PodcastFeedRecyclerItem>> items)
        {
            (ObservedResults.LastSetFeedHeading, ObservedResults.LastSetFeedItems) = items;
        }

        private void SetCacheRoot(object sender, string root)
        {
            ObservedResults.LastSetCacheRoot = root;
        }

        private void SetTitle(object sender, string title)
        {
            ObservedResults.LastSetTitle = title;
        }

        private void ShowNoDriveMessage(object sender, EventArgs e)
        {
            ObservedResults.ShowNoDriveMessageCount++;
        }

    }
}
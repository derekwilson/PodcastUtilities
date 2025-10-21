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

        // the package name needs to match the mocked one in Setup
        protected const string PATH3 = "/storage/SDCARD1/Android/data/com.andrewandderek.podcastutilities/path/to/file3";
        protected const string PATH4 = "/storage/SDCARD2/anotherplace/com.andrewandderek.podcastutilities/path/to/file4";
        protected const string PATH5 = "/storage/SDCARD3/Android/data/anotherplace/path/to/file5";
        protected const string PATH6 = "/Android/data/com.andrewandderek.podcastutilities/path/to/file6";
        protected const string PATH7 = "/storage/emulated/0/Android/data/com.andrewandderek.podcastutilities/path/to/file7";


        private const int BYTES_PER_MB = 1024 * 1024;

        protected MainViewModel ViewModel = null!;

        public class ObservedResultsGroup
        {
            public string? LastSetTitle;
            public string? LastSetCacheRoot;
            public string? LastSetFeedHeading;
            public List<PodcastFeedRecyclerItem>? LastSetFeedItems;
            public int? ShowNoDriveMessageCount;
            public string? LastToastMessage;
        }
        protected ObservedResultsGroup ObservedResults = new ObservedResultsGroup();


        // mocks
        protected Application MockApplication = null!;  // this gets passed to the AndroidViewModel ctor
        protected ILogger MockLogger = null!;
        protected IResourceProvider MockResourceProvider = null!;
        protected IFileSystemHelper MockFileSystemHelper = null!;
        protected IApplicationControlFileProvider MockApplicationControlFileProvider = null!;
        protected ICrashReporter MockCrashReporter = null!;
        protected IAnalyticsEngine MockAnalyticsEngine = null!;
        protected IGenerator MockPlaylistGenerator = null!;
        protected IReadWriteControlFile MockControlFile = null!;
        protected IDriveVolumeInfoViewFactory MockDriveVolumeInfoViewFactory = null!;
        protected IDriveVolumeInfoView MockDriveVolumeInfoView = null!;
        protected IApplicationControlFileFactory MockApplicationControlFileFactory = null!;
        protected IValueFormatter MockValueFormatter = null!;
        protected IAndroidApplication MockAndroidApplication = null!;

        // reals
        protected IByteConverter ByteConverter = new ByteConverter();

        protected void ResetObservedResults()
        {
            ObservedResults.LastSetTitle = null;
            ObservedResults.LastSetCacheRoot = null;
            ObservedResults.LastSetFeedHeading = null;
            ObservedResults.LastSetFeedItems = null;
            ObservedResults.ShowNoDriveMessageCount = 0;
            ObservedResults.LastToastMessage = null;
        }

        private void SetupResources()
        {
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.main_activity_title)).Returns("Mocked Title");
            A.CallTo(() => MockResourceProvider.GetQuantityString(Resource.Plurals.feed_list_heading, A<int>.Ignored))
                                   .ReturnsLazily((int id, int number) => "feed count == " + number.ToString());
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.control_file_loaded)).Returns("Mocked Control file loaded");
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.error_reading_control_file)).Returns("Mocked control file error");
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.error_generating_playlist)).Returns("Mocked playlist error");
        }

        protected void SetupMockControlFileFor1Podcast()
        {
            var podcastMocker = new PodcastInfoMocker()
                .ApplyFolder("folder1");
            MockControlFile = new ControlFileMocker()
                .ApplySourceRoot("/sdcard/sourceroot")
                .ApplyPlaylistFormat(PlaylistFormat.M3U)
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

        protected void SetupFileSystemStorageCardPaths()
        {
            var MockFile1 = A.Fake<Java.IO.File>();
            A.CallTo(() => MockFile1.AbsolutePath).Returns(PATH3);
            var MockFile2 = A.Fake<Java.IO.File>();
            A.CallTo(() => MockFile2.AbsolutePath).Returns(PATH4);
            var MockFile3 = A.Fake<Java.IO.File>();
            A.CallTo(() => MockFile3.AbsolutePath).Returns(PATH5);
            var MockFile4 = A.Fake<Java.IO.File>();
            A.CallTo(() => MockFile4.AbsolutePath).Returns(PATH6);
            var MockFile5 = A.Fake<Java.IO.File>();
            A.CallTo(() => MockFile5.AbsolutePath).Returns(PATH7);

            Java.IO.File[] files = new Java.IO.File[]  {
                MockFile1,
                MockFile2,
                MockFile3,
                MockFile4,
                MockFile5
            };
            A.CallTo(() => MockFileSystemHelper.GetApplicationExternalFilesDirs()).Returns(files);

            A.CallTo(() => MockFileSystemHelper.GetAvailableFileSystemSizeInBytes(A<string>.Ignored)).Returns(100 * BYTES_PER_MB);
            A.CallTo(() => MockFileSystemHelper.GetTotalFileSystemSizeInBytes(A<string>.Ignored)).Returns(200 * BYTES_PER_MB);
        }

        [SetUp]
        public void Setup()
        {
            ResetObservedResults();

            MockApplication = A.Fake<Application>();
            A.CallTo(() => MockApplication.PackageName).Returns("com.andrewandderek.podcastutilities");
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
            MockApplicationControlFileFactory = A.Fake<IApplicationControlFileFactory>();
            MockValueFormatter = A.Fake<IValueFormatter>();
            MockAndroidApplication = A.Fake<IAndroidApplication>();

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
                MockDriveVolumeInfoViewFactory,
                MockValueFormatter,
                MockAndroidApplication
            );
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.SetCacheRoot += SetCacheRoot;
            ViewModel.Observables.SetFeedItems += SetFeedItems;
            ViewModel.Observables.ShowNoDriveMessage += ShowNoDriveMessage;
            ViewModel.Observables.AddInfoView += AddInfoView;
            ViewModel.Observables.ToastMessage += ToastMessage;
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel.Observables.Title -= SetTitle;
            ViewModel.Observables.SetCacheRoot -= SetCacheRoot;
            ViewModel.Observables.SetFeedItems -= SetFeedItems;
            ViewModel.Observables.ShowNoDriveMessage -= ShowNoDriveMessage;
            ViewModel.Observables.AddInfoView -= AddInfoView;
            ViewModel.Observables.ToastMessage -= ToastMessage;
        }

        private void ToastMessage(object? sender, string message)
        {
            ObservedResults.LastToastMessage = message;
        }

        private void AddInfoView(object? sender, View view)
        {
            // we dont need to do anything but if we dont observe it then Invoke method never happens and
            // the DriveVolumeInfoView.GetView() method isnt called
            // its also not a good test of the real use of the ViewModel
        }

        private void SetFeedItems(object? sender, Tuple<string, List<PodcastFeedRecyclerItem>> items)
        {
            (ObservedResults.LastSetFeedHeading, ObservedResults.LastSetFeedItems) = items;
        }

        private void SetCacheRoot(object? sender, string root)
        {
            ObservedResults.LastSetCacheRoot = root;
        }

        private void SetTitle(object? sender, string title)
        {
            ObservedResults.LastSetTitle = title;
        }

        private void ShowNoDriveMessage(object? sender, EventArgs e)
        {
            ObservedResults.ShowNoDriveMessageCount++;
        }

    }
}
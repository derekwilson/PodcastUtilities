using Android.App;
using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Settings;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel.Download;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Download
{
    public class DownloadViewModelBase
    {
        protected DownloadViewModel ViewModel;

        public class ObservedResultsGroup
        {
            public string LastSetTitle;
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
        protected IReadWriteControlFile MockControlFile;
        protected IApplicationControlFileFactory MockApplicationControlFileFactory;
        protected IEpisodeFinder MockPodcastEpisodeFinder;
        protected ISyncItemToEpisodeDownloaderTaskConverter MockSyncItemToEpisodeDownloaderTaskConverter;
        protected ITaskPool MockTaskPool;
        protected IStatusAndProgressMessageStore MockStatusAndProgressMessageStore;
        protected INetworkHelper MockNetworkHelper;
        protected IUserSettings MockUserSettings;

        // reals
        protected IByteConverter ByteConverter = new ByteConverter();

        protected void ResetObservedResults()
        {
            ObservedResults.LastSetTitle = null;
        }

        private void SetupResources()
        {
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.download_activity_title)).Returns("Mocked Title");
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
            MockApplicationControlFileFactory = A.Fake<IApplicationControlFileFactory>();
            MockPodcastEpisodeFinder = A.Fake<IEpisodeFinder>();
            MockSyncItemToEpisodeDownloaderTaskConverter = A.Fake<ISyncItemToEpisodeDownloaderTaskConverter>();
            MockTaskPool = A.Fake<ITaskPool>();
            MockStatusAndProgressMessageStore = A.Fake<IStatusAndProgressMessageStore>();
            MockNetworkHelper = A.Fake<INetworkHelper>();
            MockUserSettings = A.Fake<IUserSettings>();

            ByteConverter = new ByteConverter();

            SetupResources();

            ViewModel = new DownloadViewModel(
                MockApplication,
                MockLogger,
                MockResourceProvider,
                MockApplicationControlFileProvider,
                MockFileSystemHelper,
                ByteConverter,
                MockPodcastEpisodeFinder,
                MockSyncItemToEpisodeDownloaderTaskConverter,
                MockTaskPool,
                MockCrashReporter,
                MockAnalyticsEngine,
                MockStatusAndProgressMessageStore,
                MockNetworkHelper,
                MockUserSettings
            );
            ViewModel.Observables.Title += SetTitle;
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel.Observables.Title -= SetTitle;
        }

        private void SetTitle(object sender, string title)
        {
            ObservedResults.LastSetTitle = title;
        }
    }

}
using Android.App;
using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel.Configure;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Configure
{
    public class EditConfigViewModelBase
    {
        protected EditConfigViewModel ViewModel;

        public class ObservedResultsGroup
        {
            public string LastMessage;
        }
        protected ObservedResultsGroup ObservedResults = new ObservedResultsGroup();

        // mocks
        protected Application MockApplication;
        protected ILogger MockLogger;
        protected IResourceProvider MockResourceProvider;
        protected IFileSystemHelper MockFileSystemHelper;
        protected IApplicationControlFileProvider MockApplicationControlFileProvider;
        protected IApplicationControlFileFactory MockApplicationControlFileFactory;
        protected ICrashReporter MockCrashReporter;
        protected IAnalyticsEngine MockAnalyticsEngine;
        protected IReadWriteControlFile MockControlFile;
        protected IValueFormatter MockValueFormatter;

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
            MockApplicationControlFileFactory = A.Fake<IApplicationControlFileFactory>();
            MockCrashReporter = A.Fake<ICrashReporter>();
            MockAnalyticsEngine = A.Fake<IAnalyticsEngine>();
            MockValueFormatter = A.Fake<IValueFormatter>();

            SetupResources();

            ViewModel = new EditConfigViewModel(
                MockApplication,
                MockLogger,
                MockResourceProvider,
                MockApplicationControlFileProvider,
                MockCrashReporter,
                MockAnalyticsEngine,
                MockFileSystemHelper,
                MockApplicationControlFileFactory,
                MockValueFormatter
            );

            ViewModel.Observables.DisplayMessage += DisplayMessage;
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel.Observables.DisplayMessage -= DisplayMessage;
        }

        private void SetupResources()
        {
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.edit_config_activity_title)).Returns("Mocked Title");
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.control_file_loaded)).Returns("Mocked Control file loaded");
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.error_reading_control_file)).Returns("Mocked control file error");
        }

        protected void ResetObservedResults()
        {
            ObservedResults.LastMessage = null;
        }

        private void DisplayMessage(object sender, string message)
        {
            ObservedResults.LastMessage = message;
        }
    }
}
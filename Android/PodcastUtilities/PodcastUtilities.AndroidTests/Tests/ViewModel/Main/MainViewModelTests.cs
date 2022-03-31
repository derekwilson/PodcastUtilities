using Android.App;
using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.ViewModel.Main;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Main
{
    [TestFixture]
    public class MainViewModelTests
    {
        private MainViewModel ViewModel;

        private string LastSetTitle;

        // mocks
        protected Application MockApplication = A.Fake<Application>();
        protected ILogger MockLogger = A.Fake<ILogger>();
        protected IResourceProvider MockResourceProvider = A.Fake<IResourceProvider>();
        protected IFileSystemHelper MockFileSystemHelper = A.Fake<IFileSystemHelper>();
        protected IApplicationControlFileProvider MockApplicationControlFileProvider = A.Fake<IApplicationControlFileProvider>();
        protected ICrashReporter MockCrashReporter = A.Fake<ICrashReporter>();
        protected IAnalyticsEngine MockAnalyticsEngine = A.Fake<IAnalyticsEngine>();
        protected IGenerator MockPlaylistGenerator = A.Fake<IGenerator>();

        // reals
        protected IByteConverter ByteConverter = new ByteConverter();

        [SetUp]
        public void Setup()
        {
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.main_activity_title)).Returns("Mocked Title");
            ViewModel = new MainViewModel(
                MockApplication, 
                MockLogger, 
                MockResourceProvider, 
                MockApplicationControlFileProvider, 
                MockFileSystemHelper, 
                ByteConverter,
                MockCrashReporter,
                MockAnalyticsEngine,
                MockPlaylistGenerator
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
            LastSetTitle = title;
        }

        [Test]
        public void Initialise_Sets_Title()
        {
            ViewModel.Initialise();

            Assert.AreEqual("Mocked Title", LastSetTitle);
        }

        [Test]
        public void Initialise_Logs()
        {
            ViewModel.Initialise();

            A.CallTo(() => MockLogger.Debug(A<ILogger.MessageGenerator>.Ignored)).MustHaveHappened(2, Times.Exactly);
        }
    }
}
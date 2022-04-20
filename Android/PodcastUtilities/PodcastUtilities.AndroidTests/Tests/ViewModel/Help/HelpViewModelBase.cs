using Android.App;
using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel.Help;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Help
{
    public class HelpViewModelBase
    {
        protected HelpViewModel ViewModel;

        public class ObservedResultsGroup
        {
            public string LastSetText;
        }
        protected ObservedResultsGroup ObservedResults = new ObservedResultsGroup();


        // mocks
        protected Application MockApplication;
        protected ILogger MockLogger;
        protected IFileSystemHelper MockFileSystemHelper;

        protected void ResetObservedResults()
        {
            ObservedResults.LastSetText = null;
        }

        protected void SetupHelpText()
        {
            FileSystemHelper helper = new FileSystemHelper(MainActivity.MainContext, MockLogger);
            string text = helper.GetAssetsFileContents("help/testhelp.html", false);
            // substitute the real text with some from our test app
            A.CallTo(() => MockFileSystemHelper.GetAssetsFileContents("help/help.html", false)).Returns(text);
        }

        [SetUp]
        public void Setup()
        {
            ResetObservedResults();

            MockApplication = A.Fake<Application>();
            A.CallTo(() => MockApplication.PackageName).Returns("com.andrewandderek.podcastutilities");
            MockLogger = A.Fake<ILogger>();
            MockFileSystemHelper = A.Fake<IFileSystemHelper>();

            ViewModel = new HelpViewModel(
                MockApplication,
                MockLogger,
                MockFileSystemHelper
            );
            ViewModel.Observables.SetText += SetText;
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel.Observables.SetText -= SetText;
        }

        private void SetText(object sender, string text)
        {
            ObservedResults.LastSetText = text;
        }
    }
}
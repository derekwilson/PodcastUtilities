using Android.App;
using Android.Text;
using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel.Help;
using System;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Help
{
    public class HelpViewModelBase
    {
        protected HelpViewModel ViewModel = null!;

        public class ObservedResultsGroup
        {
            public string? LastSetText;
        }
        protected ObservedResultsGroup ObservedResults = new ObservedResultsGroup();


        // mocks
        protected Android.App.Application MockApplication = null!;
        protected ILogger MockLogger = null!;
        protected IFileSystemHelper MockFileSystemHelper = null!;
        protected Html.IImageGetter MockImageGetter = null!;
        protected IAnalyticsEngine MockAnalyticsEngine = null!;

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

            MockApplication = A.Fake<Android.App.Application>();
            A.CallTo(() => MockApplication.PackageName).Returns("com.andrewandderek.podcastutilities");
            MockLogger = A.Fake<ILogger>();
            MockFileSystemHelper = A.Fake<IFileSystemHelper>();
            MockImageGetter = A.Fake<Html.IImageGetter>();
            MockAnalyticsEngine = A.Fake<IAnalyticsEngine>();

            ViewModel = new HelpViewModel(
                MockApplication,
                MockLogger,
                MockFileSystemHelper,
                MockImageGetter,
                MockAnalyticsEngine
            );
            ViewModel.Observables.SetText += SetText;
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel.Observables.SetText -= SetText;
        }

        private void SetText(object? sender, Tuple<string, Html.IImageGetter> parameters)
        {
            (string text, Html.IImageGetter getter) = parameters;
            ObservedResults.LastSetText = text;
        }
    }
}
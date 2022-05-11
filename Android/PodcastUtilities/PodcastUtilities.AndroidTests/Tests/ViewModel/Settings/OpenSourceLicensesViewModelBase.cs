using Android.App;
using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel.Help;
using PodcastUtilities.AndroidLogic.ViewModel.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Settings
{
    public class OpenSourceLicensesViewModelBase
    {
        protected OpenSourceLicensesViewModel ViewModel;

        public class ObservedResultsGroup
        {
            public StringBuilder Text;
            public int ScrollToTopCount;
        }
        protected ObservedResultsGroup ObservedResults = new ObservedResultsGroup();


        // mocks
        protected Application MockApplication;
        protected ILogger MockLogger;
        protected IFileSystemHelper MockFileSystemHelper;
        protected IResourceProvider MockResourceProvider;

        protected void ResetObservedResults()
        {
            ObservedResults.Text = new StringBuilder();
            ObservedResults.ScrollToTopCount = 0;
        }

        protected void SetupLicenseText()
        {
            FileSystemHelper helper = new FileSystemHelper(MainActivity.MainContext, MockLogger);
            string text = helper.GetAssetsFileContents("license/test.txt", true);
            // substitute the real text with some from our test app
            List<string> files = new List<string>();
            files.Add("file1");
            A.CallTo(() => MockFileSystemHelper.GetAssetsFolderFiles("license")).Returns(files);
            A.CallTo(() => MockFileSystemHelper.GetAssetsFileContents("file1", true)).Returns(text);
        }

        [SetUp]
        public void Setup()
        {
            ResetObservedResults();

            MockApplication = A.Fake<Application>();
            A.CallTo(() => MockApplication.PackageName).Returns("com.andrewandderek.podcastutilities");
            MockLogger = A.Fake<ILogger>();
            MockFileSystemHelper = A.Fake<IFileSystemHelper>();
            MockResourceProvider = A.Fake<IResourceProvider>();

            ViewModel = new OpenSourceLicensesViewModel(
                MockApplication,
                MockLogger,
                MockResourceProvider,
                MockFileSystemHelper
            );
            ViewModel.Observables.ScrollToTop += ScrollToTop;
            ViewModel.Observables.ResetText += ResetText;
            ViewModel.Observables.AddText += AddText;
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel.Observables.ScrollToTop -= ScrollToTop;
            ViewModel.Observables.ResetText -= ResetText;
            ViewModel.Observables.AddText -= AddText;
        }

        private void ScrollToTop(object sender, EventArgs e)
        {
            ObservedResults.ScrollToTopCount++;
        }
        private void AddText(object sender, Tuple<string, string> textBlock)
        {
            (string title, string text) = textBlock;
            ObservedResults.Text.Append(title);
            ObservedResults.Text.Append("\n");
            ObservedResults.Text.Append(text);
        }

        private void ResetText(object sender, EventArgs e)
        {
            ObservedResults.Text.Clear();
        }

    }
}
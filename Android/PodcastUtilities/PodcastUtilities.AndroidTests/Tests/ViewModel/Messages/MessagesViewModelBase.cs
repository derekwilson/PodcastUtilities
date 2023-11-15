using Android.App;
using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel.Messages;
using System;
using System.Text;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Messages
{
    public class MessagesViewModelBase
    {
        protected Guid ITEM_ID = Guid.Parse("72288452-3CD8-4BB4-BBF5-854C290CB499");

        protected MessagesViewModel ViewModel;

        public class ObservedResultsGroup
        {
            public StringBuilder Messages;
            public int ScrollToTopCount;
        }
        protected ObservedResultsGroup ObservedResults = new ObservedResultsGroup();


        // mocks
        protected Application MockApplication;
        protected ILogger MockLogger;
        protected IResourceProvider MockResourceProvider;
        protected ICrashReporter MockCrashReporter;
        protected IAnalyticsEngine MockAnalyticsEngine;

        // reals
        protected IStatusAndProgressMessageStore StatusAndProgressMessageStore;

        protected void ResetObservedResults()
        {
            ObservedResults.Messages = new StringBuilder();
            ObservedResults.ScrollToTopCount = 0;
        }

        protected void SetupStore()
        {
            StatusAndProgressMessageStore.Reset();
            StatusAndProgressMessageStore.StoreMessage(ITEM_ID, "line 1");
            StatusAndProgressMessageStore.StoreMessage(ITEM_ID, "line 2");
            StatusAndProgressMessageStore.StoreMessage(ITEM_ID, "line 3");
        }

        [SetUp]
        public void Setup()
        {
            ResetObservedResults();

            MockApplication = A.Fake<Application>();
            A.CallTo(() => MockApplication.PackageName).Returns("com.andrewandderek.podcastutilities");
            MockLogger = A.Fake<ILogger>();
            MockResourceProvider = A.Fake<IResourceProvider>();
            MockCrashReporter = A.Fake<ICrashReporter>();
            MockAnalyticsEngine = A.Fake<IAnalyticsEngine>();
            StatusAndProgressMessageStore = new StatusAndProgressMessageStore(MockCrashReporter);

            ViewModel = new MessagesViewModel(
                MockApplication,
                MockLogger,
                MockResourceProvider,
                StatusAndProgressMessageStore,
                MockAnalyticsEngine
            );
            ViewModel.Observables.ResetText += ResetText;
            ViewModel.Observables.AddText += AddText;
            ViewModel.Observables.ScrollToTop += ScrollToTop;
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel.Observables.ResetText -= ResetText;
            ViewModel.Observables.AddText -= AddText;
            ViewModel.Observables.ScrollToTop -= ScrollToTop;
        }

        private void ScrollToTop(object sender, EventArgs e)
        {
            ObservedResults.ScrollToTopCount++;
        }

        private void AddText(object sender, string line)
        {
            ObservedResults.Messages.Append(line);
        }

        private void ResetText(object sender, EventArgs e)
        {
            ObservedResults.Messages.Clear();
        }
    }
}
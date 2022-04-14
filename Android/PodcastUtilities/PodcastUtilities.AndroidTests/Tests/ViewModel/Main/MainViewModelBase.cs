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

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Main
{
    [TestFixture]
    public class MainViewModelBase
    {
        protected MainViewModel ViewModel;

        protected string LastSetTitle;
        protected string LastSetCacheRoot;
        protected string LastSetFeedHeading;
        protected List<PodcastFeedRecyclerItem> LastSetFeedItems;

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

        private void SetupResources()
        {
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.main_activity_title)).Returns("Mocked Title");
            A.CallTo(() => MockResourceProvider.GetQuantityString(Resource.Plurals.feed_list_heading, A<int>.Ignored))
                                   .ReturnsLazily((int id, int number) => "feed count == " + number.ToString());
        }

        [SetUp]
        public void Setup()
        {
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
                MockPlaylistGenerator
            );
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.SetCacheRoot += SetCacheRoot;
            ViewModel.Observables.SetFeedItems += SetFeedItems;
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel.Observables.Title -= SetTitle;
            ViewModel.Observables.SetCacheRoot -= SetCacheRoot;
            ViewModel.Observables.SetFeedItems -= SetFeedItems;
        }

        private void SetFeedItems(object sender, Tuple<string, List<PodcastFeedRecyclerItem>> items)
        {
            (LastSetFeedHeading, LastSetFeedItems) = items;
        }

        private void SetCacheRoot(object sender, string root)
        {
            LastSetCacheRoot = root;
        }

        private void SetTitle(object sender, string title)
        {
            LastSetTitle = title;
        }
    }
}
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
        protected Application MockApplication;
        protected ILogger MockLogger;
        protected IResourceProvider MockResourceProvider;
        protected IFileSystemHelper MockFileSystemHelper;
        protected IApplicationControlFileProvider MockApplicationControlFileProvider;
        protected ICrashReporter MockCrashReporter;
        protected IAnalyticsEngine MockAnalyticsEngine;
        protected IGenerator MockPlaylistGenerator;
        protected IReadWriteControlFile MockControlFile;

        // reals
        protected IByteConverter ByteConverter = new ByteConverter();

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

        [SetUp]
        public void Setup()
        {
            MockApplication = A.Fake<Application>();
            MockLogger = A.Fake<ILogger>();
            MockResourceProvider = A.Fake<IResourceProvider>();
            MockFileSystemHelper = A.Fake<IFileSystemHelper>();
            MockApplicationControlFileProvider = A.Fake<IApplicationControlFileProvider>();
            MockCrashReporter = A.Fake<ICrashReporter>();
            MockAnalyticsEngine = A.Fake<IAnalyticsEngine>();
            MockPlaylistGenerator = A.Fake<IGenerator>();

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
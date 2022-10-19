using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Download
{

    [TestFixture]
    public class DownloadViewModel_FindEpisodesToDownload : DownloadViewModelBase
    {
        [Test]
        public void FindEpisodesToDownload_HandlesNoControlFile()
        {
            // arrange
            A.CallTo(() => MockApplicationControlFileProvider.GetApplicationConfiguration()).Returns(null);
            ViewModel.Initialise();

            // act
            ViewModel.FindEpisodesToDownload(null);

            // assert
            A.CallTo(() => MockLogger.Warning(A<ILogger.MessageGenerator>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void FindEpisodesToDownload_HandlesNoNetwork()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            ViewModel.Initialise();
            A.CallTo(() => MockNetworkHelper.ActiveNetworkType).Returns(INetworkHelper.NetworkType.None);

            // act
            ViewModel.FindEpisodesToDownload(null);

            // assert
            Assert.AreEqual("No network", ObservedResults.LastSetEmptyText);
        }

        [Test]
        public void FindEpisodesToDownload_UpdatesProgress()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();

            // act
            ViewModel.FindEpisodesToDownload(null);

            // assert
            Assert.AreEqual(2, ObservedResults.StartProgress[0], "total number of feeds");
            Assert.AreEqual(1, ObservedResults.UpdateProgress[0], "updated to 1");
            Assert.AreEqual(2, ObservedResults.UpdateProgress[1], "updated to 2");
            Assert.AreEqual(1, ObservedResults.EndProgressCount, "ended once");
        }

        [Test]
        public void FindEpisodesToDownload_Adds()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();

            // act
            ViewModel.FindEpisodesToDownload(null);

            // assert
            Assert.AreEqual(5, ObservedResults.LastDownloadItems.Count, "total number episodes found");
            Assert.AreEqual(EPISODE_1_ID, ObservedResults.LastDownloadItems[0].SyncItem.Id, "episode 1 id");
            Assert.AreEqual(EPISODE_1_TITLE, ObservedResults.LastDownloadItems[0].SyncItem.EpisodeTitle, "episode 1 title");
            Assert.AreEqual(EPISODE_2_ID, ObservedResults.LastDownloadItems[1].SyncItem.Id, "episode 2 id");
            Assert.AreEqual(EPISODE_2_TITLE, ObservedResults.LastDownloadItems[1].SyncItem.EpisodeTitle, "episode 2 title");
            Assert.AreEqual(EPISODE_3_ID, ObservedResults.LastDownloadItems[2].SyncItem.Id, "episode 3 id");
            Assert.AreEqual(EPISODE_3_TITLE, ObservedResults.LastDownloadItems[2].SyncItem.EpisodeTitle, "episode 3 title");
            Assert.AreEqual(EPISODE_4_ID, ObservedResults.LastDownloadItems[3].SyncItem.Id, "episode 4 id");
            Assert.AreEqual(EPISODE_4_TITLE, ObservedResults.LastDownloadItems[3].SyncItem.EpisodeTitle, "episode 4 title");
            Assert.AreEqual(EPISODE_5_ID, ObservedResults.LastDownloadItems[4].SyncItem.Id, "episode 5 id");
            Assert.AreEqual(EPISODE_5_TITLE, ObservedResults.LastDownloadItems[4].SyncItem.EpisodeTitle, "episode 5 title");
        }

        [Test]
        public void FindEpisodesToDownload_Analytics()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();

            // act
            ViewModel.FindEpisodesToDownload(null);

            // assert
            A.CallTo(() => MockAnalyticsEngine.DownloadFeedEvent(2)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => MockAnalyticsEngine.DownloadFeedEvent(3)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void FindEpisodesToDownload_SetsTitle()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();

            // act
            ViewModel.FindEpisodesToDownload(null);

            // assert
            Assert.AreEqual("download episodes count == 5", ObservedResults.LastSetTitle);
        }

        [Test]
        public void FindEpisodesToDownload_DoesNotRunTwice()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();
            ViewModel.FindEpisodesToDownload(null);
            ResetObservedResults();

            // act
            ViewModel.FindEpisodesToDownload(null);

            // assert
            // we only use the finder once - for each episode
            A.CallTo(MockPodcastEpisodeFinder).MustHaveHappened(2, Times.Exactly);
            Assert.AreEqual(0, ObservedResults.StartProgressCount, "never started");
            Assert.AreEqual(0, ObservedResults.UpdateProgressCount, "never updated");
            Assert.AreEqual(0, ObservedResults.EndProgressCount, "never ended");

            // however we do reinitialise the UI
            Assert.AreEqual(5, ObservedResults.LastDownloadItems.Count, "total number episodes found");
            Assert.AreEqual("download episodes count == 5", ObservedResults.LastSetTitle);
        }
    }
}
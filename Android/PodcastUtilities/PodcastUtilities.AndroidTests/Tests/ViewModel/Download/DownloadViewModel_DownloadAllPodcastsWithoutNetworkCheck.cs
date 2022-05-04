using NUnit.Framework;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Download
{
    [TestFixture]
    public class DownloadViewModel_DownloadAllPodcastsWithoutNetworkCheck : DownloadViewModelBase
    {
        [Test]
        public async void DownloadAllPodcastsWithoutNetworkCheck_HandlesNoDownloads()
        {
            // arrange
            ViewModel.Initialise();

            // act
            await ViewModel.DownloadAllPodcastsWithoutNetworkCheck();

            // assert
            Assert.AreEqual("No downloads", ObservedResults.LastDisplayMessage);
        }

        [Test]
        public async void DownloadAllPodcastsWithoutNetworkCheck_StartsAndStops()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();
            ViewModel.FindEpisodesToDownload();

            // act
            await ViewModel.DownloadAllPodcastsWithoutNetworkCheck();

            // assert
            Assert.AreEqual(1, ObservedResults.StartDownloadingCount, "start count");
            Assert.AreEqual(1, ObservedResults.EndDownloadingCount, "end count");
        }
    }
}
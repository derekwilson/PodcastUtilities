using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Settings;
using PodcastUtilities.AndroidLogic.Utilities;
using System.Threading.Tasks;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Download
{
    [TestFixture]
    public class DownloadViewModel_DownloadAllPodcastsWithNetworkCheck : DownloadViewModelBase
    {
        [Test]
        public void DownloadAllPodcastsWithNetworkCheck_HandlesNoNetwork()
        {
            // arrange
            A.CallTo(() => MockNetworkHelper.ActiveNetworkType).Returns(INetworkHelper.NetworkType.None);
            ViewModel.Initialise();

            // act
            ViewModel.DownloadAllPodcastsWithNetworkCheck();

            // assert
            Assert.AreEqual("No network", ObservedResults.LastDisplayMessage);
            Assert.IsNull(ObservedResults.LastCellularPromptTitle);
        }

        [Test]

        public void DownloadAllPodcastsWithNetworkCheck_HandlesNoWifi()
        {
            // arrange
            A.CallTo(() => MockNetworkHelper.ActiveNetworkType).Returns(INetworkHelper.NetworkType.Cellular);
            A.CallTo(() => MockUserSettings.DownloadNetworkNeeded).Returns(IUserSettings.DownloadNetworkType.WIFI);
            ViewModel.Initialise();

            // act
            ViewModel.DownloadAllPodcastsWithNetworkCheck();

            // assert
            Assert.AreEqual("dialog title", ObservedResults.LastCellularPromptTitle);
            Assert.AreEqual("cellular prompt", ObservedResults.LastCellularPromptBody);
            Assert.AreEqual("cellular ok", ObservedResults.LastCellularPromptOk);
            Assert.AreEqual("cellular cancel", ObservedResults.LastCellularPromptCancel);
        }

        [Test]
        public async void DownloadAllPodcastsWithNetworkCheck_Continues()
        {
            // arrange
            A.CallTo(() => MockNetworkHelper.ActiveNetworkType).Returns(INetworkHelper.NetworkType.Wifi);
            A.CallTo(() => MockUserSettings.DownloadNetworkNeeded).Returns(IUserSettings.DownloadNetworkType.WIFI);
            ViewModel.Initialise();

            // act
            await ViewModel.DownloadAllPodcastsWithNetworkCheck();

            // assert
            Assert.AreEqual("No downloads", ObservedResults.LastDisplayMessage);
            Assert.IsNull(ObservedResults.LastCellularPromptTitle);
        }

        [Test]
        public async void DownloadAllPodcastsWithNetworkCheck_ContinuesAndDownloads()
        {
            // arrange
            A.CallTo(() => MockNetworkHelper.ActiveNetworkType).Returns(INetworkHelper.NetworkType.Wifi);
            A.CallTo(() => MockUserSettings.DownloadNetworkNeeded).Returns(IUserSettings.DownloadNetworkType.WIFI);
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();
            ViewModel.FindEpisodesToDownload();

            // act
            await ViewModel.DownloadAllPodcastsWithNetworkCheck();

            // assert
            Assert.AreEqual(1, ObservedResults.StartDownloadingCount, "start count");
            Assert.AreEqual(1, ObservedResults.EndDownloadingCount, "end count");
            Assert.AreEqual("downloads complete", ObservedResults.LastEndDownloadingMessage);
        }

    }
}
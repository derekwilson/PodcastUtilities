using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Utilities;

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
        }
    }
}
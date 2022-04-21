using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidTests.Helpers;

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
            ViewModel.FindEpisodesToDownload();

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
            ViewModel.FindEpisodesToDownload();

            // assert
            Assert.AreEqual("No network", ObservedResults.LastSetEmptyText);
        }

        [Test]
        public void FindEpisodesToDownload_Adds()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();

            // act
            ViewModel.FindEpisodesToDownload();

            // assert
            Assert.AreEqual(2, ObservedResults.StartProgress[0], "total number of feeds");
            Assert.AreEqual(1, ObservedResults.UpdateProgress[0], "upodated to 1");
            Assert.AreEqual(2, ObservedResults.UpdateProgress[1], "updated to 2");
            Assert.AreEqual(1, ObservedResults.EndProgressCount, "ended once");
            Assert.AreEqual(5, ObservedResults.LastDownloadItems.Count, "total number episodes found");
        }

    }
}
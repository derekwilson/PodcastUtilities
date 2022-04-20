using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;

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
    }
}
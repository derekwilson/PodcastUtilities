using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests
{
    public class WhenCancellingAStartedDownload : WhenTestingTheDownloaderCompletedMechanism
    {
        protected override void When()
        {
            _downloader.Cancel();
        }

        [Test]
        public void ItShouldCancelTheClient()
        {
            _webClient.AssertWasCalled(client => client.CancelAsync());
        }
    }
}
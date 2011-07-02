using NUnit.Framework;
using PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests.WebClientEvent;
using PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests.WebClientEvent.DownloadFileCompleted;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests.Cancel
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
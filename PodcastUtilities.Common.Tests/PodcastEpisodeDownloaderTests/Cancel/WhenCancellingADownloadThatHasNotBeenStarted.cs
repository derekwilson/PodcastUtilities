using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests.Cancel
{
    public class WhenCancellingADownloadThatHasNotBeenStarted : WhenTestingTheDownloader
    {
        protected override void When()
        {
            _downloader.Cancel();
        }

        [Test]
        public void ItShouldComplete()
        {
            Assert.That(_downloader.IsComplete(), Is.True);
        }

    }
}
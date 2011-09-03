using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Feeds.EpisodeDownloaderTests.Start
{
    public class WhenStartingTheDownloader : WhenTestingTheDownloader
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _downloader.SyncItem = _syncItem;
        }

        protected override void When()
        {
            _downloader.Start(null);
        }

        [Test]
        public void ItShouldStart()
        {
            Assert.That(_downloader.IsStarted(), Is.True);
            Assert.That(_downloader.IsComplete(), Is.False);
        }

        [Test]
        public void ItShouldSetTheTitle()
        {
            Assert.That(_downloader.GetName(), Is.EqualTo("title"));
        }

        [Test]
        public void ItShouldDownloadTheFile()
        {
            _webClient.AssertWasCalled(client => client.DownloadFileAsync(_syncItem.EpisodeUrl, "c:\\folder\\file.partial", _syncItem));
        }
    }
}
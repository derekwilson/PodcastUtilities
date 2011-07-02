using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests.WebClientEvent.DownloadProgressChanged
{
    public class WhenTheProgressChanges : WhenTestingTheDownloaderProgressChangedMechanism
    {
        protected override void CreateData()
        {
            base.CreateData();
            _bytesReceived = 111;
            _totalBytes = 222;
            _percentage = 50;
        }

        protected override void When()
        {
            _webClient.Raise(client => client.DownloadProgressChanged += null, this, _progressEventArgs);
        }

        [Test]
        public void ItShouldNotComplete()
        {
            Assert.That(_downloader.IsStarted(), Is.True);
            Assert.That(_downloader.IsComplete(), Is.False);
        }

        [Test]
        public void ItShouldReportProgress()
        {
            Assert.That(_statusUpdateArgs.Exception, Is.Null);
            Assert.That(_statusUpdateArgs.MessageLevel, Is.EqualTo(StatusUpdateEventArgs.Level.Progress));
            Assert.That(_statusUpdateArgs.Message, Is.StringContaining("50"));
        }
    }
}
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests.WebClientEvent.DownloadProgressChanged
{
    public class WhenTheProgressChanges : WhenTestingTheDownloaderProgressChangedMechanism
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _progressEventArgs = new DownloadProgressEventArgs()
            {
                BytesReceived = 11,
                ProgressPercentage = 50,
                TotalBytesToReceive = 22,
                UserState = _syncItem
            };
        }

        protected override void When()
        {
            _webClient.Raise(client => client.ProgressUpdate += null, this, _progressEventArgs);
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
            Assert.That(_statusUpdateArgs.State, Is.SameAs(_progressEventArgs));
        }
    }
}
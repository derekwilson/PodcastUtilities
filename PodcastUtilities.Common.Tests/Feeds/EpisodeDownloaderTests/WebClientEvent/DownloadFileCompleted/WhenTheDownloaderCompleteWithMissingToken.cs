using System.ComponentModel;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Feeds.EpisodeDownloaderTests.WebClientEvent.DownloadFileCompleted
{
    public class WhenTheDownloaderCompleteWithMissingToken : WhenTestingTheDownloaderCompletedMechanism
    {
        protected override void When()
        {
            _webClient.Raise(client => client.DownloadFileCompleted += null, this, new AsyncCompletedEventArgs(null, false, null));
        }

        [Test]
        public void ItShouldComplete()
        {
            Assert.That(_downloader.IsStarted(), Is.True);
            Assert.That(_downloader.IsComplete(), Is.True);
        }

        [Test]
        public void ItShouldSendTheCorrectStatus()
        {
            Assert.That(_statusUpdateArgs.Exception, Is.Null);
            Assert.That(_statusUpdateArgs.MessageLevel, Is.EqualTo(StatusUpdateEventArgs.Level.Error));
            Assert.That(_statusUpdateArgs.Message, Is.StringContaining("Missing token"));
        }

        [Test]
        public void ItShouldTidyUp()
        {
            _webClient.AssertWasCalled(client => client.Dispose());
        }
    }
}
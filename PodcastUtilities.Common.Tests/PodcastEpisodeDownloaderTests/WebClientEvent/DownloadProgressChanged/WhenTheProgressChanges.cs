using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests.WebClientEvent.DownloadProgressChanged
{
    public class WhenTheProgressChanges : WhenTestingTheDownloaderProgressChangedMechanism
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _progressEventArgs = new ProgressEventArgs()
            {
                ItemsProcessed = 11,
                ProgressPercentage = 50,
                TotalItemsToProcess = 22,
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
            Assert.That(_progressUpdateArgs.ItemsProcessed, Is.EqualTo(11));
            Assert.That(_progressUpdateArgs.ProgressPercentage, Is.EqualTo(50));
            Assert.That(_progressUpdateArgs.TotalItemsToProcess, Is.EqualTo(22));
            Assert.That(_progressUpdateArgs.UserState, Is.SameAs(_syncItem));
        }
    }
}
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests.WebClientEvent.DownloadProgressChanged
{
    public class WhenTheProgressHasNotChanged : WhenTestingTheDownloaderProgressChangedMechanism
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
            _webClient.Raise(client => client.ProgressUpdate += null, this, _progressEventArgs);
            // we are not interested in the first update
            _statusUpdateArgs = null;
            // bytes has changed but not percentage
            _progressEventArgs.ItemsProcessed = 12;
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
        public void ItShouldNotReportProgress()
        {
            Assert.That(_statusUpdateArgs, Is.Null);
        }
    }
}
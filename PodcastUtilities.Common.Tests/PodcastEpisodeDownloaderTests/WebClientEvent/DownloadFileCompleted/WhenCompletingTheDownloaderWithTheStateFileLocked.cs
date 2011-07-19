using System.ComponentModel;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests.WebClientEvent.DownloadFileCompleted
{
    public class WhenCompletingTheDownloaderWithTheStateFileLocked : WhenTestingTheDownloaderCompletedMechanism
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _downloader.SleepTimeOnRetryInSeconds = 0;
        }

        protected override void SetupStubs()
        {
            base.SetupStubs();
            _state.Stub(s => s.SaveState(_downloadFolder)).Repeat.Once().Throw(new System.IO.IOException());
        }

        protected override void When()
        {
            _webClient.Raise(client => client.DownloadFileCompleted += null, this,
                             new AsyncCompletedEventArgs(null, false, _syncItem));
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
            Assert.That(_statusUpdateArgs.MessageLevel, Is.EqualTo(StatusUpdateEventArgs.Level.Status));
            Assert.That(_statusUpdateArgs.Message, Is.StringContaining("Completed"));
        }

        [Test]
        public void ItShouldTidyUp()
        {
            _webClient.AssertWasCalled(client => client.Dispose());
        }

        [Test]
        public void ItShouldRenameTheDownload()
        {
            _fileUtilities.AssertWasCalled(utils => utils.FileRename("c:\\folder\\file.partial", _syncItem.DestinationPath, true));
        }

        [Test]
        public void ItShouldUpdateTheState()
        {
            _state.AssertWasCalled(state => state.DownloadHighTide = _published, options => options.Repeat.Twice());
            _state.AssertWasCalled(state => state.SaveState(_downloadFolder), options => options.Repeat.Twice());
        }
    }
}
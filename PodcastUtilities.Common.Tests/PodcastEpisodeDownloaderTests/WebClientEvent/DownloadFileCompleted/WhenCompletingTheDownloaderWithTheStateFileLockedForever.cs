using System;
using System.ComponentModel;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests.WebClientEvent.DownloadFileCompleted
{
    public class WhenCompletingTheDownloaderWithTheStateFileLockedForever : WhenTestingTheDownloaderCompletedMechanism
    {
        private Exception _thrownException;

        protected override void GivenThat()
        {
            base.GivenThat();
            _downloader.SleepTimeOnRetryInSeconds = 0;
        }

        protected override void SetupStubs()
        {
            base.SetupStubs();
            _state.Stub(s => s.SaveState(_downloadFolder)).Throw(new System.IO.IOException());
        }

        protected override void When()
        {
            try
            {
                _webClient.Raise(client => client.DownloadFileCompleted += null, this,
                                 new AsyncCompletedEventArgs(null, false, _syncItem));
            }
            catch (Exception e)
            {
                _thrownException = e;
            }
        }

        [Test]
        public void ItShouldThrow()
        {
            Assert.IsInstanceOf(typeof(System.IO.IOException),_thrownException);
        }
    }
}
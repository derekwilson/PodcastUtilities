using System;
using System.ComponentModel;
using NUnit.Framework;
using PodcastUtilities.Common.Exceptions;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests.Start
{
    public class WhenStartingTheDownloaderAfterItHasCompleted : WhenTestingTheDownloader
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _downloader.SyncItem = _syncItem;
            _downloader.Start(null);
            _webClient.Raise(client => client.DownloadFileCompleted += null, this,
                             new AsyncCompletedEventArgs(null, false, _syncItem));
        }

        protected override void When()
        {
            _exception = null;
            try
            {
                _downloader.Start(null);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [Test]
        public void ItShouldThrow()
        {
            Assert.IsInstanceOf(typeof(DownloaderException), _exception);
        }
    }
}
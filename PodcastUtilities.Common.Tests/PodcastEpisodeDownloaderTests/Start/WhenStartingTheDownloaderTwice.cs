using System;
using NUnit.Framework;
using PodcastUtilities.Common.Exceptions;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests.Start
{
    public class WhenStartingTheDownloaderTwice : WhenTestingTheDownloader
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _downloader.SyncItem = _syncItem;
            _downloader.Start(null);
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
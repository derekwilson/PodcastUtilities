using System;
using NUnit.Framework;
using PodcastUtilities.Common.Exceptions;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests.Start
{
    public class WhenStartingTheDownloaderWithoutASyncItem : WhenTestingTheDownloader
    {
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
            Assert.IsInstanceOf(typeof(DownloaderException),_exception);
        }
    }
}
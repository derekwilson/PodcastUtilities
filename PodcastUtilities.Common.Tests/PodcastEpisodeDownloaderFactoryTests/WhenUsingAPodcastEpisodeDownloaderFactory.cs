using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderFactoryTests
{
    public class WhenUsingAPodcastEpisodeDownloaderFactory
                : WhenTestingBehaviour
    {
        private IPodcastEpisodeDownloaderFactory _factory;
        private IWebClientFactory _webClientFactory;
        private IPodcastEpisodeDownloader _downloader;

        protected override void GivenThat()
        {
            base.GivenThat();
            _webClientFactory = new WebClientFactory();
            _factory = new PodcastEpisodeDownloaderFactory(_webClientFactory);
        }

        protected override void When()
        {
            _downloader = _factory.CreateDownloader();
        }

        [Test]
        public void ItShouldReturnADownloader()
        {
            Assert.IsInstanceOf(typeof(IPodcastEpisodeDownloader), _downloader);
        }
    }
}

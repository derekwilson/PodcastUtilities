using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderFactoryTests
{
    public class WhenUsingAPodcastEpisodeDownloaderFactory
                : WhenTestingBehaviour
    {
        private IPodcastEpisodeDownloaderFactory _factory;
        private IDirectoryInfoProvider _directoryInfoProvider;
        private IFileUtilities _fileUtilities;
        private IWebClientFactory _webClientFactory;
        private IPodcastEpisodeDownloader _downloader;

        protected override void GivenThat()
        {
            base.GivenThat();
            _webClientFactory = new WebClientFactory();
            _directoryInfoProvider = GenerateMock<IDirectoryInfoProvider>();
            _fileUtilities = GenerateMock<IFileUtilities>();
            _factory = new PodcastEpisodeDownloaderFactory(_webClientFactory, _directoryInfoProvider,_fileUtilities);
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

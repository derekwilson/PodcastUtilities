using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PodcastUtilities.Common.Feeds;
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
        private IStateProvider _stateProvider;

        protected override void GivenThat()
        {
            base.GivenThat();
            _stateProvider = GenerateMock<IStateProvider>();
            _webClientFactory = new WebClientFactory();
            _directoryInfoProvider = GenerateMock<IDirectoryInfoProvider>();
            _fileUtilities = GenerateMock<IFileUtilities>();
            _factory = new PodcastEpisodeDownloaderFactory(_webClientFactory, _directoryInfoProvider,_fileUtilities,_stateProvider);
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

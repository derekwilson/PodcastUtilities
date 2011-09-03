using NUnit.Framework;
using PodcastUtilities.Common.Feeds;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Tests.Feeds.EpisodeDownloaderFactoryTests
{
    public class WhenUsingAnEpisodeDownloaderFactory
                : WhenTestingBehaviour
    {
        private IEpisodeDownloaderFactory _factory;
        private IDirectoryInfoProvider _directoryInfoProvider;
        private IFileUtilities _fileUtilities;
        private IWebClientFactory _webClientFactory;
        private IEpisodeDownloader _downloader;
        private IStateProvider _stateProvider;

        protected override void GivenThat()
        {
            base.GivenThat();
            _stateProvider = GenerateMock<IStateProvider>();
            _webClientFactory = new WebClientFactory();
            _directoryInfoProvider = GenerateMock<IDirectoryInfoProvider>();
            _fileUtilities = GenerateMock<IFileUtilities>();
            _factory = new EpisodeDownloaderFactory(_webClientFactory, _directoryInfoProvider,_fileUtilities,_stateProvider);
        }

        protected override void When()
        {
            _downloader = _factory.CreateDownloader();
        }

        [Test]
        public void ItShouldReturnADownloader()
        {
            Assert.IsInstanceOf(typeof(IEpisodeDownloader), _downloader);
        }
    }
}

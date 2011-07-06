using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// factory for a downloader task
    /// </summary>
    public class PodcastEpisodeDownloaderFactory : IPodcastEpisodeDownloaderFactory
    {
        private readonly IWebClientFactory _webClientFactory;
        private readonly IDirectoryInfoProvider _directoryInfoProvider;
        private readonly IFileUtilities _fileUtilities;

        /// <summary>
        /// construct the factory
        /// </summary>
        public PodcastEpisodeDownloaderFactory(IWebClientFactory webClientFactory, IDirectoryInfoProvider directoryInfoProvider, IFileUtilities fileUtilities)
        {
            _webClientFactory = webClientFactory;
            _fileUtilities = fileUtilities;
            _directoryInfoProvider = directoryInfoProvider;
        }

        /// <summary>
        /// create an episode downloader task
        /// </summary>
        /// <returns></returns>
        public IPodcastEpisodeDownloader CreateDownloader()
        {
            return new PodcastEpisodeDownloader(_webClientFactory,_directoryInfoProvider,_fileUtilities);
        }
    }
}
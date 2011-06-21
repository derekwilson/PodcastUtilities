namespace PodcastUtilities.Common
{
    /// <summary>
    /// factory for a downloader task
    /// </summary>
    public class PodcastEpisodeDownloaderFactory : IPodcastEpisodeDownloaderFactory
    {
        private IWebClientFactory _webClientFactory;

        /// <summary>
        /// construct the factory
        /// </summary>
        public PodcastEpisodeDownloaderFactory(IWebClientFactory webClientFactory)
        {
            _webClientFactory = webClientFactory;
        }

        /// <summary>
        /// create an episode downloader task
        /// </summary>
        /// <returns></returns>
        public IPodcastEpisodeDownloader CreateDownloader()
        {
            return new PodcastEpisodeDownloader(_webClientFactory);
        }
    }
}
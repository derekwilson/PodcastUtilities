namespace PodcastUtilities.Common
{
    /// <summary>
    /// factory for a downloader task
    /// </summary>
    public interface IPodcastEpisodeDownloaderFactory
    {
        /// <summary>
        /// create an episode downloader task
        /// </summary>
        /// <returns></returns>
        IPodcastEpisodeDownloader CreateDownloader();
    }
}
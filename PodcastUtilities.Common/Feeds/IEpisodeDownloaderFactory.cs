namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// factory for a downloader task
    /// </summary>
    public interface IEpisodeDownloaderFactory
    {
        /// <summary>
        /// create an episode downloader task
        /// </summary>
        /// <returns></returns>
        IEpisodeDownloader CreateDownloader();
    }
}
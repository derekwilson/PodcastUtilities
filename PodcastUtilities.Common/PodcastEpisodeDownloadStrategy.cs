namespace PodcastUtilities.Common
{
    /// <summary>
    /// how to download episodes
    /// </summary>
    public enum PodcastEpisodeDownloadStrategy
    {
        /// <summary>
        /// get all episodes
        /// </summary>
        All,
        /// <summary>
        /// only get episodes newer than the latest download
        /// </summary>
        HighTide,
        /// <summary>
        /// only get the latest episode
        /// </summary>
        Latest
    }
}
namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// the properties of the global section that are used to fill in missing values in podcasts and feeds
    /// </summary>
    public interface IControlFileGlobalDefaults
    {
        /// <summary>
        /// the global default for feeds
        /// </summary>
        int DefaultDeleteDownloadsDaysOld { get; }

        /// <summary>
        /// the global default for feeds
        /// </summary>
        PodcastEpisodeDownloadStrategy DefaultDownloadStrategy { get; }

        /// <summary>
        /// the global default for feeds
        /// </summary>
        PodcastFeedFormat DefaultFeedFormat { get; }

        /// <summary>
        /// the global default for feeds
        /// </summary>
        int DefaultMaximumDaysOld { get; }

        /// <summary>
        /// the global default for feeds
        /// </summary>
        PodcastEpisodeNamingStyle DefaultNamingStyle { get; }
    }
}
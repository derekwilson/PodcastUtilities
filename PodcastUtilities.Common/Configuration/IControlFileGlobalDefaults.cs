using System.Diagnostics.CodeAnalysis;
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
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        int GetDefaultDeleteDownloadsDaysOld();

        /// <summary>
        /// the global default for feeds
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        PodcastEpisodeDownloadStrategy GetDefaultDownloadStrategy();

        /// <summary>
        /// the global default for feeds
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        PodcastFeedFormat GetDefaultFeedFormat();

        /// <summary>
        /// the global default for feeds
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        int GetDefaultMaximumDaysOld();

        /// <summary>
        /// the global default for feeds
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        PodcastEpisodeNamingStyle GetDefaultNamingStyle();

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        int GetDefaultNumberOfFiles();

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetDefaultFilePattern();

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        bool GetDefaultAscendingSort();

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        PodcastFileSortField GetDefaultSortField();
    }
}
namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// the properties of the global section that are used to fill in missing values in podcasts and feeds
    /// </summary>
    public interface IReadWriteControlFileGlobalDefaults : IControlFileGlobalDefaults
    {
        /// <summary>
        /// the global default for feeds
        /// </summary>
        void SetDefaultDeleteDownloadsDaysOld(int deleteDaysOld);

        /// <summary>
        /// the global default for feeds
        /// </summary>
        void SetDefaultDownloadStrategy(PodcastEpisodeDownloadStrategy strategy);

        /// <summary>
        /// the global default for feeds
        /// </summary>
        void SetDefaultFeedFormat(PodcastFeedFormat format);

        /// <summary>
        /// the global default for feeds
        /// </summary>
        void SetDefaultMaximumDaysOld(int maximumDaysOld);

        /// <summary>
        /// the global default for feeds
        /// </summary>
        void SetDefaultNamingStyle(PodcastEpisodeNamingStyle namingStyle);

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        void SetDefaultNumberOfFiles(int numberOfFiles);

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        void SetDefaultFilePattern(string pattern);

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        void SetDefaultAscendingSort(bool ascendingSort);

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        void SetDefaultSortField(PodcastFileSortField sortField);

        /// <summary>
        /// the global default for post download command
        /// </summary>
        void SetDefaultPostDownloadCommand(string command);

        /// <summary>
        /// the global default for post download command args
        /// </summary>
        void SetDefaultPostDownloadArguments(string command);

        /// <summary>
        /// the global default for post download command cwd
        /// </summary>
        void SetDefaultPostDownloadWorkingDirectory(string command);

    }
}
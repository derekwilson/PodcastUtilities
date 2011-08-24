namespace PodcastUtilities.Common
{
    ///<summary>
    /// Supplies default values when creating new podcasts
    ///</summary>
    public interface IPodcastDefaultsProvider
    {
        ///<summary>
        /// Default file pattern
        ///</summary>
        string Pattern { get; }
        ///<summary>
        /// Default sort field
        ///</summary>
        string SortField { get; }
        ///<summary>
        /// Default sort direction
        ///</summary>
        bool AscendingSort { get; }
        ///<summary>
        /// Default feed format
        ///</summary>
        PodcastFeedFormat FeedFormat { get; }
        ///<summary>
        /// Default naming style
        ///</summary>
        PodcastEpisodeNamingStyle EpisodeNamingStyle { get; }
        ///<summary>
        /// Default download strategy
        ///</summary>
        PodcastEpisodeDownloadStrategy EpisodeDownloadStrategy { get; }
        ///<summary>
        /// Default maximum podcast age
        ///</summary>
        int MaximumDaysOld { get; }
    }
}
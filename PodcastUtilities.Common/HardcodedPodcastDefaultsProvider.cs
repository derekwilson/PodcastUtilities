namespace PodcastUtilities.Common
{
    ///<summary>
    /// Simple, hard-coded podcast defaults
    ///</summary>
    public class HardcodedPodcastDefaultsProvider : IPodcastDefaultsProvider
    {
        ///<summary>
        /// Default file pattern
        ///</summary>
        public string Pattern
        {
            get { return "*.mp3"; }
        }

        ///<summary>
        /// Default sort field
        ///</summary>
        public string SortField
        {
            get { return "name"; }
        }

        ///<summary>
        /// Default sort direction
        ///</summary>
        public bool AscendingSort
        {
            get { return true; }
        }

        ///<summary>
        /// Default feed format
        ///</summary>
        public PodcastFeedFormat FeedFormat
        {
            get { return PodcastFeedFormat.RSS; }
        }

        ///<summary>
        /// Default naming style
        ///</summary>
        public PodcastEpisodeNamingStyle EpisodeNamingStyle
        {
            get { return PodcastEpisodeNamingStyle.EpisodeTitleAndPublishDateTime; }
        }

        ///<summary>
        /// Default download strategy
        ///</summary>
        public PodcastEpisodeDownloadStrategy EpisodeDownloadStrategy
        {
            get { return PodcastEpisodeDownloadStrategy.HighTide; }
        }

        ///<summary>
        /// Default maximum podcast age
        ///</summary>
        public int MaximumDaysOld
        {
            get { return 30; }
        }
    }
}
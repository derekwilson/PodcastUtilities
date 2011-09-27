namespace PodcastUtilities.Common.Configuration
{
    ///<summary>
    /// Factory for creating podcast objects
    ///</summary>
    public class PodcastFactory : IPodcastFactory
    {
        private readonly IPodcastDefaultsProvider _podcastDefaultsProvider;

        ///<summary>
        /// Factory ctor
        ///</summary>
        ///<param name="podcastDefaultsProvider"></param>
        public PodcastFactory(IPodcastDefaultsProvider podcastDefaultsProvider)
        {
            _podcastDefaultsProvider = podcastDefaultsProvider;
        }

        ///<summary>
        /// Create a new podcast
        ///</summary>
        ///<param name="controlFile"></param>
        ///<returns></returns>
        public IPodcastInfo CreatePodcast(IControlFileGlobalDefaults controlFile)
        {
            var podcast = new PodcastInfo (controlFile)
                       {
                           Pattern = _podcastDefaultsProvider.Pattern,
                           Feed = new FeedInfo (controlFile)
                       };

            podcast.AscendingSort.Value = _podcastDefaultsProvider.AscendingSort;
            podcast.SortField.Value = _podcastDefaultsProvider.SortField;

            podcast.Feed.Format.Value = _podcastDefaultsProvider.FeedFormat;
            podcast.Feed.NamingStyle.Value = _podcastDefaultsProvider.EpisodeNamingStyle;
            podcast.Feed.DownloadStrategy.Value = _podcastDefaultsProvider.EpisodeDownloadStrategy;
            podcast.Feed.MaximumDaysOld.Value = _podcastDefaultsProvider.MaximumDaysOld;
            return podcast;
        }
    }
}
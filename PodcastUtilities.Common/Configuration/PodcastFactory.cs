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
            return new PodcastInfo (controlFile)
                       {
                           Pattern = _podcastDefaultsProvider.Pattern,
                           SortField = _podcastDefaultsProvider.SortField,
                           AscendingSort = _podcastDefaultsProvider.AscendingSort,
                           Feed = new FeedInfo (controlFile)
                                      {
                                          Format = _podcastDefaultsProvider.FeedFormat,
                                          NamingStyle = _podcastDefaultsProvider.EpisodeNamingStyle,
                                          DownloadStrategy = _podcastDefaultsProvider.EpisodeDownloadStrategy,
                                          MaximumDaysOld = _podcastDefaultsProvider.MaximumDaysOld
                                      }
                       };
        }
    }
}
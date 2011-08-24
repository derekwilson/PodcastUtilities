using System;

namespace PodcastUtilities.Common
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
        ///<returns></returns>
        public PodcastInfo CreatePodcast()
        {
            return new PodcastInfo
                       {
                           Pattern = _podcastDefaultsProvider.Pattern,
                           SortField = _podcastDefaultsProvider.SortField,
                           AscendingSort = _podcastDefaultsProvider.AscendingSort,
                           Feed = new FeedInfo
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
using System.IO;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Exceptions;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// factory to get a feed
    /// </summary>
    public class PodcastFeedFactory : IPodcastFeedFactory
    {
        /// <summary>
        /// construct a feed from the supplied data in the specified format
        /// </summary>
        /// <param name="playlistFormat">the format of the data</param>
        /// <param name="feedData">the data for the feed</param>
        /// <param name="retainCopyFileName">if present then save a copy of the feed xml before parsing - null to just load</param>
        /// <returns>a podcast feed object</returns>
        public IPodcastFeed CreatePodcastFeed(PodcastFeedFormat playlistFormat, Stream feedData, string retainCopyFileName)
        {
            switch (playlistFormat)
            {
                case PodcastFeedFormat.RSS:
                    return new PodcastFeedInRssFormat(feedData, retainCopyFileName);
                default:
                    throw new EnumOutOfRangeException("playlistFormat");
            }
        }
    }
}
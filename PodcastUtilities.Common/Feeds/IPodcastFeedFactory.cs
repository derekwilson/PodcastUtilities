using System.IO;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// factory to get a feed
    /// </summary>
    public interface IPodcastFeedFactory
    {
        /// <summary>
        /// construct a feed from the supplied data in the specified format
        /// </summary>
        /// <param name="playlistFormat">the format of the data</param>
        /// <param name="feedData">the data for the feed</param>
        /// <returns>a podcast feed object</returns>
        IPodcastFeed CreatePodcastFeed(PodcastFeedFormat playlistFormat, Stream feedData);
    }
}

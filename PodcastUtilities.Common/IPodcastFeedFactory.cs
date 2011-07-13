using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common
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
        /// <returns>a podcast feed object</returns>
        public IPodcastFeed CreatePodcastFeed(PodcastFeedFormat playlistFormat, Stream feedData)
        {
            switch (playlistFormat)
            {
                case PodcastFeedFormat.RSS:
                    return new PodcastFeedInRssFormat(feedData);
                default:
                    throw new ArgumentOutOfRangeException("playlistFormat");
            }
        }
    }
}

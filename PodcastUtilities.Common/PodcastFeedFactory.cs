using System;
using System.IO;
using PodcastUtilities.Common.Exceptions;

namespace PodcastUtilities.Common
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
        /// <returns>a podcast feed object</returns>
        public IPodcastFeed CreatePodcastFeed(PodcastFeedFormat playlistFormat, Stream feedData)
        {
            switch (playlistFormat)
            {
                case PodcastFeedFormat.RSS:
                    return new PodcastFeedInRssFormat(feedData);
                default:
                    throw new EnumOutOfRangeException("playlistFormat");
            }
        }
    }
}
using System;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// download the feed channel data
    /// </summary>
    public interface IPodcastFeedDownloader
    {
        /// <summary>
        /// populate a PodcastFeed from the supplied URI
        /// </summary>
        /// <param name="feedFormat">the format of the feed</param>
        /// <param name="address">the url to get the feed from</param>
        /// <returns>the podcast feed</returns>
        IPodcastFeed DownLoadFeed(PodcastFeedFormat feedFormat, Uri address);
    }
}
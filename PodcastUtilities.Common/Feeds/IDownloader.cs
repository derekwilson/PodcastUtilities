using System;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// download the feed channel data
    /// </summary>
    public interface IDownloader
    {
        /// <summary>
        /// populate a PodcastFeed from the supplied URI
        /// </summary>
        /// <param name="feedFormat">the format of the feed</param>
        /// <param name="address">the url to get the feed from</param>
        /// <param name="retainCopyFileName">if present then save a copy of the feed xml before parsing - null to just load</param>
        /// <returns>the podcast feed</returns>
        IPodcastFeed DownloadFeed(PodcastFeedFormat feedFormat, Uri address, string retainCopyFileName);
    }
}
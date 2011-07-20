using System;
using System.Collections.Generic;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// identify the episodes that need to be downloaded in a feed
    /// </summary>
    public interface IPodcastFeedEpisodeFinder : IStatusUpdate
    {
        /// <summary>
        /// Find episodes to download
        /// </summary>
        /// <param name="rootFolder">the root folder for all downloads</param>
        /// <param name="retryWaitTimeInseconds">time to wait if there is a file access lock</param>
        /// <param name="podcastInfo">info on the podcast to download</param>
        /// <returns>list of episodes to be downloaded for the supplied podcastInfo</returns>
        IList<IFeedSyncItem> FindEpisodesToDownload(string rootFolder, int retryWaitTimeInseconds, PodcastInfo podcastInfo);
    }
}
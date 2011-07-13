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
        /// <param name="podcastInfo">info on the podcast to download</param>
        /// <returns>list of episodes to be downloaded for the supplied podcastInfo</returns>
        IList<IFeedSyncItem> FindEpisodesToDownload(string rootFolder, PodcastInfo podcastInfo);
    }
}
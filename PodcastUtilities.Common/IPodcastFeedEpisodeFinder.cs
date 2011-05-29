using System;
using System.Collections.Generic;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// identify the episodes that need to be downloaded in a feed
    /// </summary>
    public interface IPodcastFeedEpisodeFinder
    {
        /// <summary>
        /// event that is fired whenever a file is copied of an error occurs
        /// </summary>
        event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        /// <summary>
        /// Find episodes to download
        /// </summary>
        /// <param name="rootFolder">the root folder for all downloads</param>
        /// <param name="podcastInfo">info on the podcast to download</param>
        /// <param name="episodesToDownload">list of items to download, will be added to</param>
        void FindEpisodesToDownload(string rootFolder, PodcastInfo podcastInfo, IList<FeedSyncItem> episodesToDownload);
    }
}
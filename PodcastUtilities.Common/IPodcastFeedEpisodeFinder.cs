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
        /// <param name="rootFolder">the folder into which we will download</param>
        /// <param name="feedInfo">feed information</param>
        /// <param name="episodesToDownload">list of items to download, will be added to</param>
        void FindEpisodesToDownload(string rootFolder, FeedInfo feedInfo, IList<FeedSyncItem> episodesToDownload);
    }
}
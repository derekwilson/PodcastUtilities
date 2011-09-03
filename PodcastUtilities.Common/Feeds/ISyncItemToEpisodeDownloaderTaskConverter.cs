using System;
using System.Collections.Generic;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// converts a number of IFeedSyncItem to IPodcastEpisodeDownloader tasks
    /// </summary>
    public interface ISyncItemToEpisodeDownloaderTaskConverter
    {
        /// <summary>
        /// converts a number of IFeedSyncItem to IPodcastEpisodeDownloader tasks
        /// </summary>
        /// <param name="downloadItems">the items to be downloaded</param>
        /// <param name="statusUpdate">the update mechanism for the download - can be null</param>
        /// <param name="progressUpdate">the progress mechanism for the download - can be null</param>
        /// <returns>an array of tasks suitable to be run in a task pool</returns>
        IEpisodeDownloader[] ConvertItemsToTasks(List<ISyncItem> downloadItems, EventHandler<StatusUpdateEventArgs> statusUpdate, EventHandler<ProgressEventArgs> progressUpdate);
    }
}
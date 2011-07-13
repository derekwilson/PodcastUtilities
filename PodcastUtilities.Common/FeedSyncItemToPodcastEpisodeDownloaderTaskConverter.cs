using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// converts a number of IFeedSyncItem to IPodcastEpisodeDownloader tasks
    /// </summary>
    public class FeedSyncItemToPodcastEpisodeDownloaderTaskConverter : IFeedSyncItemToPodcastEpisodeDownloaderTaskConverter
    {
        private IPodcastEpisodeDownloaderFactory _downloaderFactory;

        /// <summary>
        /// construct the converter
        /// </summary>
        public FeedSyncItemToPodcastEpisodeDownloaderTaskConverter(IPodcastEpisodeDownloaderFactory downloaderFactory)
        {
            _downloaderFactory = downloaderFactory;
        }

        /// <summary>
        /// converts a number of IFeedSyncItem to IPodcastEpisodeDownloader tasks
        /// </summary>
        /// <param name="downloadItems">the items to be downloaded</param>
        /// <param name="statusUpdate">the update mechanism for the download - can be null</param>
        /// <param name="progressUpdate">the progress mechanism for the download - can be null</param>
        /// <returns>an array of tasks suitable to be run in a task pool</returns>
        public IPodcastEpisodeDownloader[] ConvertItemsToTasks(List<IFeedSyncItem> downloadItems, EventHandler<StatusUpdateEventArgs> statusUpdate, EventHandler<ProgressEventArgs> progressUpdate)
        {
            IPodcastEpisodeDownloader[] downloadTasks = new PodcastEpisodeDownloader[downloadItems.Count];

            for (int index = 0; index < downloadItems.Count; index++)
            {
                downloadTasks[index] = _downloaderFactory.CreateDownloader();
                downloadTasks[index].SyncItem = downloadItems[index];
                downloadTasks[index].StatusUpdate += statusUpdate;
                downloadTasks[index].ProgressUpdate += progressUpdate;
            }

            return downloadTasks;
        }
    }
}

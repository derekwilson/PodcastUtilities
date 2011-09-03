using System;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// an item to be downloaded
    /// </summary>
    public class SyncItem : ISyncItem
    {
        /// <summary>
        /// date time the episode was published
        /// </summary>
        public DateTime Published { get; set; }

        /// <summary>
        /// state key
        /// </summary>
        public string StateKey { get; set; }

        /// <summary>
        /// the url to download from
        /// </summary>
        public Uri EpisodeUrl { get; set; }

        /// <summary>
        /// pathname to be downloaded to
        /// </summary>
        public string DestinationPath { get; set; }

        /// <summary>
        /// the title of the eposide
        /// </summary>
        public string EpisodeTitle { get; set; }

        /// <summary>
        /// time to wait if there is a file lock on state
        /// </summary>
        public int RetryWaitTimeInSeconds { get; set; }
    }
}
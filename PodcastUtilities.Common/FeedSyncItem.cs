using System;
using System.Security.Policy;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// an item to be downloaded
    /// </summary>
    public class FeedSyncItem
    {
        /// <summary>
        /// the url to download from
        /// </summary>
        public Uri EpisodeUrl { get; set; }

        /// <summary>
        /// pathname to be downloaded to
        /// </summary>
        public string DestinationPath { get; set; }
    }
}
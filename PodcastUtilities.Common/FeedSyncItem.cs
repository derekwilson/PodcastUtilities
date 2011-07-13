using System;
using System.Security.Policy;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// an item to be downloaded
    /// </summary>
    public class FeedSyncItem : IFeedSyncItem
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
    }
}
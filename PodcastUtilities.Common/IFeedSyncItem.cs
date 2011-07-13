using System;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// an item to be downloaded
    /// </summary>
    public interface IFeedSyncItem
    {
        /// <summary>
        /// date time the episode was published
        /// </summary>
        DateTime Published { get; set; }

        /// <summary>
        /// state key
        /// </summary>
        string StateKey { get; set; }

        /// <summary>
        /// the url to download from
        /// </summary>
        Uri EpisodeUrl { get; set; }

        /// <summary>
        /// pathname to be downloaded to
        /// </summary>
        string DestinationPath { get; set; }

        /// <summary>
        /// the title of the eposide
        /// </summary>
        string EpisodeTitle { get; set; }
    }
}
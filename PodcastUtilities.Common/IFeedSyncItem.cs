using System;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// an item to be downloaded
    /// </summary>
    public interface IFeedSyncItem
    {
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
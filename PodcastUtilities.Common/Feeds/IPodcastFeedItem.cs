using System;
using System.Diagnostics.CodeAnalysis;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// an individual episode of a pocast feed
    /// </summary>
    public interface IPodcastFeedItem
    {
        /// <summary>
        /// title of the item
        /// </summary>
        string EpisodeTitle { get; set; }

        /// <summary>
        /// address of the attachment to download
        /// </summary>
        Uri Address { get; set; }

        /// <summary>
        /// the date the episode was published
        /// </summary>
        DateTime Published { get; set; }

        /// <summary>
        /// filename to use when saving the podcast file
        /// </summary>
        string FileName { get;  }

        /// <summary>
        /// get the episode title in a form that can be used as a filename
        /// </summary>
        string TitleAsFileName { get; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// an individual episode of a pocast feed
    /// </summary>
    public interface IPodcastFeedItem
    {
        /// <summary>
        /// title of the item
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// address of the attachment to download
        /// </summary>
        Uri Address { get; set; }

        /// <summary>
        /// filename to use when saving the podcast file
        /// </summary>
        /// <returns></returns>
        string GetFilename();
    }
}

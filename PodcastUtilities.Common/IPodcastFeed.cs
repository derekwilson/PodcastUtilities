using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// a feed for a podcast
    /// </summary>
    public interface IPodcastFeed
    {
        /// <summary>
        /// the title of the podcast
        /// </summary>
        string PodcastTitle { get; }

        /// <summary>
        /// event that is fired whenever a file is copied of an error occurs
        /// </summary>
        event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        /// <summary>
        /// get the episodes of a feed
        /// </summary>
        /// <returns></returns>
        List<IPodcastFeedItem> GetFeedEpisodes();
    }
}

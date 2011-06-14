using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// a feed for a podcast
    /// </summary>
    public interface IPodcastFeed : IStatusUpdate
    {
        /// <summary>
        /// the title of the podcast
        /// </summary>
        string PodcastTitle { get; }

        /// <summary>
        /// get the episodes of a feed
        /// </summary>
        /// <returns></returns>
        IList<IPodcastFeedItem> GetFeedEpisodes();
    }
}

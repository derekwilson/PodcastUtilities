using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IList<IPodcastFeedItem> GetFeedEpisodes();
    }
}

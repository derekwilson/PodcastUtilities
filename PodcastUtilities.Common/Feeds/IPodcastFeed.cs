using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// a feed for a podcast
    /// </summary>
    public interface IPodcastFeed : IStatusUpdate
    {
        /// <summary>
        /// the title of the podcast
        /// </summary>
        string Title { get; }

        /// <summary>
        /// get the episodes of a feed
        /// </summary>
        /// <returns></returns>
        IList<IPodcastFeedItem> Episodes { get; }
    }
}

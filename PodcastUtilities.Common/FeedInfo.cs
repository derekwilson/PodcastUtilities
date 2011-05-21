using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// configuration info for a podcast feed
    /// </summary>
    public class FeedInfo
    {
        /// <summary>
        /// the address of the podcast feed
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// the format the feed is in
        /// </summary>
        public PodcastFeedFormat Format { get; set; }
    }
}

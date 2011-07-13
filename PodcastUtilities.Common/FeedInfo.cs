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
        public Uri Address { get; set; }

        /// <summary>
        /// the format the feed is in
        /// </summary>
        public PodcastFeedFormat Format { get; set; }

        /// <summary>
        /// do not download podcasts that werre published before this number of days ago
        /// </summary>
        public int MaximumDaysOld { get; set; }

        /// <summary>
        /// the naming style to use for episodes downloaded from the feed
        /// </summary>
        public PodcastEpisodeNamingStyle NamingStyle { get; set; }
    }
}

using System;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// configuration info for a podcast feed
    /// </summary>
    public class FeedInfo : IFeedInfo
    {
        private IControlFileGlobalDefaults _controlFileGlobalDefaults;

        /// <summary>
        /// construct a new feed element
        /// </summary>
        public FeedInfo(IControlFileGlobalDefaults controlFileGlobalDefaults)
        {
            _controlFileGlobalDefaults = controlFileGlobalDefaults;
        }

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

        /// <summary>
        /// the strategy to be used when downloading episodes
        /// </summary>
        public PodcastEpisodeDownloadStrategy DownloadStrategy { get; set; }

        private int? _deleteDownloadsDaysOld;

        /// <summary>
        /// number of days before we delete a download
        /// </summary>
        public int DeleteDownloadsDaysOld
        {
            get { return _deleteDownloadsDaysOld.GetValueOrDefault(_controlFileGlobalDefaults.DefaultDeleteDownloadsDaysOld); }
            set { _deleteDownloadsDaysOld = value; }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            var copy = new FeedInfo(_controlFileGlobalDefaults) {Address = Address};
            copy.DownloadStrategy = DownloadStrategy;
            copy.Format = Format;
            copy.MaximumDaysOld = MaximumDaysOld;
            copy.NamingStyle = NamingStyle;
            copy._deleteDownloadsDaysOld = _deleteDownloadsDaysOld;

            return copy;
        }
    }
}

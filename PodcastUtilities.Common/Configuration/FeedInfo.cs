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

        private PodcastFeedFormat? _format;

        /// <summary>
        /// the format the feed is in
        /// </summary>
        public PodcastFeedFormat Format
        {
            get { return _format.GetValueOrDefault(_controlFileGlobalDefaults.DefaultFeedFormat); }
            set { _format = value; }
        }

        private int? _maximumDaysOld;

        /// <summary>
        /// do not download podcasts that werre published before this number of days ago
        /// </summary>
        public int MaximumDaysOld
        {
            get { return _maximumDaysOld.GetValueOrDefault(_controlFileGlobalDefaults.DefaultMaximumDaysOld); }
            set { _maximumDaysOld = value; }
        }

        private PodcastEpisodeNamingStyle? _namingStyle;

        /// <summary>
        /// the naming style to use for episodes downloaded from the feed
        /// </summary>
        public PodcastEpisodeNamingStyle NamingStyle
        {
            get { return _namingStyle.GetValueOrDefault(_controlFileGlobalDefaults.DefaultNamingStyle); }
            set { _namingStyle = value; }
        }

        private PodcastEpisodeDownloadStrategy? _downloadStrategy;

        /// <summary>
        /// the strategy to be used when downloading episodes
        /// </summary>
        public PodcastEpisodeDownloadStrategy DownloadStrategy
        {
            get { return _downloadStrategy.GetValueOrDefault(_controlFileGlobalDefaults.DefaultDownloadStrategy); }
            set { _downloadStrategy = value; }
        }

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
        /// remove the Format, in other words revert to the global default
        /// </summary>
        public void RemoveFormat()
        {
            _format = null;
        }

        /// <summary>
        /// remove the MaximumDaysOld, in other words revert to the global default
        /// </summary>
        public void RemoveMaximumDaysOld()
        {
            _maximumDaysOld = null;
        }

        /// <summary>
        /// remove the NamingStyle, in other words revert to the global default
        /// </summary>
        public void RemoveNamingStyle()
        {
            _namingStyle = null;
        }

        /// <summary>
        /// remove the DownloadStrategy, in other words revert to the global default
        /// </summary>
        public void RemoveDownloadStrategy()
        {
            _downloadStrategy = null;
        }

        /// <summary>
        /// remove the DeleteDownloadsDaysOld, in other words revert to the global default
        /// </summary>
        public void RemoveDeleteDownloadsDaysOld()
        {
            _deleteDownloadsDaysOld = null;
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
            copy._downloadStrategy = _downloadStrategy;
            copy._format = _format;
            copy._maximumDaysOld = _maximumDaysOld;
            copy._namingStyle = _namingStyle;
            copy._deleteDownloadsDaysOld = _deleteDownloadsDaysOld;

            return copy;
        }
    }
}

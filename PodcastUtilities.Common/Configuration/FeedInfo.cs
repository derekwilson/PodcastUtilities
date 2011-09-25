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
            Format = new DefaultableItem<PodcastFeedFormat>(_controlFileGlobalDefaults.GetDefaultFeedFormat);
            MaximumDaysOld = new DefaultableItem<int>(_controlFileGlobalDefaults.GetDefaultMaximumDaysOld);
            NamingStyle = new DefaultableItem<PodcastEpisodeNamingStyle>(_controlFileGlobalDefaults.GetDefaultNamingStyle);
            DownloadStrategy = new DefaultableItem<PodcastEpisodeDownloadStrategy>(_controlFileGlobalDefaults.GetDefaultDownloadStrategy);
            DeleteDownloadsDaysOld = new DefaultableItem<int>(_controlFileGlobalDefaults.GetDefaultDeleteDownloadsDaysOld);
        }

        /// <summary>
        /// the address of the podcast feed
        /// </summary>
        public Uri Address { get; set; }

        /// <summary>
        /// the format the feed is in
        /// </summary>
        public IDefaultableItem<PodcastFeedFormat> Format { get; set; }

        /// <summary>
        /// do not download podcasts that werre published before this number of days ago
        /// </summary>
        public IDefaultableItem<int> MaximumDaysOld { get; set; }

        /// <summary>
        /// the naming style to use for episodes downloaded from the feed
        /// </summary>
        public IDefaultableItem<PodcastEpisodeNamingStyle> NamingStyle { get; set; }

        /// <summary>
        /// the strategy to be used when downloading episodes
        /// </summary>
        public IDefaultableItem<PodcastEpisodeDownloadStrategy> DownloadStrategy { get; set; }

        /// <summary>
        /// number of days before we delete a download
        /// </summary>
        public IDefaultableItem<int> DeleteDownloadsDaysOld { get; set;  }

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
            if (DownloadStrategy.IsSet)
            {
                copy.DownloadStrategy.Value = DownloadStrategy.Value;
            }
            if (Format.IsSet)
            {
                copy.Format.Value = Format.Value;
            }
            if (MaximumDaysOld.IsSet)
            {
                copy.MaximumDaysOld.Value = MaximumDaysOld.Value;
            }
            if (NamingStyle.IsSet)
            {
                copy.NamingStyle.Value = NamingStyle.Value;
            }
            if (DeleteDownloadsDaysOld.IsSet)
            {
                copy.DeleteDownloadsDaysOld.Value = DeleteDownloadsDaysOld.Value;
            }

            return copy;
        }

    }
}

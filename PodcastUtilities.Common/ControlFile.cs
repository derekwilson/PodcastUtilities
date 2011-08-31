using System;
using System.Collections.Generic;
using System.Xml;

namespace PodcastUtilities.Common
{
	/// <summary>
	/// this object represents the xml control file
	/// </summary>
    public class ControlFile : IControlFile
	{
        private const string DefaultSortField = "Name";
        private const string DefaultSortDirection = "ascending";
	    private const string DefaultFeedFormat = "rss";
	    private const string DefaultFeedEpisodeDownloadStrategy = "high_tide";
        
        private readonly XmlDocument _xmlDocument;

		private List<PodcastInfo> _podcasts;

		/// <summary>
		/// create the object and read the control file from the specified filename
		/// </summary>
		/// <param name="filename">pathname to the control file xml</param>
        public ControlFile(string filename)
		{
			_xmlDocument = new XmlDocument();

			_xmlDocument.Load(filename);

			ReadPodcasts();
		}

        /// <summary>
        /// only used for unit testing
        /// </summary>
        public ControlFile(XmlDocument document)
		{
		    _xmlDocument = document;

            ReadPodcasts();
		}

		/// <summary>
		/// pathname to the root folder to copy from when synchronising
		/// </summary>
        public string SourceRoot
		{
			get { return GetNodeText("podcasts/global/sourceRoot"); }
		}
		
		/// <summary>
		/// pathname to the destination root folder
		/// </summary>
        public string DestinationRoot
		{
			get { return GetNodeText("podcasts/global/destinationRoot"); }
		}

		/// <summary>
		/// filename and extension for the generated playlist
		/// </summary>
        public string PlaylistFileName
		{
			get { return GetNodeText("podcasts/global/playlistFilename"); }
		}

        /// <summary>
        /// the format for the generated playlist
        /// </summary>
        public PlaylistFormat PlaylistFormat
        {
            get
            {
                string format = GetNodeText("podcasts/global/playlistFormat").ToLower();
                switch (format)
                {
                    case "wpl":
                        return PlaylistFormat.WPL;
                    case "asx":
                        return PlaylistFormat.ASX;
                }
                throw new IndexOutOfRangeException(string.Format("{0} is not a valid value for the playlist format",format));
            }
        }

        /// <summary>
        /// free space in MB to leave on the destination device when syncing
        /// </summary>
        public long FreeSpaceToLeaveOnDestination
        {
            get
            {
                try
                {
                    return Convert.ToInt64(GetNodeText("podcasts/global/freeSpaceToLeaveOnDestinationMB"));
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// free space in MB to leave on the download device - when downloading
        /// </summary>
        public long FreeSpaceToLeaveOnDownload
        {
            get
            {
                try
                {
                    return Convert.ToInt64(GetNodeText("podcasts/global/freeSpaceToLeaveOnDownloadMB"));
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
		/// the configuration for the individual podcasts
		/// </summary>
        public IList<PodcastInfo> Podcasts
		{
			get { return _podcasts; }
		}

        /// <summary>
        /// maximum number of background downloads
        /// </summary>
        public int MaximumNumberOfConcurrentDownloads
        {
            get
            {
                try
                {
                    return Convert.ToInt32(GetNodeText("podcasts/global/maximumNumberOfConcurrentDownloads"));
                }
                catch
                {
                    return 5;
                }
            }
        }

        /// <summary>
        /// number of seconds to wait when trying a file conflict
        /// </summary>
        public int RetryWaitInSeconds
        {
            get
            {
                try
                {
                    return Convert.ToInt32(GetNodeText("podcasts/global/retryWaitInSeconds"));
                }
                catch
                {
                    return 10;
                }
            }
        }

        /// <summary>
		/// the field we are using to sort the podcasts on
		/// </summary>
        public string SortField
        {
            get
            {
				return GetNodeTextOrDefault("podcasts/global/sortfield", DefaultSortField);
            }
        }

        /// <summary>
        /// direction to sort in
        /// </summary>
        public string SortDirection
        {
            get
            {
                return GetNodeTextOrDefault("podcasts/global/sortdirection", DefaultSortDirection);
            }
        }

        /// <summary>
        /// global default maximum days old for feed download
        /// </summary>
        public string FeedMaximumDaysOld
        {
            get
            {
                return GetNodeTextOrDefault("podcasts/global/feed/maximumDaysOld", int.MaxValue.ToString());
            }
        }

        /// <summary>
        /// global default number of days before deleteing a download
        /// </summary>
        public string FeedDeleteDownloadsDaysOld
        {
            get
            {
                return GetNodeTextOrDefault("podcasts/global/feed/deleteDownloadsDaysOld", int.MaxValue.ToString());
            }
        }

        /// <summary>
        /// global default feed format
        /// </summary>
        public string FeedFormat
        {
            get
            {
                return GetNodeTextOrDefault("podcasts/global/feed/format", DefaultFeedFormat);
            }
        }

        /// <summary>
        /// global default for naming downloaded episodes
        /// </summary>
        protected string FeedEpisodeNamingStyle
        {
            get
            {
                return GetNodeTextOrDefault("podcasts/global/feed/namingStyle", DefaultFeedFormat);
            }
        }

        /// <summary>
        /// global default for naming downloaded episodes
        /// </summary>
        protected string FeedEpisodeDownloadStrategy
        {
            get
            {
                return GetNodeTextOrDefault("podcasts/global/feed/downloadStrategy", DefaultFeedEpisodeDownloadStrategy);
            }
        }

        private PodcastFeedFormat ReadFeedFormat(string format)
        {
            switch (format.ToLower())
            {
                case "rss":
                    return PodcastFeedFormat.RSS;
                case "atom":
                    return PodcastFeedFormat.ATOM;
            }
            throw new IndexOutOfRangeException(string.Format("{0} is not a valid value for the feed format", format));
        }

        private int ReadMaximumDaysOld(XmlNode feedNode)
        {
            if (feedNode == null)
            {
                return int.MaxValue;
            }
            try
            {
                return Convert.ToInt32(GetNodeTextOrDefault(feedNode, "maximumDaysOld", FeedMaximumDaysOld));
            }
            catch
            {
                return int.MaxValue;
            }
        }

        private int ReadDeleteDownloadsDaysOld(XmlNode feedNode)
        {
            if (feedNode == null)
            {
                return int.MaxValue;
            }
            try
            {
                var days = Convert.ToInt32(GetNodeTextOrDefault(feedNode, "deleteDownloadsDaysOld", FeedDeleteDownloadsDaysOld));
                if (days == 0)
                {
                    return int.MaxValue;
                }
                return days;
            }
            catch
            {
                return int.MaxValue;
            }
        }

        private PodcastEpisodeNamingStyle ReadFeedEpisodeNamingStyle(string format)
        {
            switch (format.ToLower())
            {
                case "pubdate_url":
                    return PodcastEpisodeNamingStyle.UrlFilenameAndPublishDateTime;
                case "pubdate_title_url":
                    return PodcastEpisodeNamingStyle.UrlFilenameFeedTitleAndPublishDateTime;
                case "pubdate_folder_title_url":
                    return PodcastEpisodeNamingStyle.UrlFilenameFeedTitleAndPublishDateTimeInFolder;
                case "etitle":
                    return PodcastEpisodeNamingStyle.EpisodeTitle;
                case "pubdate_etitle":
                    return PodcastEpisodeNamingStyle.EpisodeTitleAndPublishDateTime;
                default:
                    return PodcastEpisodeNamingStyle.UrlFilename;

            }
            throw new IndexOutOfRangeException(string.Format("{0} is not a valid value for the feed format", format));
        }

        private PodcastEpisodeDownloadStrategy ReadFeedEpisodeDownloadStrategy(string strategy)
        {
            switch (strategy.ToLower())
            {
                case "high_tide":
                    return PodcastEpisodeDownloadStrategy.HighTide;
                case "all":
                    return PodcastEpisodeDownloadStrategy.All;
                case "latest":
                    return PodcastEpisodeDownloadStrategy.Latest;
                default:
                    return PodcastEpisodeDownloadStrategy.All;

            }
            throw new IndexOutOfRangeException(string.Format("{0} is not a valid value for the feed download strategy", strategy));
        }


        private FeedInfo ReadFeedInfo(XmlNode feedNode)
        {
            if (feedNode == null)
            {
                return null;
            }
            return new FeedInfo()
                       {
                           Address = new Uri(GetNodeText(feedNode, "url")),
                           Format = ReadFeedFormat(GetNodeTextOrDefault(feedNode, "format",FeedFormat)),
                           MaximumDaysOld = ReadMaximumDaysOld(feedNode),
                           NamingStyle = ReadFeedEpisodeNamingStyle(GetNodeTextOrDefault(feedNode, "namingStyle", FeedEpisodeNamingStyle)),
                           DownloadStrategy = ReadFeedEpisodeDownloadStrategy(GetNodeTextOrDefault(feedNode, "downloadStrategy", FeedEpisodeDownloadStrategy)),
                           DeleteDownloadsDaysOld = ReadDeleteDownloadsDaysOld(feedNode)
                       };
        }

	    private void ReadPodcasts()
		{
			_podcasts = new List<PodcastInfo>();

			var podcastNodes = _xmlDocument.SelectNodes("podcasts/podcast");

			if (podcastNodes != null)
			{
				foreach (XmlNode podcastNode in podcastNodes)
				{
					var podcastInfo = new PodcastInfo
					{
                        Feed = ReadFeedInfo(podcastNode.SelectSingleNode("feed")),
                        Folder = GetNodeText(podcastNode, "folder"),
						Pattern = GetNodeText(podcastNode, "pattern"),
						MaximumNumberOfFiles = Convert.ToInt32(GetNodeTextOrDefault(podcastNode, "number", "-1")),
						SortField = GetNodeTextOrDefault(podcastNode, "sortfield", SortField),
						AscendingSort = !(GetNodeTextOrDefault(podcastNode, "sortdirection", SortDirection).ToLower().StartsWith("desc"))
					};

					_podcasts.Add(podcastInfo);
				}
			}
		}

		private string GetNodeText(string xpath)
		{
			return GetNodeText(_xmlDocument, xpath);
		}

		private string GetNodeTextOrDefault(string xpath, string defaultText)
		{
			return GetNodeTextOrDefault(_xmlDocument, xpath, defaultText);
		}

		private static string GetNodeText(XmlNode root, string xpath)
		{
			XmlNode n = root.SelectSingleNode(xpath);
			if (n == null)
			{
				throw new System.Exception("GetNodeText : Node path '" + xpath + "' not found");
			}
			return n.InnerText;
		}

		private static string GetNodeTextOrDefault(XmlNode root, string xpath, string defaultText)
		{
			XmlNode n = root.SelectSingleNode(xpath);

			return ((n != null) ? n.InnerText : defaultText);
		}
	}
}

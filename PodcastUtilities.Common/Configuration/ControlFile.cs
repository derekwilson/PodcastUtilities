using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using PodcastUtilities.Common.Exceptions;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Configuration
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
		/// <param name="fileName">pathname to the control file xml</param>
        public ControlFile(string fileName)
		{
			_xmlDocument = new XmlDocument();

			_xmlDocument.Load(fileName);

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
                string format = GetNodeText("podcasts/global/playlistFormat").ToUpperInvariant();
                switch (format)
                {
                    case "WPL":
                        return PlaylistFormat.WPL;
                    case "ASX":
                        return PlaylistFormat.ASX;
                }
                throw new ControlFileValueFormatException(string.Format(CultureInfo.InvariantCulture,"{0} is not a valid value for the playlist format", format));
            }
        }

        /// <summary>
        /// free space in MB to leave on the destination device when syncing
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public long FreeSpaceToLeaveOnDestination
        {
            get
            {
                try
                {
                    return Convert.ToInt64(GetNodeText("podcasts/global/freeSpaceToLeaveOnDestinationMB"), CultureInfo.InvariantCulture);
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
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public long FreeSpaceToLeaveOnDownload
        {
            get
            {
                try
                {
                    return Convert.ToInt64(GetNodeText("podcasts/global/freeSpaceToLeaveOnDownloadMB"), CultureInfo.InvariantCulture);
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
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public int MaximumNumberOfConcurrentDownloads
        {
            get
            {
                try
                {
                    return Convert.ToInt32(GetNodeText("podcasts/global/maximumNumberOfConcurrentDownloads"),CultureInfo.InvariantCulture);
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
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public int RetryWaitInSeconds
        {
            get
            {
                try
                {
                    return Convert.ToInt32(GetNodeText("podcasts/global/retryWaitInSeconds"), CultureInfo.InvariantCulture);
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
                return GetNodeTextOrDefault("podcasts/global/feed/maximumDaysOld", int.MaxValue.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// global default number of days before deleteing a download
        /// </summary>
        public string FeedDeleteDownloadsDaysOld
        {
            get
            {
                return GetNodeTextOrDefault("podcasts/global/feed/deleteDownloadsDaysOld", int.MaxValue.ToString(CultureInfo.InvariantCulture));
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

        private static PodcastFeedFormat ReadFeedFormat(string format)
        {
            switch (format.ToUpperInvariant())
            {
                case "RSS":
                    return PodcastFeedFormat.RSS;
                case "ATOM":
                    return PodcastFeedFormat.ATOM;
            }
            throw new ControlFileValueFormatException(string.Format(CultureInfo.InvariantCulture, "{0} is not a valid value for the feed format", format));
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private int ReadMaximumDaysOld(XmlNode feedNode)
        {
            if (feedNode == null)
            {
                return int.MaxValue;
            }
            try
            {
                return Convert.ToInt32(GetNodeTextOrDefault(feedNode, "maximumDaysOld", FeedMaximumDaysOld), CultureInfo.InvariantCulture);
            }
            catch
            {
                return int.MaxValue;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private int ReadDeleteDownloadsDaysOld(XmlNode feedNode)
        {
            if (feedNode == null)
            {
                return int.MaxValue;
            }
            try
            {
                var days = Convert.ToInt32(GetNodeTextOrDefault(feedNode, "deleteDownloadsDaysOld", FeedDeleteDownloadsDaysOld), CultureInfo.InvariantCulture);
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

        private static PodcastEpisodeNamingStyle ReadFeedEpisodeNamingStyle(string format)
        {
            switch (format.ToUpperInvariant())
            {
                case "PUBDATE_URL":
                    return PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime;
                case "PUBDATE_TITLE_URL":
                    return PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTime;
                case "PUBDATE_FOLDER_TITLE_URL":
                    return PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTimeInfolder;
                case "ETITLE":
                    return PodcastEpisodeNamingStyle.EpisodeTitle;
                case "PUBDATE_ETITLE":
                    return PodcastEpisodeNamingStyle.EpisodeTitleAndPublishDateTime;
                default:
                    return PodcastEpisodeNamingStyle.UrlFileName;

            }
        }

        private static PodcastEpisodeDownloadStrategy ReadFeedEpisodeDownloadStrategy(string strategy)
        {
            switch (strategy.ToUpperInvariant())
            {
                case "HIGH_TIDE":
                    return PodcastEpisodeDownloadStrategy.HighTide;
                case "ALL":
                    return PodcastEpisodeDownloadStrategy.All;
                case "LATEST":
                    return PodcastEpisodeDownloadStrategy.Latest;
                default:
                    return PodcastEpisodeDownloadStrategy.All;

            }
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
                        MaximumNumberOfFiles = Convert.ToInt32(GetNodeTextOrDefault(podcastNode, "number", "-1"), CultureInfo.InvariantCulture),
						SortField = GetNodeTextOrDefault(podcastNode, "sortfield", SortField),
						AscendingSort = !(GetNodeTextOrDefault(podcastNode, "sortdirection", SortDirection).ToUpperInvariant().StartsWith("DESC",StringComparison.Ordinal))
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
				throw new XmlStructureException("GetNodeText : Node path '" + xpath + "' not found");
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

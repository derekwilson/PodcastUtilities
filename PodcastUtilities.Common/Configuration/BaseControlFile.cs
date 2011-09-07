using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using PodcastUtilities.Common.Exceptions;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// base operations to work with controlfiles
    /// </summary>
    public abstract class BaseControlFile
    {
        private const string DefaultSortField = "Name";
        private const string DefaultSortDirection = "ascending";
        private const string DefaultFeedFormat = "rss";
        private const string DefaultFeedEpisodeDownloadStrategy = "high_tide";

        /// <summary>
        /// pathname to the root folder to copy from when synchronising
        /// </summary>
        private string _sourceRootBackingField;
        /// <summary>
        /// pathname to the destination root folder
        /// </summary>
        private string _destinationRootBackingField;
        /// <summary>
        /// filename and extension for the generated playlist
        /// </summary>
        private string _playlistFileNameBackingField;
        /// <summary>
        /// the format for the generated playlist
        /// </summary>
        private PlaylistFormat _playlistFormatBackingField;
        /// <summary>
        /// free space in MB to leave on the destination device when syncing
        /// </summary>
        private long _freeSpaceToLeaveOnDestinationBackingField = 0;
        /// <summary>
        /// free space in MB to leave on the download device - when downloading
        /// </summary>
        private long _freeSpaceToLeaveOnDownloadBackingField = 0;
        /// <summary>
        /// the configuration for the individual podcasts
        /// </summary>
        private IList<PodcastInfo> _podcastsBackingField;
        /// <summary>
        /// maximum number of background downloads
        /// </summary>
        private int _maximumNumberOfConcurrentDownloadsBackingField = 5;
        /// <summary>
        /// number of seconds to wait when trying a file conflict
        /// </summary>
        private int _retryWaitInSecondsBackingField = 10;

        /// <summary>
        /// the field we are using to sort the podcasts on
        /// </summary>
        private string _sortFieldBackingField;
        /// <summary>
        /// direction to sort in
        /// </summary>
        private string _sortDirectionBackingField;
        /// <summary>
        /// global default maximum days old for feed download
        /// </summary>
        private string _feedMaximumDaysOldBackingField;
        /// <summary>
        /// global default number of days before deleteing a download
        /// </summary>
        private string _feedDeleteDownloadsDaysOldBackingField;
        /// <summary>
        /// global default feed format 
        /// </summary>
        private string _feedFormatBackingField;
        /// <summary>
        /// global default for naming downloaded episodes
        /// </summary>
        private string _feedEpisodeNamingStyleBackingField;
        /// <summary>
        /// global default for naming downloaded episodes
        /// </summary>
        private string _feedEpisodeDownloadStrategyBackingField;

        /// <summary>
        /// pathname to the root folder to copy from when synchronising
        /// </summary>
        protected internal string SourceRootBackingField
        {
            get { return _sourceRootBackingField; }
            set { _sourceRootBackingField = value; }
        }

        /// <summary>
        /// pathname to the destination root folder
        /// </summary>
        protected internal string DestinationRootBackingField
        {
            get { return _destinationRootBackingField; }
            set { _destinationRootBackingField = value; }
        }

        /// <summary>
        /// filename and extension for the generated playlist
        /// </summary>
        protected internal string PlaylistFileNameBackingField
        {
            get { return _playlistFileNameBackingField; }
            set { _playlistFileNameBackingField = value; }
        }

        /// <summary>
        /// the format for the generated playlist
        /// </summary>
        protected internal PlaylistFormat PlaylistFormatBackingField
        {
            get { return _playlistFormatBackingField; }
            set { _playlistFormatBackingField = value; }
        }

        /// <summary>
        /// free space in MB to leave on the destination device when syncing
        /// </summary>
        protected internal long FreeSpaceToLeaveOnDestinationBackingField
        {
            get { return _freeSpaceToLeaveOnDestinationBackingField; }
            set { _freeSpaceToLeaveOnDestinationBackingField = value; }
        }

        /// <summary>
        /// free space in MB to leave on the download device - when downloading
        /// </summary>
        protected internal long FreeSpaceToLeaveOnDownloadBackingField
        {
            get { return _freeSpaceToLeaveOnDownloadBackingField; }
            set { _freeSpaceToLeaveOnDownloadBackingField = value; }
        }

        /// <summary>
        /// the configuration for the individual podcasts
        /// </summary>
        protected internal IList<PodcastInfo> PodcastsBackingField
        {
            get { return _podcastsBackingField; }
        }

        /// <summary>
        /// maximum number of background downloads
        /// </summary>
        protected internal int MaximumNumberOfConcurrentDownloadsBackingField
        {
            get { return _maximumNumberOfConcurrentDownloadsBackingField; }
            set { _maximumNumberOfConcurrentDownloadsBackingField = value; }
        }

        /// <summary>
        /// number of seconds to wait when trying a file conflict
        /// </summary>
        protected internal int RetryWaitInSecondsBackingField
        {
            get { return _retryWaitInSecondsBackingField; }
            set { _retryWaitInSecondsBackingField = value; }
        }

        /// <summary>
        /// load the control file from an xml file on disk
        /// </summary>
        /// <param name="fileName">filename of the xml file</param>
        protected void LoadFromFile(string fileName)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);

            ReadGlobalSection(xmlDocument);
            ReadPodcasts(xmlDocument);
        }

        /// <summary>
        /// load the control file from an in memory object
        /// </summary>
        /// <param name="document">doument to load from</param>
        protected void LoadFromXml(IXPathNavigable document)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.InnerXml = document.CreateNavigator().InnerXml;

            ReadGlobalSection(xmlDocument);
            ReadPodcasts(xmlDocument);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void ReadGlobalSection(XmlDocument xmlDocument)
        {
            _sourceRootBackingField = GetNodeText(xmlDocument,"podcasts/global/sourceRoot");
            _destinationRootBackingField = GetNodeText(xmlDocument,"podcasts/global/destinationRoot");
            _playlistFileNameBackingField = GetNodeText(xmlDocument,"podcasts/global/playlistFilename");

            string format = GetNodeText(xmlDocument,"podcasts/global/playlistFormat").ToUpperInvariant();
            switch (format)
            {
                case "WPL":
                    _playlistFormatBackingField = PlaylistFormat.WPL;
                    break;
                case "ASX":
                    _playlistFormatBackingField = PlaylistFormat.ASX;
                    break;
                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "{0} is not a valid value for the playlist format", format));
            }

            try
            {
                _freeSpaceToLeaveOnDestinationBackingField = Convert.ToInt64(GetNodeText(xmlDocument,"podcasts/global/freeSpaceToLeaveOnDestinationMB"), CultureInfo.InvariantCulture);
            }
            catch {}

            try
            {
                _freeSpaceToLeaveOnDownloadBackingField = Convert.ToInt64(GetNodeText(xmlDocument,"podcasts/global/freeSpaceToLeaveOnDownloadMB"), CultureInfo.InvariantCulture);
            }
            catch { }

            try
            {
                _maximumNumberOfConcurrentDownloadsBackingField = Convert.ToInt32(GetNodeText(xmlDocument, "podcasts/global/maximumNumberOfConcurrentDownloads"), CultureInfo.InvariantCulture);
            }
            catch { }

            try
            {
                _retryWaitInSecondsBackingField = Convert.ToInt32(GetNodeText(xmlDocument,"podcasts/global/retryWaitInSeconds"), CultureInfo.InvariantCulture);
            }
            catch { }

            _sortFieldBackingField = GetNodeTextOrDefault(xmlDocument,"podcasts/global/sortfield", DefaultSortField);
            _sortDirectionBackingField = GetNodeTextOrDefault(xmlDocument,"podcasts/global/sortdirection", DefaultSortDirection);
            _feedMaximumDaysOldBackingField = GetNodeTextOrDefault(xmlDocument,"podcasts/global/feed/maximumDaysOld", int.MaxValue.ToString(CultureInfo.InvariantCulture));
            _feedDeleteDownloadsDaysOldBackingField = GetNodeTextOrDefault(xmlDocument,"podcasts/global/feed/deleteDownloadsDaysOld", int.MaxValue.ToString(CultureInfo.InvariantCulture));
            _feedFormatBackingField = GetNodeTextOrDefault(xmlDocument,"podcasts/global/feed/format", DefaultFeedFormat);
            _feedEpisodeNamingStyleBackingField = GetNodeTextOrDefault(xmlDocument,"podcasts/global/feed/namingStyle", DefaultFeedFormat);
            _feedEpisodeDownloadStrategyBackingField = GetNodeTextOrDefault(xmlDocument,"podcasts/global/feed/downloadStrategy", DefaultFeedEpisodeDownloadStrategy);
        }

        private void ReadPodcasts(XmlDocument xmlDocument)
        {
            _podcastsBackingField = new List<PodcastInfo>();

            var podcastNodes = xmlDocument.SelectNodes("podcasts/podcast");

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
                        SortField = GetNodeTextOrDefault(podcastNode, "sortfield", _sortFieldBackingField),
                        AscendingSort = !(GetNodeTextOrDefault(podcastNode, "sortdirection", _sortDirectionBackingField).ToUpperInvariant().StartsWith("DESC", StringComparison.Ordinal))
                    };

                    _podcastsBackingField.Add(podcastInfo);
                }
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
                Format = ReadFeedFormat(GetNodeTextOrDefault(feedNode, "format", _feedFormatBackingField)),
                MaximumDaysOld = ReadMaximumDaysOld(feedNode),
                NamingStyle = ReadFeedEpisodeNamingStyle(GetNodeTextOrDefault(feedNode, "namingStyle", _feedEpisodeNamingStyleBackingField)),
                DownloadStrategy = ReadFeedEpisodeDownloadStrategy(GetNodeTextOrDefault(feedNode, "downloadStrategy", _feedEpisodeDownloadStrategyBackingField)),
                DeleteDownloadsDaysOld = ReadDeleteDownloadsDaysOld(feedNode)
            };
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
                return Convert.ToInt32(GetNodeTextOrDefault(feedNode, "maximumDaysOld", _feedMaximumDaysOldBackingField), CultureInfo.InvariantCulture);
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
                var days = Convert.ToInt32(GetNodeTextOrDefault(feedNode, "deleteDownloadsDaysOld", _feedDeleteDownloadsDaysOldBackingField), CultureInfo.InvariantCulture);
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
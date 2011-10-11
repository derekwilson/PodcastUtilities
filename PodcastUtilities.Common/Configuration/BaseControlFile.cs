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
    public abstract class BaseControlFile : IControlFileGlobalDefaults
    {
        /// <summary>
        /// the file pattern for files that are in a podcast
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal string DefaultFilePattern { get; set; }
        /// <summary>
        /// the field we are using to sort the podcasts on
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal PodcastFileSortField DefaultSortField { get; set; }
        /// <summary>
        /// direction to sort in
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal bool DefaultAscendingSort { get; set; }
        /// <summary>
        /// global default maximum days old for feed download
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal int DefaultFeedMaximumDaysOld { get; set; }
        /// <summary>
        /// global default number of days before deleteing a download
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal int DefaultFeedDeleteDownloadsDaysOld { get; set; }
        /// <summary>
        /// global default feed format 
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal PodcastFeedFormat DefaultFeedFormat { get; set; }
        /// <summary>
        /// global default for naming downloaded episodes
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal PodcastEpisodeNamingStyle DefaultFeedEpisodeNamingStyle { get; set; }
        /// <summary>
        /// global default for mechanism for downloading episodes
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal PodcastEpisodeDownloadStrategy DefaultFeedEpisodeDownloadStrategy { get; set; }

        /// <summary>
        /// setup the hard coded defaults
        /// </summary>
        protected BaseControlFile()
        {
            SetHardcodedDefaults();
        }

        void SetHardcodedDefaults()
        {
            DefaultFilePattern = "*.mp3";
            DefaultAscendingSort = true;
            DefaultSortField = PodcastFileSortField.FileName;
            DefaultFeedFormat = PodcastFeedFormat.RSS;
            DefaultFeedEpisodeNamingStyle = PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime;
            DefaultFeedEpisodeDownloadStrategy = PodcastEpisodeDownloadStrategy.All;
            DefaultFeedMaximumDaysOld = int.MaxValue;
            DefaultFeedDeleteDownloadsDaysOld = int.MaxValue;

            FreeSpaceToLeaveOnDestination = 0;
            FreeSpaceToLeaveOnDownload = 0;
            MaximumNumberOfConcurrentDownloads = 5;
            RetryWaitInSeconds = 10;

            Podcasts = new List<PodcastInfo>();
        }

        /// <summary>
        /// the global default for feeds
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public int GetDefaultDeleteDownloadsDaysOld()
        {
            return DefaultFeedDeleteDownloadsDaysOld;
        }

        /// <summary>
        /// the global default for feeds
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public PodcastEpisodeDownloadStrategy GetDefaultDownloadStrategy()
        {
            return DefaultFeedEpisodeDownloadStrategy;
        }

        /// <summary>
        /// the global default for feeds
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public PodcastFeedFormat GetDefaultFeedFormat()
        {
            return DefaultFeedFormat;
        }

        /// <summary>
        /// the global default for feeds
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public int GetDefaultMaximumDaysOld()
        {
            return DefaultFeedMaximumDaysOld;
        }

        /// <summary>
        /// the global default for feeds
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public PodcastEpisodeNamingStyle GetDefaultNamingStyle()
        {
            return DefaultFeedEpisodeNamingStyle;
        }

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetDefaultFilePattern()
        {
            return DefaultFilePattern;
        }

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public bool GetDefaultAscendingSort()
        {
            return DefaultAscendingSort;
        }

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public PodcastFileSortField GetDefaultSortField()
        {
            return DefaultSortField;
        }

        /// <summary>
        /// pathname to the root folder to copy from when synchronising
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal string SourceRoot { get; set; }

        /// <summary>
        /// pathname to the destination root folder
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal string DestinationRoot { get; set; }

        /// <summary>
        /// filename and extension for the generated playlist
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal string PlaylistFileName { get; set; }

        /// <summary>
        /// the format for the generated playlist
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal PlaylistFormat PlaylistFormat { get; set; }

        /// <summary>
        /// free space in MB to leave on the destination device when syncing
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal long FreeSpaceToLeaveOnDestination { get; set; }

        /// <summary>
        /// free space in MB to leave on the download device - when downloading
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal long FreeSpaceToLeaveOnDownload { get; set; }

        /// <summary>
        /// the configuration for the individual podcasts
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal IList<PodcastInfo> Podcasts { get; private set; }

        /// <summary>
        /// maximum number of background downloads
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal int MaximumNumberOfConcurrentDownloads { get; set; }

        /// <summary>
        /// number of seconds to wait when trying a file conflict
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal int RetryWaitInSeconds { get; set; }

        /// <summary>
        /// pathname to the root folder to copy from when synchronising
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetSourceRoot()
        {
            return SourceRoot;
        }

        /// <summary>
        /// pathname to the destination root folder
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetDestinationRoot()
        {
            return DestinationRoot;
        }

        /// <summary>
        /// filename and extension for the generated playlist
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetPlaylistFileName()
        {
            return PlaylistFileName;
        }

        /// <summary>
        /// the format for the generated playlist
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public PlaylistFormat GetPlaylistFormat()
        {
            return PlaylistFormat;
        }

        /// <summary>
        /// free space in MB to leave on the destination device when syncing
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public long GetFreeSpaceToLeaveOnDestination()
        {
            return FreeSpaceToLeaveOnDestination;
        }

        /// <summary>
        /// free space in MB to leave on the download device - when downloading
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public long GetFreeSpaceToLeaveOnDownload()
        {
            return FreeSpaceToLeaveOnDownload;
        }

        /// <summary>
        /// the configuration for the individual podcasts
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public IEnumerable<PodcastInfo> GetPodcasts()
        {
            return Podcasts;
        }

        /// <summary>
        /// maximum number of background downloads
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public int GetMaximumNumberOfConcurrentDownloads()
        {
            return MaximumNumberOfConcurrentDownloads;
        }

        /// <summary>
        /// number of seconds to wait when trying a file conflict
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public int GetRetryWaitInSeconds()
        {
            return RetryWaitInSeconds;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized. 
        ///                 </param>
        public void ReadXml(XmlReader reader)
        {
            SetHardcodedDefaults();
            XmlSerializationHelper.ProcessElement(reader, "podcasts", ProcessPodcastsElements);
        }

        private ProcessorResult ProcessPodcastsElements(XmlReader reader)
        {
            var result = ProcessorResult.Processed;

            var elementName = reader.LocalName;

            switch (elementName)
            {
                case "global":
                    XmlSerializationHelper.ProcessElement(reader, "global", ProcessGlobalElements);
                    break;
                case "podcast":
                    var newPodcast = new PodcastInfo(this);
                    newPodcast.ReadXml(reader);
                    Podcasts.Add(newPodcast);
                    break;
                default:
                    result = ProcessorResult.Ignored;
                    break;
            }

            return result;
        }

        private ProcessorResult ProcessGlobalElements(XmlReader reader)
        {
            var result = ProcessorResult.Processed;

            var elementName = reader.LocalName;

            if (elementName == "feed")
            {
                XmlSerializationHelper.ProcessElement(reader, "feed", ProcessGlobalFeedElements);
            }
            
            reader.Read();
            var content = reader.Value.Trim();

            long longValue;
            int intValue;
            switch (elementName)
            {
                case "sourceRoot":
                    SourceRoot = content;
                    break;
                case "destinationRoot":
                    DestinationRoot = content;
                    break;
                case "playlistFilename":
                    PlaylistFileName = content;
                    break;
                case "playlistFormat":
                    PlaylistFormat = ReadPlaylistFormat(content);
                    break;
                case "freeSpaceToLeaveOnDestinationMB":
                    if (long.TryParse(content, NumberStyles.Integer, CultureInfo.InvariantCulture,out longValue))
                    {
                        FreeSpaceToLeaveOnDestination = longValue;
                    }
                    break;
                case "freeSpaceToLeaveOnDownloadMB":
                    if (long.TryParse(content, NumberStyles.Integer, CultureInfo.InvariantCulture,out longValue))
                    {
                        FreeSpaceToLeaveOnDownload = longValue;
                    }
                    break;
                case "maximumNumberOfConcurrentDownloads":
                    if (int.TryParse(content, NumberStyles.Integer, CultureInfo.InvariantCulture,out intValue))
                    {
                        MaximumNumberOfConcurrentDownloads = intValue;
                    }
                    break;
                case "retryWaitInSeconds":
                    if (int.TryParse(content, NumberStyles.Integer, CultureInfo.InvariantCulture,out intValue))
                    {
                        RetryWaitInSeconds = intValue;
                    }
                    break;
                case "pattern":
                    if (!string.IsNullOrEmpty(content))
                    {
                        DefaultFilePattern = content;
                    }
                    break;
                case "sortfield":
                    DefaultSortField = PodcastInfo.ReadSortField(content);
                    break;
                case "sortdirection":
                    DefaultAscendingSort = PodcastInfo.ReadSortDirection(content);
                    break;
                default:
                    result = ProcessorResult.Ignored;
                    break;
            }
            return result;
        }

        private ProcessorResult ProcessGlobalFeedElements(XmlReader reader)
        {
            var result = ProcessorResult.Processed;

            var elementName = reader.LocalName;
            reader.Read();
            var content = reader.Value.Trim();

            int intValue;
            switch (elementName)
            {
                case "maximumDaysOld":
                    if (int.TryParse(content, NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue))
                    {
                        DefaultFeedMaximumDaysOld = intValue;
                    }
                    break;
                case "deleteDownloadsDaysOld":
                    if (int.TryParse(content, NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue))
                    {
                        DefaultFeedDeleteDownloadsDaysOld = intValue;
                    }
                    break;
                case "format":
                    DefaultFeedFormat = FeedInfo.ReadFeedFormat(content);
                    break;
                case "namingStyle":
                    DefaultFeedEpisodeNamingStyle = FeedInfo.ReadFeedEpisodeNamingStyle(content);
                    break;
                case "downloadStrategy":
                    DefaultFeedEpisodeDownloadStrategy = FeedInfo.ReadFeedEpisodeDownloadStrategy(content);
                    break;
                default:
                    result = ProcessorResult.Ignored;
                    break;
            }
            return result;
        }

        private static PlaylistFormat ReadPlaylistFormat(string format)
        {
            switch (format.ToUpperInvariant())
            {
                case "WPL":
                    return PlaylistFormat.WPL;
                case "ASX":
                    return PlaylistFormat.ASX;
                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "{0} is not a valid value for the playlist format", format));
            }
        }
    }
}
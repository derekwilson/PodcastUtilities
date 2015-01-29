#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
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
        protected internal int DefaultNumberOfFiles { get; set; }
        /// <summary>
        /// the file pattern for files that are in a podcast
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal string DefaultFilePattern { get; set; }
        /// <summary>
        /// global default for option to delete folder that become empty
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal bool DefaultDeleteEmptyFolder { get; set; }
        /// <summary>
        /// the command to be run after the download
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal string DefaultPostDownloadCommand { get; set; }
        /// <summary>
        /// the args for the post download command
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal string DefaultPostDownloadArguments { get; set; }
        /// <summary>
        /// the cwd for the post download command
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal string DefaultPostDownloadWorkingDirectory { get; set; }
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
        /// only used for unit testing
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        public BaseControlFile(XmlDocument document)
        {
            SetHardcodedDefaults();
            MemoryStream stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;
            ReadXml(XmlReader.Create(stream));
        }

        /// <summary>
        /// setup the hard coded defaults
        /// </summary>
        protected BaseControlFile()
        {
            SetHardcodedDefaults();
        }

        void SetHardcodedDefaults()
        {
            DefaultNumberOfFiles = -1;
            DefaultFilePattern = "*.mp3";
            DefaultDeleteEmptyFolder = false;
            DefaultAscendingSort = true;
            DefaultSortField = PodcastFileSortField.FileName;
            DefaultFeedFormat = PodcastFeedFormat.RSS;
            DefaultFeedEpisodeNamingStyle = PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime;
            DefaultFeedEpisodeDownloadStrategy = PodcastEpisodeDownloadStrategy.All;
            DefaultFeedMaximumDaysOld = int.MaxValue;
            DefaultFeedDeleteDownloadsDaysOld = int.MaxValue;
            DefaultPostDownloadCommand = "";
            DefaultPostDownloadArguments = "";
            DefaultPostDownloadWorkingDirectory = "";

            FreeSpaceToLeaveOnDestination = 0;
            FreeSpaceToLeaveOnDownload = 0;
            MaximumNumberOfConcurrentDownloads = 5;
            RetryWaitInSeconds = 10;
            DiagnosticOutput = DiagnosticOutputLevel.None;
            DiagnosticRetainTemporaryFiles = false;

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
        public int GetDefaultNumberOfFiles()
        {
            return DefaultNumberOfFiles;
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
        /// the global default for feeds
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public bool GetDefaultDeleteEmptyFolder()
        {
            return DefaultDeleteEmptyFolder;
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
        /// the global default for post download command
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetDefaultPostDownloadCommand()
        {
            return DefaultPostDownloadCommand;
        }

        /// <summary>
        /// the global default for post download command args
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetDefaultPostDownloadArguments()
        {
            return DefaultPostDownloadArguments;
        }

        /// <summary>
        /// the global default for post download command cwd
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetDefaultPostDownloadWorkingDirectory()
        {
            return DefaultPostDownloadWorkingDirectory;
        }

        /// <summary>
        /// level of diagnostic output
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal DiagnosticOutputLevel DiagnosticOutput { get; set; }

        /// <summary>
        /// set to retain intermediate files
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        protected internal bool DiagnosticRetainTemporaryFiles { get; set; }

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
        /// level of diagnostic output
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public DiagnosticOutputLevel GetDiagnosticOutput()
        {
            return DiagnosticOutput;
        }

        /// <summary>
        /// set to retain intermediate files
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public bool GetDiagnosticRetainTemporaryFiles()
        {
            return DiagnosticRetainTemporaryFiles;
        }

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
                return result;
            }
            if (elementName == "postdownloadcommand")
            {
                XmlSerializationHelper.ProcessElement(reader, "postdownloadcommand", ProcessGlobalPostDownloadCommandElements);
                return result;
            }
            if (elementName == "diagnostics")
            {
                XmlSerializationHelper.ProcessElement(reader, "diagnostics", ProcessGlobalDiagnosticsElements);
                return result;
            }
            
            if (!reader.IsEmptyElement)
            {
                reader.Read();
            }
            var content = reader.Value.Trim();

            long longValue;
            int intValue;
            bool boolValue;
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
                    if (int.TryParse(content, NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue))
                    {
                        RetryWaitInSeconds = intValue;
                    }
                    break;
                case "number":
                    if (int.TryParse(content, NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue))
                    {
                        DefaultNumberOfFiles = intValue;
                    }
                    break;
                case "pattern":
                    if (!string.IsNullOrEmpty(content))
                    {
                        DefaultFilePattern = content;
                    }
                    break;
                case "deleteEmptyFolder":
                    if (bool.TryParse(content, out boolValue))
                    {
                        DefaultDeleteEmptyFolder = boolValue;
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

        private ProcessorResult ProcessGlobalDiagnosticsElements(XmlReader reader)
        {
            var result = ProcessorResult.Processed;

            var elementName = reader.LocalName;
            if (!reader.IsEmptyElement)
            {
                reader.Read();
            }
            var content = reader.Value.Trim();

            switch (elementName)
            {
                case "outputLevel":
                    DiagnosticOutput = ReadDiagnosticOutputLevel(content);
                    break;
                case "retainTempFiles":
                    DiagnosticRetainTemporaryFiles = ReadDiagnosticRetainTemporaryFiles(content);
                    break;
                default:
                    result = ProcessorResult.Ignored;
                    break;
            }
            return result;
        }

        private ProcessorResult ProcessGlobalPostDownloadCommandElements(XmlReader reader)
        {
            var result = ProcessorResult.Processed;

            var elementName = reader.LocalName;
            if (!reader.IsEmptyElement)
            {
                reader.Read();
            }
            var content = reader.Value.Trim();

            switch (elementName)
            {
                case "command":
                    if (!string.IsNullOrEmpty(content))
                    {
                        DefaultPostDownloadCommand = content;
                    }
                    break;
                case "arguments":
                    if (!string.IsNullOrEmpty(content))
                    {
                        DefaultPostDownloadArguments = content;
                    }
                    break;
                case "workingdirectory":
                    if (!string.IsNullOrEmpty(content))
                    {
                        DefaultPostDownloadWorkingDirectory = content;
                    }
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
            if (!reader.IsEmptyElement)
            {
                reader.Read();
            }
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
                case "UNKNOWN":
                    return PlaylistFormat.Unknown;
                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "{0} is not a valid value for the playlist format", format));
            }
        }

        private static bool ReadDiagnosticRetainTemporaryFiles(string content)
        {
            return content.ToUpperInvariant().StartsWith("TRUE", StringComparison.Ordinal);
        }

        private static DiagnosticOutputLevel ReadDiagnosticOutputLevel(string format)
        {
            switch (format.ToUpperInvariant())
            {
                case "VERBOSE":
                    return DiagnosticOutputLevel.Verbose;
                case "NONE":
                    return DiagnosticOutputLevel.None;
                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "{0} is not a valid value for the diagnostic output level", format));
            }
        }
    }
}
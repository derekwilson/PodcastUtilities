using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// controlfile implementation that supports loading and saving
    /// </summary>
    public class ReadWriteControlFile : BaseControlFile, IReadWriteControlFile, IReadWriteControlFileGlobalDefaults
    {
        		/// <summary>
		/// create the object and read the control file from the specified filename
		/// </summary>
		/// <param name="fileName">pathname to the control file xml</param>
        public ReadWriteControlFile(string fileName)
		{
            XmlReaderSettings readSettings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };

            FileStream fileStream = new FileStream(fileName, FileMode.Open);

            ReadXml(XmlReader.Create(fileStream, readSettings));
        }

        /// <summary>
        /// used for cloning
        /// </summary>
        protected ReadWriteControlFile() : base()
        {
        }

                /// <summary>
        /// only used for unit testing
        /// </summary>
        public ReadWriteControlFile(XmlReader xml) : base(xml)
        {
        }

        /// <summary>
        /// only used for unit testing
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        public ReadWriteControlFile(XmlDocument document) : base(document)
        {
        }

        /// <summary>
        /// only used for unit testing
        /// </summary>
        public ReadWriteControlFile(Stream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// the global default for feeds
        /// </summary>
        public void SetDefaultDeleteDownloadsDaysOld(int deleteDaysOld)
        {
            DefaultFeedDeleteDownloadsDaysOld = deleteDaysOld;
        }

        /// <summary>
        /// the global default for feeds
        /// </summary>
        public void SetDefaultDownloadStrategy(PodcastEpisodeDownloadStrategy strategy)
        {
            DefaultFeedEpisodeDownloadStrategy = strategy;
        }

        /// <summary>
        /// the global default for feeds
        /// </summary>
        public void SetDefaultFeedFormat(PodcastFeedFormat format)
        {
            DefaultFeedFormat = format;
        }

        /// <summary>
        /// the global default for feeds
        /// </summary>
        public void SetDefaultMaximumDaysOld(int maximumDaysOld)
        {
            DefaultFeedMaximumDaysOld = maximumDaysOld;
        }

        /// <summary>
        /// the global default for feeds
        /// </summary>
        public void SetDefaultNamingStyle(PodcastEpisodeNamingStyle namingStyle)
        {
            DefaultFeedEpisodeNamingStyle = namingStyle;
        }

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        public void SetDefaultNumberOfFiles(int numberOfFiles)
        {
            DefaultNumberOfFiles = numberOfFiles;
        }

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        public void SetDefaultFilePattern(string pattern)
        {
            DefaultFilePattern = pattern;
        }

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        public void SetDefaultAscendingSort(bool ascendingSort)
        {
            DefaultAscendingSort = ascendingSort;
        }

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        public void SetDefaultSortField(PodcastFileSortField sortField)
        {
            DefaultSortField = sortField;
        }

        /// <summary>
        /// level of diagnostic output
        /// </summary>
        public void SetDiagnosticOutput(DiagnosticOutputLevel level)
        {
            DiagnosticOutput = level;
        }

        /// <summary>
        /// set to retain intermediate files
        /// </summary>
        public void SetDiagnosticRetainTemporaryFiles(bool retainFiles)
        {
            DiagnosticRetainTemporaryFiles = retainFiles;
        }

        /// <summary>
        /// pathname to the root folder to copy from when synchronising
        /// </summary>
        public void SetSourceRoot(string value)
        {
            SourceRoot = value;
        }

        /// <summary>
        /// pathname to the destination root folder
        /// </summary>
        public void SetDestinationRoot(string value)
        {
            DestinationRoot = value;
        }

        /// <summary>
        /// filename and extension for the generated playlist
        /// </summary>
        public void SetPlaylistFileName(string value)
        {
            PlaylistFileName = value;
        }

        /// <summary>
        /// the format for the generated playlist
        /// </summary>
        public void SetPlaylistFormat(PlaylistFormat value)
        {
            PlaylistFormat = value;
        }

        /// <summary>
        /// free space in MB to leave on the destination device when syncing
        /// </summary>
        public void SetFreeSpaceToLeaveOnDestination(long value)
        {
            FreeSpaceToLeaveOnDestination = value;
        }

        /// <summary>
        /// free space in MB to leave on the download device - when downloading
        /// </summary>
        public void SetFreeSpaceToLeaveOnDownload(long value)
        {
            FreeSpaceToLeaveOnDownload = value;
        }

        /// <summary>
        /// maximum number of background downloads
        /// </summary>
        public void SetMaximumNumberOfConcurrentDownloads(int value)
        {
            MaximumNumberOfConcurrentDownloads = value;
        }

        /// <summary>
        /// number of seconds to wait when trying a file conflict
        /// </summary>
        public void SetRetryWaitInSeconds(int value)
        {
            RetryWaitInSeconds = value;
        }

        /// <summary>
        /// persist the control file to disk
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveToFile(string fileName)
        {
            XmlWriterSettings writeSettings = new XmlWriterSettings
            {
                OmitXmlDeclaration = false,
                Indent = true,
                ConformanceLevel = ConformanceLevel.Document,
                CloseOutput = true,
                Encoding = Encoding.UTF8
            };

            FileStream fileStream = new FileStream(fileName,FileMode.Create);
            var xmlWriter = XmlWriter.Create(fileStream, writeSettings);

            // simulate the behaviour of XmlSerialisation
            xmlWriter.WriteStartElement("podcasts");
            WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();

            xmlWriter.Flush();
            xmlWriter.Close();
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
            var copy = new ReadWriteControlFile();
            XmlSerializationHelper.CloneUsingXmlSerialization("podcasts", this, copy);
            return copy;
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized. 
        ///                 </param>
        public void WriteXml(XmlWriter writer)
        {
            Assembly me = System.Reflection.Assembly.GetExecutingAssembly();
            AssemblyName name = me.GetName();
            writer.WriteComment(string.Format(CultureInfo.InvariantCulture, "Written by PodcastUtilities.Common v{0}",name.Version));

            writer.WriteStartElement("global");
            if (SourceRoot != null)
            {
                writer.WriteElementString("sourceRoot", SourceRoot);
            }
            if (DestinationRoot != null)
            {
                writer.WriteElementString("destinationRoot", DestinationRoot);
            }
            if (PlaylistFileName != null)
            {
                writer.WriteElementString("playlistFilename", PlaylistFileName);
            }
            writer.WriteElementString("playlistFormat", WritePlaylistFormat(PlaylistFormat));
            writer.WriteElementString("freeSpaceToLeaveOnDestinationMB", FreeSpaceToLeaveOnDestination.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("freeSpaceToLeaveOnDownloadMB", FreeSpaceToLeaveOnDownload.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("maximumNumberOfConcurrentDownloads", MaximumNumberOfConcurrentDownloads.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("retryWaitInSeconds", RetryWaitInSeconds.ToString(CultureInfo.InvariantCulture));

            writer.WriteElementString("number", DefaultNumberOfFiles.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("pattern", DefaultFilePattern);
            writer.WriteElementString("sortfield", PodcastInfo.WriteSortField(DefaultSortField));
            writer.WriteElementString("sortdirection", PodcastInfo.WriteSortDirection(DefaultAscendingSort));

            writer.WriteStartElement("feed");
            writer.WriteElementString("maximumDaysOld", DefaultFeedMaximumDaysOld.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("deleteDownloadsDaysOld", DefaultFeedDeleteDownloadsDaysOld.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("format", FeedInfo.WriteFeedFormat(DefaultFeedFormat));
            writer.WriteElementString("namingStyle", FeedInfo.WriteFeedEpisodeNamingStyle(DefaultFeedEpisodeNamingStyle));
            writer.WriteElementString("downloadStrategy", FeedInfo.WriteFeedEpisodeDownloadStrategy(DefaultFeedEpisodeDownloadStrategy));
            writer.WriteEndElement();

            writer.WriteStartElement("diagnostics");
            writer.WriteElementString("retainTempFiles", WriteDiagnosticRetainTemporaryFiles(DiagnosticRetainTemporaryFiles));
            writer.WriteElementString("outputLevel", WriteDiagnosticOutputLevel(DiagnosticOutput));
            writer.WriteEndElement();

            writer.WriteEndElement();

            foreach (var podcastInfo in Podcasts)
            {
                // simulate the behaviour of XmlSerialisation
                writer.WriteStartElement("podcast");
                podcastInfo.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        private static string WriteDiagnosticOutputLevel(DiagnosticOutputLevel diagnosticOutput)
        {
            switch (diagnosticOutput)
            {
                case DiagnosticOutputLevel.Verbose:
                    return "verbose";
                default:
                    return "none";
            }
        }

        private static string WriteDiagnosticRetainTemporaryFiles(bool diagnosticRetainTemporaryFiles)
        {
            if (diagnosticRetainTemporaryFiles)
            {
                return "true";
            }
            return "false";
        }

        private static string WritePlaylistFormat(PlaylistFormat playlistFormat)
        {
            switch (playlistFormat)
            {
                case PlaylistFormat.ASX:
                    return "asx";
                case PlaylistFormat.WPL:
                    return "wpl";
                default:
                    throw new ArgumentOutOfRangeException("playlistFormat");
            }
        }
    }
}
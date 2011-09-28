using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PodcastUtilities.Common.Exceptions;

namespace PodcastUtilities.Common.Configuration
{    
    /// <summary>
    /// configuration info for a podcast feed
    /// </summary>
    public class FeedInfo : IFeedInfo
    {
        private readonly IControlFileGlobalDefaults _controlFileGlobalDefaults;

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
            XmlWriterSettings writeSettings = new XmlWriterSettings();
            writeSettings.OmitXmlDeclaration = true;
            writeSettings.ConformanceLevel = ConformanceLevel.Fragment;
            writeSettings.CloseOutput = false;
            writeSettings.Encoding = Encoding.UTF8;

            MemoryStream memoryStream = new MemoryStream();
            var xmlWriter = XmlWriter.Create(memoryStream, writeSettings);

            // simulate the behaviour of XmlSerialisation
            xmlWriter.WriteStartElement("feed");
            this.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();

            xmlWriter.Flush(); 
            memoryStream.Position = 0;

            XmlReaderSettings readSettings = new XmlReaderSettings();
            readSettings.ConformanceLevel = ConformanceLevel.Fragment;
            var reader = XmlReader.Create(memoryStream, readSettings);

            var copy = new FeedInfo(_controlFileGlobalDefaults);

            copy.ReadXml(reader);

            //var copy = new FeedInfo(_controlFileGlobalDefaults) {Address = Address};
            //if (DownloadStrategy.IsSet)
            //{
            //    copy.DownloadStrategy.Value = DownloadStrategy.Value;
            //}
            //if (Format.IsSet)
            //{
            //    copy.Format.Value = Format.Value;
            //}
            //if (MaximumDaysOld.IsSet)
            //{
            //    copy.MaximumDaysOld.Value = MaximumDaysOld.Value;
            //}
            //if (NamingStyle.IsSet)
            //{
            //    copy.NamingStyle.Value = NamingStyle.Value;
            //}
            //if (DeleteDownloadsDaysOld.IsSet)
            //{
            //    copy.DeleteDownloadsDaysOld.Value = DeleteDownloadsDaysOld.Value;
            //}

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
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized. 
        ///                 </param>
        public void ReadXml(XmlReader reader)
        {
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "feed")
            {
                reader.Read(); // Skip ahead to next node
                var element = reader.MoveToContent();
                while (element != XmlNodeType.None)
                {
                    if (element == XmlNodeType.EndElement && reader.LocalName == "feed")
                    {
                        break;
                    }
                    if (reader.IsStartElement())
                    {
                        var elementName = reader.LocalName;
                        reader.Read();
                        ProcessFeedElements(elementName, reader.Value.Trim());
                        reader.Read();
                    }
                    else
                    {
                        reader.Read();
                    }
                    element = reader.MoveToContent();
                }
            }
        }

        private void ProcessFeedElements(string localName, string content)
        {
            switch (localName)
            {
                case "url":
                    Address = new Uri(content);
                    break;
                case "downloadStrategy":
                    DownloadStrategy.Value = ReadFeedEpisodeDownloadStrategy(content);
                    break;
                case "format":
                    Format.Value = ReadFeedFormat(content);
                    break;
                case "maximumDaysOld":
                    MaximumDaysOld.Value = Convert.ToInt32(content,CultureInfo.InvariantCulture);
                    break;
                case "namingStyle":
                    NamingStyle.Value = ReadFeedEpisodeNamingStyle(content);
                    break;
                case "deleteDownloadsDaysOld":
                    DeleteDownloadsDaysOld.Value = Convert.ToInt32(content,CultureInfo.InvariantCulture);
                    break;
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized. 
        ///                 </param>
        public void WriteXml(XmlWriter writer)
        {
            if (Address != null)
            {
                writer.WriteElementString("url", Address.ToString());
            }
            if (DownloadStrategy.IsSet)
            {
                writer.WriteElementString("downloadStrategy", WriteFeedEpisodeDownloadStrategy(DownloadStrategy));
            }
            if (Format.IsSet)
            {
                writer.WriteElementString("format", WriteFeedFormat(Format));
            }
            if (MaximumDaysOld.IsSet)
            {
                writer.WriteElementString("maximumDaysOld", MaximumDaysOld.Value.ToString(CultureInfo.InvariantCulture));
            }
            if (NamingStyle.IsSet)
            {
                writer.WriteElementString("namingStyle", WriteFeedEpisodeNamingStyle(NamingStyle));
            }
            if (DeleteDownloadsDaysOld.IsSet)
            {
                writer.WriteElementString("deleteDownloadsDaysOld", DeleteDownloadsDaysOld.Value.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// parse the feed naming style
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static PodcastEpisodeNamingStyle ReadFeedEpisodeNamingStyle(string format)
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

        private static string WriteFeedEpisodeNamingStyle(IDefaultableItem<PodcastEpisodeNamingStyle> namingStyle)
        {
            switch (namingStyle.Value)
            {
                case PodcastEpisodeNamingStyle.UrlFileName:
                    return "url";
                case PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime:
                    return "pubdate_url";
                case PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTime:
                    return "pubdate_title_url";
                case PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTimeInfolder:
                    return "pubdate_folder_title_url";
                case PodcastEpisodeNamingStyle.EpisodeTitle:
                    return "etitle";
                case PodcastEpisodeNamingStyle.EpisodeTitleAndPublishDateTime:
                    return "pubdate_etitle";
                default:
                    throw new EnumOutOfRangeException("namingStyle");
            }
        }

        /// <summary>
        /// parse the download strategy
        /// </summary>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public static PodcastEpisodeDownloadStrategy ReadFeedEpisodeDownloadStrategy(string strategy)
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

        private static string WriteFeedEpisodeDownloadStrategy(IDefaultableItem<PodcastEpisodeDownloadStrategy> downloadStrategy)
        {
            switch (downloadStrategy.Value)
            {
                case PodcastEpisodeDownloadStrategy.All:
                    return "all";
                case PodcastEpisodeDownloadStrategy.HighTide:
                    return "high_tide";
                case PodcastEpisodeDownloadStrategy.Latest:
                    return "latest";
                default:
                    throw new EnumOutOfRangeException("downloadStrategy");
            }
        }

        /// <summary>
        /// parse the feed format
        /// </summary>
        public static PodcastFeedFormat ReadFeedFormat(string format)
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

        private static string WriteFeedFormat(IDefaultableItem<PodcastFeedFormat> format)
        {
            switch (format.Value)
            {
                case PodcastFeedFormat.RSS:
                    return "rss";
                case PodcastFeedFormat.ATOM:
                    return "atom";
                default:
                    throw new EnumOutOfRangeException("format");
            }
        }
    }
}

﻿#region License
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
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
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
            Format = new DefaultableValueTypeItem<PodcastFeedFormat>(_controlFileGlobalDefaults.GetDefaultFeedFormat);
            MaximumDaysOld = new DefaultableValueTypeItem<int>(_controlFileGlobalDefaults.GetDefaultMaximumDaysOld);
            NamingStyle = new DefaultableValueTypeItem<PodcastEpisodeNamingStyle>(_controlFileGlobalDefaults.GetDefaultNamingStyle);
            DownloadStrategy = new DefaultableValueTypeItem<PodcastEpisodeDownloadStrategy>(_controlFileGlobalDefaults.GetDefaultDownloadStrategy);
            DeleteDownloadsDaysOld = new DefaultableValueTypeItem<int>(_controlFileGlobalDefaults.GetDefaultDeleteDownloadsDaysOld);
            MaximumNumberOfDownloadedItems = new DefaultableValueTypeItem<int>(_controlFileGlobalDefaults.GetDefaultMaximumNumberOfDownloadedItems);
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
        /// maximum number of items from the feed to have in the cache
        /// </summary>
        public IDefaultableItem<int> MaximumNumberOfDownloadedItems { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            var copy = new FeedInfo(_controlFileGlobalDefaults);
            XmlSerializationHelper.CloneUsingXmlSerialization("feed", this, copy);
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
            XmlSerializationHelper.ProcessElement(reader, "feed", ProcessFeedElements);
        }

        private ProcessorResult ProcessFeedElements(XmlReader reader)
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
                    DeleteDownloadsDaysOld.Value = Convert.ToInt32(content, CultureInfo.InvariantCulture);
                    break;
                case "maximumNumberOfDownloadedItems":
                    MaximumNumberOfDownloadedItems.Value = Convert.ToInt32(content, CultureInfo.InvariantCulture);
                    break;
                default:
                    result = ProcessorResult.Ignored;
                    break;

            }
            return result;
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
                writer.WriteElementString("downloadStrategy", WriteFeedEpisodeDownloadStrategy(DownloadStrategy.Value));
            }
            if (Format.IsSet)
            {
                writer.WriteElementString("format", WriteFeedFormat(Format.Value));
            }
            if (MaximumDaysOld.IsSet)
            {
                writer.WriteElementString("maximumDaysOld", MaximumDaysOld.Value.ToString(CultureInfo.InvariantCulture));
            }
            if (NamingStyle.IsSet)
            {
                writer.WriteElementString("namingStyle", WriteFeedEpisodeNamingStyle(NamingStyle.Value));
            }
            if (DeleteDownloadsDaysOld.IsSet)
            {
                writer.WriteElementString("deleteDownloadsDaysOld", DeleteDownloadsDaysOld.Value.ToString(CultureInfo.InvariantCulture));
            }
            if (MaximumNumberOfDownloadedItems.IsSet)
            {
                writer.WriteElementString("maximumNumberOfDownloadedItems", MaximumNumberOfDownloadedItems.Value.ToString(CultureInfo.InvariantCulture));
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
                    return PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime;

            }
        }

        /// <summary>
        /// convert the namingstyle for serialisation
        /// </summary>
        /// <returns></returns>
        public static string WriteFeedEpisodeNamingStyle(PodcastEpisodeNamingStyle namingStyle)
        {
            switch (namingStyle)
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

        /// <summary>
        /// convert the download strategy for serialisation
        /// </summary>
        /// <returns></returns>
        public static string WriteFeedEpisodeDownloadStrategy(PodcastEpisodeDownloadStrategy downloadStrategy)
        {
            switch (downloadStrategy)
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

        /// <summary>
        /// convert the feedformat for serialisation
        /// </summary>
        /// <returns></returns>
        public static string WriteFeedFormat(PodcastFeedFormat format)
        {
            switch (format)
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

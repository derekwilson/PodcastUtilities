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
using PodcastUtilities.Common.Exceptions;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// a podcast feed in RSS format
    /// </summary>
    public class PodcastFeedInRssFormat : IPodcastFeed
    {
        private XmlDocument _feedXml;

        /// <summary>
        /// create a feed from the supplied stream
        /// </summary>
        /// <param name="feedXml">the feed xml</param>
        /// <param name="retainCopyFileName">pathname to save the feed xml to, null to not save</param>
        public PodcastFeedInRssFormat(Stream feedXml, string retainCopyFileName)
        {
            _feedXml = new XmlDocument();

            if (retainCopyFileName != null)
            {
                Stream seekableStream = CopyStream(feedXml);

                seekableStream.Position = 0;
                SaveStreamToFile(seekableStream, retainCopyFileName);

                seekableStream.Position = 0;
                _feedXml.Load(seekableStream);
            }
            else
            {
                _feedXml.Load(feedXml);
            }
        }

        private static Stream CopyStream(Stream feedXml)
        {
            MemoryStream memoryStream = new MemoryStream();

            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = feedXml.Read(buffer, 0, Length);
            while (bytesRead > 0)
            {
                memoryStream.Write(buffer, 0, bytesRead);
                bytesRead = feedXml.Read(buffer, 0, Length);
            }
            return memoryStream;
        }

        private static void SaveStreamToFile(Stream feedXml, string filename)
        {
            FileStream writeStream = new FileStream(filename, FileMode.Create, FileAccess.Write);

            int Length = 256;
            Byte [] buffer = new Byte[Length];
            int bytesRead = feedXml.Read(buffer,0,Length);
            while( bytesRead > 0 ) 
            {
                writeStream.Write(buffer,0,bytesRead);
                bytesRead = feedXml.Read(buffer,0,Length);
            }
            writeStream.Flush();
            writeStream.Close();
        }

        /// <summary>
        /// event that is fired whenever a file is copied of an error occurs
        /// </summary>
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        /// <summary>
        /// the title of the podcast
        /// </summary>
        public string Title
        {
            get { return GetNodeText("rss/channel/title"); }
        }

        /// <summary>
        /// get the episodes of a feed
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public IList<IPodcastFeedItem> Episodes
        {
            get
            {
                var episodes = new List<IPodcastFeedItem>(20);

                XmlNodeList nodes = _feedXml.SelectNodes("rss/channel/item");
                if (nodes == null || nodes.Count < 1)
                {
                    return episodes;
                }

                foreach (XmlNode node in nodes)
                {
                    if (node.SelectSingleNode("enclosure") != null)
                    {
                        try
                        {
                            var episode =
                                new PodcastFeedItem()
                                {
                                    Address = new Uri(GetNodeText(node, "enclosure/@url")),
                                    EpisodeTitle = GetNodeText(node, "title"),
                                    Published = Rfc822DateTime.Parse(GetNodeText(node, "pubDate"))
                                };
                            episodes.Add(episode);
                            OnStatusUpdate(string.Format(CultureInfo.InvariantCulture,"Found: Feed: {0}, Episode: {1}", Title, episode.EpisodeTitle));
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "GetFeedEpisodes: error {0}", ex.Message));
                            OnWarningUpdate(string.Format(CultureInfo.InvariantCulture, "GetFeedEpisodes: warning, unable to add an episode for {0}, {1}", Title, ex.Message));
                        }
                    }
                }

                return episodes;
            }
        }

        private string GetNodeText(string xpath)
        {
            return GetNodeText(_feedXml, xpath);
        }

        private static string GetNodeText(XmlNode root, string xpath)
        {
            XmlNode n = root.SelectSingleNode(xpath);
            if (n == null)
            {
                throw new FeedStructureException("GetNodeText : Node path '" + xpath + "' not found");
            }
            return n.InnerText;
        }

        private void OnStatusUpdate(string message)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateLevel.Verbose, message));
        }

        private void OnWarningUpdate(string message)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateLevel.Warning, message));
        }

        private void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            if (StatusUpdate != null)
                StatusUpdate(this, e);
        }
    }
}
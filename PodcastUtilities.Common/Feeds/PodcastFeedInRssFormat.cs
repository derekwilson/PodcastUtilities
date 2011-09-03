using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        public PodcastFeedInRssFormat(Stream feedXml)
        {
            _feedXml = new XmlDocument();
            _feedXml.Load(feedXml);
        }

        /// <summary>
        /// event that is fired whenever a file is copied of an error occurs
        /// </summary>
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        /// <summary>
        /// the title of the podcast
        /// </summary>
        public string PodcastTitle
        {
            get { return GetNodeText("rss/channel/title"); }
        }

        /// <summary>
        /// get the episodes of a feed
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public IList<IPodcastFeedItem> GetFeedEpisodes()
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
                        OnStatusUpdate(string.Format("Found: Feed: {0}, Episode: {1}", PodcastTitle, episode.EpisodeTitle));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("GetFeedEpisodes: error {0}", ex.Message));
                        OnWarningUpdate(string.Format("GetFeedEpisodes: warning, unable to add an episode for {0}, {1}", PodcastTitle, ex.Message));
                    }
                }
            }

            return episodes;
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
                throw new XmlStructureException("GetNodeText : Node path '" + xpath + "' not found");
            }
            return n.InnerText;
        }

        private void OnStatusUpdate(string message)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Verbose, message));
        }

        private void OnWarningUpdate(string message)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Warning, message));
        }

        private void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            if (StatusUpdate != null)
                StatusUpdate(this, e);
        }
    }
}
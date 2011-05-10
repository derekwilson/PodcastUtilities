using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common
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
        public List<IPodcastFeedItem> GetFeedEpisodes()
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
                    episodes.Add(
                        new PodcastFeedItem()
                            {
                                Address = new Uri(GetNodeText(node, "enclosure/@url")),
                                Title = GetNodeText(node, "title")
                            });
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
                throw new System.Exception("GetNodeText : Node path '" + xpath + "' not found");
            }
            return n.InnerText;
        }
    }
}
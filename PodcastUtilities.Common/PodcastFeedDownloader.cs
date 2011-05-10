using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// object to download the feed channel data
    /// </summary>
    public class PodcastFeedDownloader : IPodcastFeedDownloader
    {
        private IWebClient _webClient;
        private IPodcastFeedFactory _feedFactory;

        /// <summary>
        /// create a downloader
        /// </summary>
        /// <param name="webClient">access to the internet</param>
        /// <param name="feedFactory">factory to create a feed</param>
        public PodcastFeedDownloader(IWebClient webClient, IPodcastFeedFactory feedFactory)
        {
            _webClient = webClient;
            _feedFactory = feedFactory;
        }

        /// <summary>
        /// populate a PodcastFeed from the supplied URI
        /// </summary>
        /// <param name="feedFormat">the format of the feed</param>
        /// <param name="address">the url to get the feed from</param>
        /// <returns>the podcast feed</returns>
        public IPodcastFeed DownLoadFeed(PodcastFeedFormat feedFormat, Uri address)
        {
            Stream feedData = _webClient.OpenRead(address);

            return _feedFactory.CreatePodcastFeed(feedFormat, feedData);
        }
    }
}

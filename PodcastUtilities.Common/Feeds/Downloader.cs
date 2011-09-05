using System;
using System.IO;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// object to download the feed channel data
    /// </summary>
    public class Downloader : IDownloader
    {
        private IWebClient _webClient;
        private IPodcastFeedFactory _feedFactory;

        /// <summary>
        /// create a downloader
        /// </summary>
        /// <param name="webClient">access to the internet</param>
        /// <param name="feedFactory">factory to create a feed</param>
        public Downloader(IWebClient webClient, IPodcastFeedFactory feedFactory)
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
        public IPodcastFeed DownloadFeed(PodcastFeedFormat feedFormat, Uri address)
        {
            Stream feedData = _webClient.OpenRead(address);

            return _feedFactory.CreatePodcastFeed(feedFormat, feedData);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// discover items to be downloaded from a feed
    /// </summary>
    public class PodcastFeedEpisodeFinder
    {
        private IFileUtilities _fileUtilities;
        private IPodcastFeedFactory _feedFactory;
        private IWebClientFactory _webClientFactory;

        /// <summary>
        /// discover items to be downloaded from a feed
        /// </summary>
        public PodcastFeedEpisodeFinder(IFileUtilities fileFinder, IPodcastFeedFactory feedFactory, IWebClientFactory webClientFactory)
        {
            _fileUtilities = fileFinder;
            _webClientFactory = webClientFactory;
            _feedFactory = feedFactory;
        }

        /// <summary>
        /// Find episodes to download
        /// </summary>
        /// <param name="rootFolder">the folder into which we will download</param>
        /// <param name="feed">feed information</param>
        /// <param name="episodesToDownload">list of items to download, will be added to</param>
        public void FindEpisodesToDownload(string rootFolder, FeedInfo feed, IList<FeedSyncItem> episodesToDownload)
        {
            using (var webClient = _webClientFactory.GetWebClient())
            {
                var downloader = new PodcastFeedDownloader(webClient, _feedFactory);

                var feed1 = downloader.DownLoadFeed(feed.Format, feed.Address);
                //feed1.StatusUpdate += StatusUpdate;
                var episodes = feed1.GetFeedEpisodes();
                foreach (IPodcastFeedItem podcastFeedItem in episodes)
                {
                    var destinationPath = Path.Combine(rootFolder, podcastFeedItem.GetFilename());
                    if (!_fileUtilities.FileExists(destinationPath))
                    {
                        var downloadItem = new FeedSyncItem()
                            {
                                EpisodeUrl = podcastFeedItem.Address,
                                DestinationPath = destinationPath
                            };
                        episodesToDownload.Add(downloadItem);
                    }
                }
            }
        }
    }
}

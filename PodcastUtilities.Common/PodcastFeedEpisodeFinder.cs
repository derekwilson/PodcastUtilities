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
        private readonly IFileUtilities _fileUtilities;
        private readonly IPodcastFeedFactory _feedFactory;
        private readonly IWebClientFactory _webClientFactory;
        private readonly ITimeProvider _timeProvider;

        /// <summary>
        /// discover items to be downloaded from a feed
        /// </summary>
        public PodcastFeedEpisodeFinder(IFileUtilities fileFinder, IPodcastFeedFactory feedFactory, IWebClientFactory webClientFactory, ITimeProvider timeProvider)
        {
            _fileUtilities = fileFinder;
            _timeProvider = timeProvider;
            _webClientFactory = webClientFactory;
            _feedFactory = feedFactory;
        }

        /// <summary>
        /// event that is fired whenever a file is copied of an error occurs
        /// </summary>
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        /// <summary>
        /// Find episodes to download
        /// </summary>
        /// <param name="rootFolder">the folder into which we will download</param>
        /// <param name="feedInfo">feed information</param>
        /// <param name="episodesToDownload">list of items to download, will be added to</param>
        public void FindEpisodesToDownload(string rootFolder, FeedInfo feedInfo, IList<FeedSyncItem> episodesToDownload)
        {
            using (var webClient = _webClientFactory.GetWebClient())
            {
                var downloader = new PodcastFeedDownloader(webClient, _feedFactory);

                var feed = downloader.DownLoadFeed(feedInfo.Format, feedInfo.Address);
                feed.StatusUpdate += StatusUpdate;
                var episodes = feed.GetFeedEpisodes();

                var oldestEpisodeToAccept = DateTime.MinValue;
                if (feedInfo.MaximumDaysOld < int.MaxValue)
                {
                    oldestEpisodeToAccept = _timeProvider.UtcNow.AddDays(-feedInfo.MaximumDaysOld);
                }

                foreach (IPodcastFeedItem podcastFeedItem in episodes)
                {
                    if (podcastFeedItem.Published > oldestEpisodeToAccept)
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
}

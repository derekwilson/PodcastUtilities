using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// discover items to be downloaded from a feed
    /// </summary>
    public class PodcastFeedEpisodeFinder : IPodcastFeedEpisodeFinder
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

        private string GetDownloadPathname(string rootFolder, PodcastInfo podcastInfo, IPodcastFeedItem podcastFeedItem)
        {
            var proposedFilename = podcastFeedItem.GetFilename();

            switch (podcastInfo.Feed.NamingStyle)
            {
                case PodcastEpisodeNamingStyle.UrlFilenameAndPublishDateTime:
                    proposedFilename = string.Format("{0}_{1}",
                                                    podcastFeedItem.Published.ToString("yyyy_MM_dd_hhmm"),
                                                    proposedFilename);
                    break;
                case PodcastEpisodeNamingStyle.UrlFilenameFeedTitleAndPublishDateTime:
                    proposedFilename = string.Format("{0}_{1}_{2}",
                                                    podcastFeedItem.Published.ToString("yyyy_MM_dd_hhmm"),
                                                    podcastInfo.Folder,
                                                    proposedFilename);
                    break;
                case PodcastEpisodeNamingStyle.UrlFilename:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("NamingStyle");
            }

            return Path.Combine(Path.Combine(rootFolder, podcastInfo.Folder), proposedFilename);
        }

        /// <summary>
        /// Find episodes to download
        /// </summary>
        /// <param name="rootFolder">the root folder for all downloads</param>
        /// <param name="podcastInfo">info on the podcast to download</param>
        /// <param name="episodesToDownload">list of items to download, will be added to</param>
        public void FindEpisodesToDownload(string rootFolder, PodcastInfo podcastInfo, IList<IFeedSyncItem> episodesToDownload)
        {
            if (podcastInfo.Feed == null)
            {
                // it is optional to have a feed
                return;
            }
            using (var webClient = _webClientFactory.GetWebClient())
            {
                var downloader = new PodcastFeedDownloader(webClient, _feedFactory);

                var feed = downloader.DownLoadFeed(podcastInfo.Feed.Format, podcastInfo.Feed.Address);
                feed.StatusUpdate += StatusUpdate;
                var episodes = feed.GetFeedEpisodes();

                var oldestEpisodeToAccept = DateTime.MinValue;
                if (podcastInfo.Feed.MaximumDaysOld < int.MaxValue)
                {
                    oldestEpisodeToAccept = _timeProvider.UtcNow.AddDays(-podcastInfo.Feed.MaximumDaysOld);
                }

                foreach (IPodcastFeedItem podcastFeedItem in episodes)
                {
                    if (podcastFeedItem.Published > oldestEpisodeToAccept)
                    {
                        var destinationPath = GetDownloadPathname(rootFolder, podcastInfo, podcastFeedItem);
                        if (!_fileUtilities.FileExists(destinationPath))
                        {
                            var downloadItem = new FeedSyncItem()
                            {
                                EpisodeUrl = podcastFeedItem.Address,
                                DestinationPath = destinationPath,
                                EpisodeTitle = string.Format("{0} {1}", podcastInfo.Folder,podcastFeedItem.EpisodeTitle)
                            };
                            episodesToDownload.Add(downloadItem);
                            OnStatusMessageUpdate(string.Format("Queued: {0}, Episode: {1}", podcastInfo.Folder, podcastFeedItem.EpisodeTitle));
                        }
                        else
                        {
                            OnStatusVerbose(string.Format("Episode already downloaded: {0}", podcastFeedItem.EpisodeTitle));
                        }
                    }
                    else
                    {
                        OnStatusVerbose(string.Format("Episode too old: {0}",podcastFeedItem.EpisodeTitle));
                    }
                }
            }
        }

        private void OnStatusVerbose(string message)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Verbose, message));
        }

        private void OnStatusMessageUpdate(string message)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Status, message));
        }

        private void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            if (StatusUpdate != null)
                StatusUpdate(this, e);
        }
    }
}

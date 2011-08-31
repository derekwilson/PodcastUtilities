using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        private readonly IStateProvider _stateProvider;

        /// <summary>
        /// discover items to be downloaded from a feed
        /// </summary>
        public PodcastFeedEpisodeFinder(IFileUtilities fileFinder, IPodcastFeedFactory feedFactory, IWebClientFactory webClientFactory, ITimeProvider timeProvider, IStateProvider stateProvider)
        {
            _fileUtilities = fileFinder;
            _stateProvider = stateProvider;
            _timeProvider = timeProvider;
            _webClientFactory = webClientFactory;
            _feedFactory = feedFactory;
        }

        /// <summary>
        /// event that is fired whenever a file is copied of an error occurs
        /// </summary>
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        private static string GetDownloadPathname(string rootFolder, PodcastInfo podcastInfo, IPodcastFeedItem podcastFeedItem)
        {
            var proposedFilename = podcastFeedItem.GetFileName();

            switch (podcastInfo.Feed.NamingStyle)
            {
                case PodcastEpisodeNamingStyle.UrlFilenameAndPublishDateTime:
                    proposedFilename = string.Format("{0}_{1}",
                                                    podcastFeedItem.Published.ToString("yyyy_MM_dd_HHmm"),
                                                    proposedFilename);
                    break;
                case PodcastEpisodeNamingStyle.UrlFilenameFeedTitleAndPublishDateTime:
                    proposedFilename = string.Format("{0}_{1}_{2}",
                                                    podcastFeedItem.Published.ToString("yyyy_MM_dd_HHmm"),
                                                    podcastInfo.Folder,
                                                    proposedFilename);
                    break;
                case PodcastEpisodeNamingStyle.UrlFilenameFeedTitleAndPublishDateTimeInFolder:
                    proposedFilename = string.Format("{0}\\{1}_{2}_{3}",
                                                    podcastFeedItem.Published.ToString("yyyy_MM"),
                                                    podcastFeedItem.Published.ToString("yyyy_MM_dd_HHmm"),
                                                    podcastInfo.Folder,
                                                    proposedFilename);
                    break;
                case PodcastEpisodeNamingStyle.EpisodeTitle:
                    proposedFilename = podcastFeedItem.GetTitleAsFileName();
                    break;
                case PodcastEpisodeNamingStyle.EpisodeTitleAndPublishDateTime:
                    proposedFilename = string.Format("{0}_{1}",
                                                    podcastFeedItem.Published.ToString("yyyy_MM_dd_HHmm"),
                                                    podcastFeedItem.GetTitleAsFileName());
                    break;
                case PodcastEpisodeNamingStyle.UrlFilename:
                    break;
                default:
                    throw new IndexOutOfRangeException("NamingStyle");
            }

            return Path.Combine(Path.Combine(rootFolder, podcastInfo.Folder), proposedFilename);
        }

        private List<IFeedSyncItem> ApplyDownloadStrategy(string stateKey, PodcastInfo podcastInfo, List<IFeedSyncItem> episodesFound)
        {
            switch (podcastInfo.Feed.DownloadStrategy)
            {
                case PodcastEpisodeDownloadStrategy.All:
                    return episodesFound;

                case PodcastEpisodeDownloadStrategy.HighTide:
                    var state = _stateProvider.GetState(stateKey);
                    var newEpisodes =
                        (from episode in episodesFound
                         where episode.Published > state.DownloadHighTide
                         select episode);
                    var filteredEpisodes =  new List<IFeedSyncItem>(1);
                    filteredEpisodes.AddRange(newEpisodes);
                    return filteredEpisodes;

                case PodcastEpisodeDownloadStrategy.Latest:
                    episodesFound.Sort((e1, e2) => e2.Published.CompareTo(e1.Published));
                    var latestEpisodes =  new List<IFeedSyncItem>(1);
                    latestEpisodes.AddRange(episodesFound.Take(1));
                    return latestEpisodes;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        /// <summary>
        /// Find episodes to download
        /// </summary>
        /// <param name="rootFolder">the root folder for all downloads</param>
        /// <param name="retryWaitTimeInSeconds">time to wait if there is a file access lock</param>
        /// <param name="podcastInfo">info on the podcast to download</param>
        /// <returns>list of episodes to be downloaded for the supplied podcastInfo</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public IList<IFeedSyncItem> FindEpisodesToDownload(string rootFolder, int retryWaitTimeInSeconds, PodcastInfo podcastInfo)
        {
            List<IFeedSyncItem> episodesToDownload = new List<IFeedSyncItem>(10);
            if (podcastInfo.Feed == null)
            {
                // it is optional to have a feed
                return episodesToDownload;
            }

            var stateKey = Path.Combine(rootFolder, podcastInfo.Folder);

            using (var webClient = _webClientFactory.GetWebClient())
            {
                var downloader = new PodcastFeedDownloader(webClient, _feedFactory);

                try
                {
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
                                                           StateKey = stateKey,
                                                           RetryWaitTimeInSeconds = retryWaitTimeInSeconds,
                                                           Published = podcastFeedItem.Published,
                                                           EpisodeUrl = podcastFeedItem.Address,
                                                           DestinationPath = destinationPath,
                                                           EpisodeTitle = string.Format("{0} {1}", podcastInfo.Folder,podcastFeedItem.EpisodeTitle)
                                                       };
                                episodesToDownload.Add(downloadItem);
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
                catch (Exception e)
                {
                    OnStatusError(string.Format("Error processing feed {0}: {1}", podcastInfo.Feed.Address, e.Message));
                }
            }

            var filteredEpisodes = ApplyDownloadStrategy(stateKey, podcastInfo, episodesToDownload);
            foreach (var filteredEpisode in filteredEpisodes)
            {
                OnStatusMessageUpdate(string.Format("Queued: {0}", filteredEpisode.EpisodeTitle));
            }
            return filteredEpisodes;
        }

        private void OnStatusVerbose(string message)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Verbose, message));
        }

        private void OnStatusMessageUpdate(string message)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Status, message));
        }

        private void OnStatusError(string message)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Error, message));
        }

        private void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            if (StatusUpdate != null)
                StatusUpdate(this, e);
        }
    }
}

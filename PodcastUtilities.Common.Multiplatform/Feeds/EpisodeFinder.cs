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
using System.Linq;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Exceptions;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// discover items to be downloaded from a feed
    /// </summary>
    public class EpisodeFinder : IEpisodeFinder
    {
        private readonly IDirectoryInfoProvider _directoryInfoProvider;
        private readonly IFileUtilities _fileUtilities;
        private readonly IPodcastFeedFactory _feedFactory;
        private readonly IWebClientFactory _webClientFactory;
        private readonly ITimeProvider _timeProvider;
        private readonly IStateProvider _stateProvider;
        private readonly ICommandGenerator _commandGenerator;
        private readonly IPathUtilities _pathUtilities;

        /// <summary>
        /// discover items to be downloaded from a feed
        /// </summary>
        public EpisodeFinder(IFileUtilities fileFinder, IPodcastFeedFactory feedFactory, IWebClientFactory webClientFactory, ITimeProvider timeProvider, IStateProvider stateProvider, IDirectoryInfoProvider directoryInfoProvider, ICommandGenerator commandGenerator, IPathUtilities pathUtilities)
        {
            _fileUtilities = fileFinder;
            _commandGenerator = commandGenerator;
            _directoryInfoProvider = directoryInfoProvider;
            _stateProvider = stateProvider;
            _timeProvider = timeProvider;
            _webClientFactory = webClientFactory;
            _feedFactory = feedFactory;
            _pathUtilities = pathUtilities;
        }

        /// <summary>
        /// event that is fired whenever a file is copied of an error occurs
        /// </summary>
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        private string GetDownloadPathname(string rootFolder, PodcastInfo podcastInfo, IPodcastFeedItem podcastFeedItem)
        {
            var proposedFilename = podcastFeedItem.FileName;

            switch (podcastInfo.Feed.NamingStyle.Value)
            {
                case PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime:
                    proposedFilename = string.Format(CultureInfo.InvariantCulture, "{0}_{1}",
                                                    podcastFeedItem.Published.ToString("yyyy_MM_dd_HHmm",CultureInfo.InvariantCulture),
                                                    proposedFilename);
                    break;
                case PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTime:
                    proposedFilename = string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}",
                                                    podcastFeedItem.Published.ToString("yyyy_MM_dd_HHmm",CultureInfo.InvariantCulture),
                                                    podcastInfo.Folder,
                                                    proposedFilename);
                    break;
                case PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTimeInfolder:
                    proposedFilename = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}_{3}_{4}",
                                                    podcastFeedItem.Published.ToString("yyyy_MM",CultureInfo.InvariantCulture),
                                                    _pathUtilities.GetPathSeparator(),
                                                    podcastFeedItem.Published.ToString("yyyy_MM_dd_HHmm",CultureInfo.InvariantCulture),
                                                    podcastInfo.Folder,
                                                    proposedFilename);
                    break;
                case PodcastEpisodeNamingStyle.EpisodeTitle:
                    proposedFilename = podcastFeedItem.TitleAsFileName;
                    break;
                case PodcastEpisodeNamingStyle.EpisodeTitleAndPublishDateTime:
                    proposedFilename = string.Format(CultureInfo.InvariantCulture, "{0}_{1}",
                                                    podcastFeedItem.Published.ToString("yyyy_MM_dd_HHmm",CultureInfo.InvariantCulture),
                                                    podcastFeedItem.TitleAsFileName);
                    break;
                case PodcastEpisodeNamingStyle.UrlFileName:
                    break;
                default:
                    throw new EnumOutOfRangeException("NamingStyle");
            }

            return Path.Combine(Path.Combine(rootFolder, podcastInfo.Folder), proposedFilename);
        }

        private List<ISyncItem> ApplyDownloadStrategy(string stateKey, PodcastInfo podcastInfo, List<ISyncItem> episodesFound)
        {
            switch (podcastInfo.Feed.DownloadStrategy.Value)
            {
                case PodcastEpisodeDownloadStrategy.All:
                    return episodesFound;

                case PodcastEpisodeDownloadStrategy.HighTide:
                    var state = _stateProvider.GetState(stateKey);
                    var newEpisodes =
                        (from episode in episodesFound
                         where episode.Published > state.DownloadHighTide
                         select episode);
                    var filteredEpisodes =  new List<ISyncItem>(1);
                    filteredEpisodes.AddRange(newEpisodes);
                    return filteredEpisodes;

                case PodcastEpisodeDownloadStrategy.Latest:
                    episodesFound.Sort((e1, e2) => e2.Published.CompareTo(e1.Published));
                    var latestEpisodes =  new List<ISyncItem>(1);
                    latestEpisodes.AddRange(episodesFound.Take(1));
                    return latestEpisodes;

                default:
                    throw new EnumOutOfRangeException();
            }
        }

        private void CreateFolderIfNeeded(string folder)
        {
            IDirectoryInfo dir = _directoryInfoProvider.GetDirectoryInfo(folder);
            if (!dir.Exists)
            {
                dir.Create();
            }
        }

        /// <summary>
        /// Find episodes to download
        /// </summary>
        /// <param name="rootFolder">the root folder for all downloads</param>
        /// <param name="retryWaitTimeInSeconds">time to wait if there is a file access lock</param>
        /// <param name="podcastInfo">info on the podcast to download</param>
        /// <param name="retainFeedStream">true to keep the downloaded stream</param>
        /// <returns>list of episodes to be downloaded for the supplied podcastInfo</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public IList<ISyncItem> FindEpisodesToDownload(string rootFolder, int retryWaitTimeInSeconds, PodcastInfo podcastInfo, bool retainFeedStream)
        {
            List<ISyncItem> episodesToDownload = new List<ISyncItem>(10);
            if (podcastInfo.Feed == null)
            {
                // it is optional to have a feed
                return episodesToDownload;
            }

            var stateKey = Path.Combine(rootFolder, podcastInfo.Folder);
            string feedSaveFile = null;
            if (retainFeedStream)
            {
                CreateFolderIfNeeded(stateKey);
                feedSaveFile = Path.Combine(Path.Combine(rootFolder, podcastInfo.Folder), "last_download_feed.xml");
            }

            using (var webClient = _webClientFactory.CreateWebClient())
            {
                var downloader = new Downloader(webClient, _feedFactory);

                try
                {
                    var feed = downloader.DownloadFeed(podcastInfo.Feed.Format.Value, podcastInfo.Feed.Address,feedSaveFile);
                    feed.StatusUpdate += StatusUpdate;
                    var episodes = feed.Episodes;

                    var oldestEpisodeToAccept = DateTime.MinValue;
                    if (podcastInfo.Feed.MaximumDaysOld.Value < int.MaxValue)
                    {
                        oldestEpisodeToAccept = _timeProvider.UtcNow.AddDays(-podcastInfo.Feed.MaximumDaysOld.Value);
                    }

                    foreach (IPodcastFeedItem podcastFeedItem in episodes)
                    {
                        if (podcastFeedItem.Published > oldestEpisodeToAccept)
                        {
                            var destinationPath = GetDownloadPathname(rootFolder, podcastInfo, podcastFeedItem);
                            if (!_fileUtilities.FileExists(destinationPath))
                            {
                                var downloadItem = new SyncItem()
                                                       {
                                                           Id = Guid.NewGuid(), 
                                                           StateKey = stateKey,
                                                           RetryWaitTimeInSeconds = retryWaitTimeInSeconds,
                                                           Published = podcastFeedItem.Published,
                                                           EpisodeUrl = podcastFeedItem.Address,
                                                           DestinationPath = destinationPath,
                                                           EpisodeTitle = string.Format(CultureInfo.InvariantCulture, "{0} {1}", podcastInfo.Folder, podcastFeedItem.EpisodeTitle),
                                                           PostDownloadCommand = _commandGenerator.ReplaceTokensInCommand(podcastInfo.PostDownloadCommand,rootFolder,destinationPath,podcastInfo),
                                                       };
                                episodesToDownload.Add(downloadItem);
                            }
                            else
                            {
                                OnStatusVerbose(string.Format(CultureInfo.InvariantCulture, "Episode already downloaded: {0}", podcastFeedItem.EpisodeTitle), podcastInfo);
                            }
                        }
                        else
                        {
                            OnStatusVerbose(string.Format(CultureInfo.InvariantCulture, "Episode too old: {0}", podcastFeedItem.EpisodeTitle), podcastInfo);
                        }
                    }
                }
                catch (Exception e)
                {
                    OnStatusError(string.Format(CultureInfo.InvariantCulture, "Error processing feed {0}: {1}", podcastInfo.Feed.Address, e.Message), podcastInfo);
                }
            }

            var filteredEpisodes = ApplyDownloadStrategy(stateKey, podcastInfo, episodesToDownload);
            foreach (var filteredEpisode in filteredEpisodes)
            {
                OnStatusMessageUpdate(string.Format(CultureInfo.InvariantCulture, "Queued: {0}", filteredEpisode.EpisodeTitle), podcastInfo);
            }
            return filteredEpisodes;
        }

        private void OnStatusVerbose(string message, PodcastInfo podcastInfo)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateLevel.Verbose, message, false, podcastInfo));
        }

        private void OnStatusMessageUpdate(string message, PodcastInfo podcastInfo)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateLevel.Status, message, false, podcastInfo));
        }

        private void OnStatusError(string message, PodcastInfo podcastInfo)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateLevel.Error, message, false, podcastInfo));
        }

        private void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            StatusUpdate?.Invoke(this, e);
        }
    }
}

﻿using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// factory for a downloader task
    /// </summary>
    public class EpisodeDownloaderFactory : IEpisodeDownloaderFactory
    {
        private readonly IWebClientFactory _webClientFactory;
        private readonly IDirectoryInfoProvider _directoryInfoProvider;
        private readonly IFileUtilities _fileUtilities;
        private readonly IStateProvider _stateProvider;

        /// <summary>
        /// construct the factory
        /// </summary>
        public EpisodeDownloaderFactory(IWebClientFactory webClientFactory, IDirectoryInfoProvider directoryInfoProvider, IFileUtilities fileUtilities, IStateProvider stateProvider)
        {
            _webClientFactory = webClientFactory;
            _stateProvider = stateProvider;
            _fileUtilities = fileUtilities;
            _directoryInfoProvider = directoryInfoProvider;
        }

        /// <summary>
        /// create an episode downloader task
        /// </summary>
        /// <returns></returns>
        public IEpisodeDownloader CreateDownloader()
        {
            return new EpisodeDownloader(_webClientFactory,_directoryInfoProvider,_fileUtilities, _stateProvider);
        }
    }
}
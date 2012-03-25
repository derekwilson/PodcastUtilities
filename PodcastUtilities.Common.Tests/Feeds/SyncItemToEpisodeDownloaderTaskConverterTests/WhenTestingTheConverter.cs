using System.Collections.Generic;
using PodcastUtilities.Common.Feeds;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Tests.Feeds.SyncItemToEpisodeDownloaderTaskConverterTests
{
    public abstract class WhenTestingTheConverter
        : WhenTestingBehaviour
    {
        protected SyncItemToEpisodeDownloaderTaskConverter _converter;

        protected IWebClientFactory _webClientFactory;
        protected IDirectoryInfoProvider _directoryInfoProvider;
        protected IFileUtilities _fileUtilities;
        protected IStateProvider _stateProvider;
        protected ICounterFactory _counterFactory;

        protected IEpisodeDownloaderFactory _downloaderFactory;

        protected List<ISyncItem> _downloadItems;
        protected IEpisodeDownloader[] _tasks;

        protected override void GivenThat()
        {
            base.GivenThat();

            _counterFactory = GenerateMock<ICounterFactory>();
            _stateProvider = GenerateMock<IStateProvider>();
            _webClientFactory = GenerateMock<IWebClientFactory>();
            _downloaderFactory = new EpisodeDownloaderFactory(_webClientFactory,_directoryInfoProvider,_fileUtilities,_stateProvider,_counterFactory);
            _downloadItems = new List<ISyncItem>(10);

            SetupData();
            SetupStubs();

            _converter = new SyncItemToEpisodeDownloaderTaskConverter(_downloaderFactory);
        }

        protected virtual void SetupData()
        {
        }

        protected virtual void SetupStubs()
        {

        }
    }
}

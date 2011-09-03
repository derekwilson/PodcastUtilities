using System.Collections.Generic;
using System.Linq;
using System.Text;
using PodcastUtilities.Common.Feeds;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.FeedSyncItemToPodcastEpisodeDownloaderTaskConverterTests
{
    public abstract class WhenTestingTheConverter
        : WhenTestingBehaviour
    {
        protected SyncItemToEpisodeDownloaderTaskConverter _converter;

        protected IWebClientFactory _webClientFactory;
        protected IDirectoryInfoProvider _directoryInfoProvider;
        protected IFileUtilities _fileUtilities;
        protected IStateProvider _stateProvider;

        protected IEpisodeDownloaderFactory _downloaderFactory;

        protected List<ISyncItem> _downloadItems;
        protected IEpisodeDownloader[] _tasks;

        protected override void GivenThat()
        {
            base.GivenThat();

            _stateProvider = GenerateMock<IStateProvider>();
            _webClientFactory = GenerateMock<IWebClientFactory>();
            _downloaderFactory = new EpisodeDownloaderFactory(_webClientFactory,_directoryInfoProvider,_fileUtilities,_stateProvider);
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

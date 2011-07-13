using System.Collections.Generic;
using System.Linq;
using System.Text;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.FeedSyncItemToPodcastEpisodeDownloaderTaskConverterTests
{
    public abstract class WhenTestingTheConverter
        : WhenTestingBehaviour
    {
        protected FeedSyncItemToPodcastEpisodeDownloaderTaskConverter _converter;

        protected IWebClientFactory _webClientFactory;
        protected IDirectoryInfoProvider _directoryInfoProvider;
        protected IFileUtilities _fileUtilities;
        protected IStateProvider _stateProvider;

        protected IPodcastEpisodeDownloaderFactory _downloaderFactory;

        protected List<IFeedSyncItem> _downloadItems;
        protected IPodcastEpisodeDownloader[] _tasks;

        protected override void GivenThat()
        {
            base.GivenThat();

            _stateProvider = GenerateMock<IStateProvider>();
            _webClientFactory = GenerateMock<IWebClientFactory>();
            _downloaderFactory = new PodcastEpisodeDownloaderFactory(_webClientFactory,_directoryInfoProvider,_fileUtilities,_stateProvider);
            _downloadItems = new List<IFeedSyncItem>(10);

            SetupData();
            SetupStubs();

            _converter = new FeedSyncItemToPodcastEpisodeDownloaderTaskConverter(_downloaderFactory);
        }

        protected virtual void SetupData()
        {
        }

        protected virtual void SetupStubs()
        {

        }
    }
}

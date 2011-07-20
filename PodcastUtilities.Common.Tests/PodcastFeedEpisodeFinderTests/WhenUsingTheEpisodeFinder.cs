using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastFeedEpisodeFinderTests
{
    public abstract class WhenUsingTheEpisodeFinder
        : WhenTestingBehaviour
    {
        protected PodcastFeedEpisodeFinder _episodeFinder;

        protected IWebClientFactory _webClientFactory;
        protected IWebClient _webClient;
        protected IPodcastFeedFactory _feedFactory;
        protected IFileUtilities _fileUtilities;
        protected ITimeProvider _timeProvider;
        protected IStateProvider _stateProvider;
        protected IState _state;

        protected string _rootFolder;
        protected int _retryWaitTime;
        protected PodcastInfo _podcastInfo;
        protected FeedInfo _feedInfo;
        protected IList<IFeedSyncItem> _episodesToSync;
        protected string _feedAddress;
        protected MemoryStream _stream;
        protected IPodcastFeed _podcastFeed;
        protected IList<IPodcastFeedItem> _podcastFeedItems;

        protected DateTime _now;

        protected StatusUpdateEventArgs _latestUpdate;

        protected override void GivenThat()
        {
            base.GivenThat();

            _stateProvider = GenerateMock<IStateProvider>();
            _state = GenerateMock<IState>();
            _timeProvider = GenerateMock<ITimeProvider>();
            _webClientFactory = GenerateMock<IWebClientFactory>();
            _webClient = GenerateMock<IWebClient>();
            _feedFactory = GenerateMock<IPodcastFeedFactory>();
            _fileUtilities = GenerateMock<IFileUtilities>();
            _podcastFeed = GenerateMock<IPodcastFeed>();

            SetupData();
            SetupStubs();

            _episodeFinder = new PodcastFeedEpisodeFinder(_fileUtilities,_feedFactory,_webClientFactory,_timeProvider,_stateProvider);
            _episodeFinder.StatusUpdate += new EventHandler<StatusUpdateEventArgs>(EpisodeFinderStatusUpdate);
            _latestUpdate = null;
        }

        void EpisodeFinderStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            _latestUpdate = e;
        }

        protected virtual void SetupData()
        {
            _now = new DateTime(2010,5,1,16,11,12);

            _feedAddress = "http://test";

            _feedInfo = new FeedInfo();
            _feedInfo.Format = PodcastFeedFormat.RSS;
            _feedInfo.NamingStyle = PodcastEpisodeNamingStyle.UrlFilename;
            _feedInfo.Address = new Uri(_feedAddress);
            _feedInfo.MaximumDaysOld = int.MaxValue;
            _feedInfo.DownloadStrategy = PodcastEpisodeDownloadStrategy.All;

            _retryWaitTime = 13;
            _rootFolder = "c:\\TestRoot";
            _podcastInfo = new PodcastInfo();
            _podcastInfo.Folder = "TestFolder";
            _podcastInfo.Feed = _feedInfo;

            _podcastFeedItems = new List<IPodcastFeedItem>(10);
        }

        protected virtual void SetupStubs()
        {
            SetupStubs(false);
        }

        protected virtual void SetupStubs(bool throwErrorFromFeed)
        {
            _timeProvider.Stub(time => time.UtcNow).Return(_now);
            _webClient.Stub(client => client.OpenRead(_feedInfo.Address)).Return(_stream);
            _webClientFactory.Stub(factory => factory.GetWebClient()).Return(_webClient);
            _feedFactory.Stub(factory => factory.CreatePodcastFeed(_feedInfo.Format, _stream)).Return(_podcastFeed);
            if (throwErrorFromFeed)
            {
                _podcastFeed.Stub(feed => feed.GetFeedEpisodes()).Throw(new Exception("ERROR"));
            }
            else
            {
                _podcastFeed.Stub(feed => feed.GetFeedEpisodes()).Return(_podcastFeedItems);
            }
            _stateProvider.Stub(provider => provider.GetState(Path.Combine(_rootFolder, _podcastInfo.Folder))).Return(_state);
        }
    }
}

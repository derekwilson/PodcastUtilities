﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PodcastUtilities.Common.IO;
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

        protected string _rootFolder;
        protected FeedInfo _feedInfo;
        protected IList<FeedSyncItem> _episodesToSync;
        protected string _feedAddress;
        protected MemoryStream _stream;
        protected IPodcastFeed _podcastFeed;
        protected IList<IPodcastFeedItem> _podcastFeedItems;

        protected DateTime _now;

        protected override void GivenThat()
        {
            base.GivenThat();

            _webClientFactory = GenerateMock<IWebClientFactory>();
            _webClient = GenerateMock<IWebClient>();
            _feedFactory = GenerateMock<IPodcastFeedFactory>();
            _fileUtilities = GenerateMock<IFileUtilities>();
            _podcastFeed = GenerateMock<IPodcastFeed>();

            SetupData();
            SetupStubs();

            _episodeFinder = new PodcastFeedEpisodeFinder(_fileUtilities,_feedFactory,_webClientFactory);
        }

        protected virtual void SetupData()
        {
            _now = new DateTime(2010,5,1,10,11,12);

            _rootFolder = "c:\\TestFeed";
            _feedAddress = "http://test";

            _feedInfo = new FeedInfo();
            _feedInfo.Format = PodcastFeedFormat.RSS;
            _feedInfo.Address = new Uri(_feedAddress);

            _podcastFeedItems = new List<IPodcastFeedItem>(10);
            _episodesToSync = new List<FeedSyncItem>(10);
        }

        protected virtual void SetupStubs()
        {
            _webClient.Stub(client => client.OpenRead(_feedInfo.Address)).Return(_stream);
            _webClientFactory.Stub(factory => factory.GetWebClient()).Return(_webClient);
            _feedFactory.Stub(factory => factory.CreatePodcastFeed(_feedInfo.Format, _stream)).Return(_podcastFeed);
            _podcastFeed.Stub(feed => feed.GetFeedEpisodes()).Return(_podcastFeedItems);
        }
    }
}
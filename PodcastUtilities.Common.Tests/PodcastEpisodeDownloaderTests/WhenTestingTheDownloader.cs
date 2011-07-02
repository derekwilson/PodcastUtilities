using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests
{
    public abstract class WhenTestingTheDownloader
        : WhenTestingBehaviour
    {
        protected PodcastEpisodeDownloader _downloader;

        protected IWebClientFactory _webClientFactory;
        protected IWebClient _webClient;

        protected IFeedSyncItem _syncItem;

        protected MemoryStream _stream;

        protected System.Exception _exception;

        protected StatusUpdateEventArgs _statusUpdateArgs;

        protected override void GivenThat()
        {
            base.GivenThat();

            _webClientFactory = GenerateMock<IWebClientFactory>();
            _webClient = GenerateMock<IWebClient>();

            _syncItem = new FeedSyncItem();
            _exception = null;

            SetupData();
            SetupStubs();

            _downloader = new PodcastEpisodeDownloader(_webClientFactory);
            _downloader.StatusUpdate += new EventHandler<StatusUpdateEventArgs>(DownloaderStatusUpdate);
        }

        protected virtual void DownloaderStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            _statusUpdateArgs = e;
        }

        protected virtual void SetupData()
        {
            _syncItem.EpisodeUrl = new Uri("http://test");
            _syncItem.DestinationPath = "c:\\folder\\file.ext";
            _syncItem.EpisodeTitle = "title";
        }

        protected virtual void SetupStubs()
        {
            _webClient.Stub(client => client.OpenRead(_syncItem.EpisodeUrl)).Return(_stream);
            _webClientFactory.Stub(factory => factory.GetWebClient()).Return(_webClient);
        }
    }
}

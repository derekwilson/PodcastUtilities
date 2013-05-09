using System;
using System.IO;
using PodcastUtilities.Common.Feeds;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Feeds.DownloaderTests
{
    public abstract class WhenTestingTheDownloader
        : WhenTestingBehaviour
    {
        protected Downloader FeedDownloader { get; set; }
        protected IWebClient WebClient { get; set; }
        protected IPodcastFeedFactory FeedFactory { get; set; }
        protected Uri Address { get; set; }

        protected IPodcastFeed Feed { get; set; }
        protected Stream StreamData { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Address = new Uri("http://localhost/fred");
            WebClient = GenerateMock<IWebClient>();
            FeedFactory = GenerateMock<IPodcastFeedFactory>();
            FeedDownloader = new Downloader(WebClient,FeedFactory);

            StreamData = new MemoryStream();

            WebClient.Stub(client => client.OpenRead(Address)).Return(StreamData);
        }
    }
}

using System;
using System.IO;
using System.Reflection;
using PodcastUtilities.Common.Feeds;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Feeds.PodcastFeedInRssFormatTests
{
    public abstract class WhenTestingTheFeed
        : WhenTestingBehaviour
    {
        protected PodcastFeedInRssFormat Feed { get; set; }
        protected IWebClient WebClient { get; set; }
        protected Uri Address { get; set; }
        protected string FeedXmlResourcePath { get; set; }
        protected Stream FeedXmlStream { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            WebClient = GenerateMock<IWebClient>();

            CreateData();
            CreateStubs();
        }

        protected virtual void CreateData()
        {
            FeedXmlResourcePath = "PodcastUtilities.Common.Tests.XML.testrssfeed.xml";
            FeedXmlStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(FeedXmlResourcePath);
        }

        protected virtual void CreateStubs()
        {
            WebClient.Stub(client => client.OpenRead(Address)).Return(FeedXmlStream);
        }
    }
}

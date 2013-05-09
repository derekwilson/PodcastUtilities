using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.Common.Tests.Feeds.PodcastFeedInRssFormatTests
{
    class WhenLoadingALargeExmapleFile
        : WhenTestingTheFeed
    {
        private IList<IPodcastFeedItem> _episodes;

        protected override void GivenThat()
        {
            base.GivenThat();
            Feed = new PodcastFeedInRssFormat(FeedXmlStream,null);
        }

        protected override void CreateData()
        {
            base.CreateData();
            FeedXmlResourcePath = "PodcastUtilities.Common.Tests.XML.testbigrssfeed.xml";
            FeedXmlStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(FeedXmlResourcePath);
        }

        protected override void When()
        {
            _episodes = Feed.Episodes;
        }

        [Test]
        public void ItShouldLoadTheEpisodes()
        {
            Assert.That(_episodes.Count, Is.EqualTo(275));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.PodcastFeedInRssFormatTests
{
    class WhenLoadingALargeExmapleFile
        : WhenTestingTheFeed
    {
        private IList<IPodcastFeedItem> _episodes;

        protected override void GivenThat()
        {
            base.GivenThat();
            Feed = new PodcastFeedInRssFormat(FeedXmlStream);
        }

        protected override void CreateData()
        {
            base.CreateData();
            FeedXmlResourcePath = "PodcastUtilities.Common.Tests.XML.testbigrssfeed.xml";
            FeedXmlStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(FeedXmlResourcePath);
        }

        protected override void When()
        {
            _episodes = Feed.GetFeedEpisodes();
        }

        [Test]
        public void ItShouldLoadTheEpisodes()
        {
            Assert.That(_episodes.Count, Is.EqualTo(275));
        }
    }
}

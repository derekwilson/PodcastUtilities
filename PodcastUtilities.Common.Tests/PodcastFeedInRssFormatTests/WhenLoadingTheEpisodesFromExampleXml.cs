using System.Collections.Generic;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.PodcastFeedInRssFormatTests
{
    public class WhenLoadingTheEpisodesFromExampleXml : WhenTestingTheFeed
    {
        private List<IPodcastFeedItem> _episodes;

        protected override void GivenThat()
        {
            base.GivenThat();
            Feed = new PodcastFeedInRssFormat(FeedXmlStream);
        }

        protected override void When()
        {
            _episodes = Feed.GetFeedEpisodes();
        }

        [Test]
        public void ItShouldLoadTheEpisodes()
        {
            Assert.That(_episodes.Count, Is.EqualTo(15));
        }
    }
}
using NUnit.Framework;
using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.Common.Tests.PodcastFeedInRssFormatTests
{
    public class WhenConstructingAFeedFromExampleXml : WhenTestingTheFeed
    {
        protected override void When()
        {
            Feed = new PodcastFeedInRssFormat(FeedXmlStream);
        }

        [Test]
        public void ItShouldGetTheCorrectTitle()
        {
            Assert.That(Feed.PodcastTitle, Is.EqualTo("This Developer's Life"));
        }
    }
}
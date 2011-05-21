using NUnit.Framework;

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
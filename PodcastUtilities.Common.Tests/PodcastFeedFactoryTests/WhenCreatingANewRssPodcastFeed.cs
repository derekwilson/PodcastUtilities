using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.PodcastFeedFactoryTests
{
    public class WhenCreatingANewRssPodcastFeed : WhenCreatingANewPodcastFeed
    {
        protected override void When()
        {
            Feed = FeedFactory.CreatePodcastFeed(PodcastFeedFormat.RSS, FeedData);
        }

        [Test]
        public void ItShouldReturnTheCorrectObject()
        {
            Assert.That(Feed, Is.InstanceOf<PodcastFeedInRssFormat>());
        }
    }
}
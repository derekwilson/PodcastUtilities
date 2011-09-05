using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Feeds.DownloaderTests
{
    public class WhenTestingTheDownloaderInRss : WhenTestingTheDownloader
    {
        protected override void When()
        {
            Feed = FeedDownloader.DownloadFeed(PodcastFeedFormat.RSS,Address);
        }

        [Test]
        public void ItShouldDownloadTheFeed()
        {
            WebClient.AssertWasCalled(c => c.OpenRead(Address));
        }

        [Test]
        public void ItShouldReturnAFeed()
        {
            FeedFactory.AssertWasCalled(f => f.CreatePodcastFeed(PodcastFeedFormat.RSS, StreamData));
        }
    }
}
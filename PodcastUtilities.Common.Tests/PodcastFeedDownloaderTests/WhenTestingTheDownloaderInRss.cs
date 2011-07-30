using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastFeedDownloaderTests
{
    public class WhenTestingTheDownloaderInRss : WhenTestingTheDownloader
    {
        protected override void When()
        {
            Feed = FeedDownloader.DownLoadFeed(PodcastFeedFormat.RSS,Address);
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
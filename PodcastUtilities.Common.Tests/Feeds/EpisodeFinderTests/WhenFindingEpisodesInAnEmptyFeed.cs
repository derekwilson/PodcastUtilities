using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.Feeds.EpisodeFinderTests
{
    public class WhenFindingEpisodesInAnEmptyFeed : WhenUsingTheEpisodeFinder
    {
        protected override void When()
        {
            _episodesToSync = _episodeFinder.FindEpisodesToDownload(_rootFolder,_retryWaitTime, _podcastInfo,_retainFeedXml);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            Assert.That(_episodesToSync.Count, Is.EqualTo(0));
        }
    }
}
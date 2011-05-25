using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.PodcastFeedEpisodeFinderTests
{
    public class WhenFindingEpisodesInAnEmptyFeed : WhenUsingTheEpisodeFinder
    {
        protected override void When()
        {
            _episodeFinder.FindEpisodesToDownload(_rootFolder, _feedInfo, _episodesToSync);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            Assert.That(_episodesToSync.Count, Is.EqualTo(0));
        }
    }
}
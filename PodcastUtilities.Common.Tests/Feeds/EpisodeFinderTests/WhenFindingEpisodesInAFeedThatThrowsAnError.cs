using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.Feeds.EpisodeFinderTests
{
    public class WhenFindingEpisodesInAFeedThatThrowsAnError : WhenUsingTheEpisodeFinder
    {
        protected override void SetupStubs()
        {
            base.SetupStubs(true);
        }

        protected override void When()
        {
            _episodesToSync = _episodeFinder.FindEpisodesToDownload(_rootFolder,_retryWaitTime, _podcastInfo);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            Assert.That(_episodesToSync.Count, Is.EqualTo(0));
        }

        [Test]
        public void ItShouldUpdateTheStatus()
        {
            Assert.That(_latestUpdate.MessageLevel, Is.EqualTo(StatusUpdateLevel.Error));
        }
    }
}
using NUnit.Framework;
using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests
{
    public class WhenTestingTheInitialState : WhenTestingTheDownloader
    {
        protected override void When()
        {
            _downloader = new EpisodeDownloader(_webClientFactory,_directoryInfoProvider,_fileUtilities,_stateProvider);
        }

        [Test]
        public void ItShouldSetTheName()
        {
            // we do not care what the default name is, only that it has one
            Assert.That(_downloader.GetName().Length, Is.GreaterThan(0));
        }

        [Test]
        public void ItShouldSetState()
        {
            Assert.That(_downloader.IsStarted(), Is.False);
            Assert.That(_downloader.IsComplete(), Is.False);
        }
    }
}
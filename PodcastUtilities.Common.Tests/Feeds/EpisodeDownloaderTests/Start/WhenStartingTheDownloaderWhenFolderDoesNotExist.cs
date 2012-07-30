using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Feeds.EpisodeDownloaderTests.Start
{
    public class WhenStartingTheDownloaderWhenFolderDoesNotExist : WhenTestingTheDownloader
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _downloader.SyncItem = _syncItem;
        }

        protected override void SetupStubs()
        {
            base.SetupStubs();
            _directoryInfo.Stub(dir => dir.Exists).Return(false);
        }
        
        protected override void When()
        {
            _downloader.Start(null);
        }

        [Test]
        public void ItShouldStart()
        {
            Assert.That(_downloader.IsStarted(), Is.True);
            Assert.That(_downloader.IsComplete(), Is.False);
        }

        [Test]
        public void ItShouldSetTheTitle()
        {
            Assert.That(_downloader.Name, Is.EqualTo("title"));
        }

        [Test]
        public void ItShouldCreateTheFolder()
        {
            _directoryInfo.AssertWasCalled(dir => dir.Create());
        }
    }
}
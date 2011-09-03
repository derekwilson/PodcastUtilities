using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.Files.PodcastEpisodePurgerTests
{
    public class WhenUsingTheEpisodePurgerToPurgeFilesWhenPurgingIsNotSelected : WhenUsingTheEpisodePurger
    {
        protected override void SetupData()
        {
            base.SetupData();

            _feedInfo.DeleteDownloadsDaysOld = int.MaxValue;
        }

        protected override void SetupStubs()
        {
            StubFiles();

            base.SetupStubs();
        }

        protected override void When()
        {
            _episodesToDelete = _episodePurger.FindEpisodesToPurge(_rootFolder, _podcastInfo);
        }

        [Test]
        public void ItShouldReturnTheCorrectFiles()
        {
            Assert.AreEqual(0, _episodesToDelete.Count);
        }
    }
}
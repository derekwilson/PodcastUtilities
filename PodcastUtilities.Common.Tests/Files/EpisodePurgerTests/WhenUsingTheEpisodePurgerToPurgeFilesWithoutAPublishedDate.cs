using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Files.PodcastEpisodePurgerTests
{
    public class WhenUsingTheEpisodePurgerToPurgeFilesWithoutAPublishedDate : WhenUsingTheEpisodePurger
    {
        protected override void SetupData()
        {
            base.SetupData();

            _feedInfo.DeleteDownloadsDaysOld = 5;
            _feedInfo.NamingStyle = PodcastEpisodeNamingStyle.UrlFilename;
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
            Assert.AreEqual(2, _episodesToDelete.Count);
            Assert.AreEqual(_downloadedFiles[2], _episodesToDelete[0], "incorrect file selected");
            Assert.AreEqual(_downloadedFiles[3], _episodesToDelete[1], "incorrect file selected");
        }
    }
}
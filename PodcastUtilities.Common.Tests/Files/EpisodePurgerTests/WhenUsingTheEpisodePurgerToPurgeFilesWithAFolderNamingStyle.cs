using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Files.PodcastEpisodePurgerTests
{
    public class WhenUsingTheEpisodePurgerToPurgeFilesWithAFolderNamingStyle : WhenUsingTheEpisodePurger
    {
        protected override void SetupData()
        {
            base.SetupData();

            _feedInfo.DeleteDownloadsDaysOld.Value = 5;
            _feedInfo.NamingStyle.Value = PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTimeInfolder;
        }

        protected override void SetupStubs()
        {
            StubSubFolders();
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
            Assert.AreEqual(4, _episodesToDelete.Count);
            Assert.AreEqual(_downloadedFiles[2], _episodesToDelete[0], "incorrect file selected");
            Assert.AreEqual(_downloadedFiles[3], _episodesToDelete[1], "incorrect file selected");
            Assert.AreEqual(_downloadedFiles[2], _episodesToDelete[2], "incorrect file selected");
            Assert.AreEqual(_downloadedFiles[3], _episodesToDelete[3], "incorrect file selected");
        }
    }
}
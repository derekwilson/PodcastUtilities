using System;
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastFeedEpisodeFinderTests
{
    public class WhenFindingEpisodesInAFeedThatIsAlreadyDownloaded : WhenUsingTheEpisodeFinder
    {
        protected override void SetupData()
        {
            base.SetupData();
            _podcastFeedItems.Add(new PodcastFeedItem()
                                      {
                                          Address = new Uri("http://test/podcast.mp3"),
                                          EpisodeTitle = "TestEpisode",
                                          Published = _now.AddMonths(-1)
                                      });
            _podcastFeedItems.Add(new PodcastFeedItem()
                                      {
                                          Address = new Uri("http://test/podcast2.mp3"),
                                          EpisodeTitle = "TestEpisode2",
                                          Published = _now.AddMonths(-1)
                                      });
        }

        protected override void SetupStubs()
        {
            base.SetupStubs();

            _fileUtilities.Stub(utils => utils.FileExists(Path.Combine(_rootFolder,"podcast.mp3"))).Return(true);
        }

        protected override void When()
        {
            _episodeFinder.FindEpisodesToDownload(_rootFolder, _feedInfo, _episodesToSync);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            Assert.That(_episodesToSync.Count, Is.EqualTo(1));
            Assert.That(_episodesToSync[0].EpisodeUrl.ToString(), Is.EqualTo("http://test/podcast2.mp3"));
            Assert.That(_episodesToSync[0].DestinationPath, Is.EqualTo(Path.Combine(_rootFolder, "podcast2.mp3")));
        }
    }
}
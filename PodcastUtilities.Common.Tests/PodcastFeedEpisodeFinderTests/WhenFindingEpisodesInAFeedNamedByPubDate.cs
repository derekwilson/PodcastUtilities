using System;
using System.IO;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.PodcastFeedEpisodeFinderTests
{
    public class WhenFindingEpisodesInAFeedNamedByPubDate : WhenUsingTheEpisodeFinder
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

            _podcastInfo.Feed.NamingStyle = PodcastEpisodeNamingStyle.UrlFilenameAndPublishDateTime;
        }

        protected override void When()
        {
            _episodeFinder.FindEpisodesToDownload(_rootFolder, _podcastInfo, _episodesToSync);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            Assert.That(_episodesToSync.Count, Is.EqualTo(1));
            Assert.That(_episodesToSync[0].EpisodeUrl.ToString(), Is.EqualTo("http://test/podcast.mp3"));
            Assert.That(_episodesToSync[0].DestinationPath, Is.EqualTo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "2010_04_01_1011_podcast.mp3")));
        }
    }

    public class WhenFindingEpisodesInAFeedNamedByPubDateAndFolder : WhenUsingTheEpisodeFinder
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

            _podcastInfo.Feed.NamingStyle = PodcastEpisodeNamingStyle.UrlFilenameFeedTitleAndPublishDateTime;
        }

        protected override void When()
        {
            _episodeFinder.FindEpisodesToDownload(_rootFolder, _podcastInfo, _episodesToSync);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            Assert.That(_episodesToSync.Count, Is.EqualTo(1));
            Assert.That(_episodesToSync[0].EpisodeUrl.ToString(), Is.EqualTo("http://test/podcast.mp3"));
            Assert.That(_episodesToSync[0].DestinationPath, Is.EqualTo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "2010_04_01_1011_TestFolder_podcast.mp3")));
        }
    }
}
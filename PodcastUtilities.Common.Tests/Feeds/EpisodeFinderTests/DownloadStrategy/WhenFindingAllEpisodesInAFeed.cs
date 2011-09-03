using System;
using System.IO;
using NUnit.Framework;
using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.Common.Tests.Feeds.EpisodeFinderTests.DownloadStrategy
{
    public class WhenFindingAllEpisodesInAFeed : WhenUsingTheEpisodeFinder
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

        protected override void When()
        {
            _episodesToSync = _episodeFinder.FindEpisodesToDownload(_rootFolder,_retryWaitTime, _podcastInfo);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            Assert.That(_episodesToSync.Count, Is.EqualTo(2));

            Assert.That(_episodesToSync[0].EpisodeUrl.ToString(), Is.EqualTo("http://test/podcast.mp3"));
            Assert.That(_episodesToSync[0].DestinationPath, Is.EqualTo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast.mp3")));
            Assert.That(_episodesToSync[0].StateKey, Is.EqualTo(Path.Combine(_rootFolder, _podcastInfo.Folder)));
            Assert.That(_episodesToSync[0].Published, Is.EqualTo(_now.AddMonths(-1)));

            Assert.That(_episodesToSync[1].EpisodeUrl.ToString(), Is.EqualTo("http://test/podcast2.mp3"));
            Assert.That(_episodesToSync[1].DestinationPath, Is.EqualTo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast2.mp3")));
            Assert.That(_episodesToSync[1].StateKey, Is.EqualTo(Path.Combine(_rootFolder, _podcastInfo.Folder)));
            Assert.That(_episodesToSync[1].Published, Is.EqualTo(_now.AddMonths(-1)));
        }

        [Test]
        public void ItShouldSetTheRetryTime()
        {
            foreach (var episode in _episodesToSync)
            {
                Assert.That(episode.RetryWaitTimeInSeconds, Is.EqualTo(_retryWaitTime));
            }
        }
    }
}
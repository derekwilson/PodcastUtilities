using System;
using System.IO;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Feeds.EpisodeFinderTests.DownloadStrategy
{
    public class WhenFindingNewEpisodesInAFeed : WhenUsingTheEpisodeFinder
    {
        protected override void SetupData()
        {
            base.SetupData();

            _feedInfo.DownloadStrategy.Value = PodcastEpisodeDownloadStrategy.HighTide;

            _podcastFeedItems.Add(new PodcastFeedItem()
                                      {
                                          Address = new Uri("http://test/podcast.mp3"),
                                          EpisodeTitle = "TestEpisode",
                                          Published = _now.AddDays(-2)
                                      });
            _podcastFeedItems.Add(new PodcastFeedItem()
                                      {
                                          Address = new Uri("http://test/podcast2.mp3"),
                                          EpisodeTitle = "TestEpisode2",
                                          Published = _now.AddDays(-4)
                                      });
            _podcastFeedItems.Add(new PodcastFeedItem()
                                      {
                                          Address = new Uri("http://test/podcast3.mp3"),
                                          EpisodeTitle = "TestEpisode3",
                                          Published = _now.AddDays(-1)
                                      });
        }

        protected override void SetupStubs()
        {
            base.SetupStubs();
            _state.Stub(s => s.DownloadHighTide).Return(_now.AddDays(-3));
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
            Assert.That(_episodesToSync[0].Published, Is.EqualTo(_now.AddDays(-2)));

            Assert.That(_episodesToSync[1].EpisodeUrl.ToString(), Is.EqualTo("http://test/podcast3.mp3"));
            Assert.That(_episodesToSync[1].DestinationPath, Is.EqualTo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast3.mp3")));
            Assert.That(_episodesToSync[1].StateKey, Is.EqualTo(Path.Combine(_rootFolder, _podcastInfo.Folder)));
            Assert.That(_episodesToSync[1].Published, Is.EqualTo(_now.AddDays(-1)));
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
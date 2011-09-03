using System;
using System.IO;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.Common.Tests.Feeds.EpisodeFinderTests.NamingStyle
{
    public class WhenFindingEpisodesInAFeedNamedByEpisodeTitle : WhenUsingTheEpisodeFinder
    {
        protected override void SetupData()
        {
            base.SetupData();
            _podcastFeedItems.Add(new PodcastFeedItem()
                                      {
                                          Address = new Uri("http://test/podcast.mp3"),
                                          EpisodeTitle = "TestEpisode:",
                                          Published = _now.AddMonths(-1)
                                      });

            _podcastInfo.Feed.NamingStyle = PodcastEpisodeNamingStyle.EpisodeTitle;
        }

        protected override void When()
        {
            _episodesToSync = _episodeFinder.FindEpisodesToDownload(_rootFolder,_retryWaitTime, _podcastInfo);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            Assert.That(_episodesToSync.Count, Is.EqualTo(1));
            Assert.That(_episodesToSync[0].EpisodeUrl.ToString(), Is.EqualTo("http://test/podcast.mp3"));
            Assert.That(_episodesToSync[0].DestinationPath, Is.EqualTo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "TestEpisode_.mp3")));
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
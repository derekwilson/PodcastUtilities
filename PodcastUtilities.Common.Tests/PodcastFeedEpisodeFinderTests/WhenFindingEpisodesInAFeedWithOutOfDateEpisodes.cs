﻿using System;
using System.IO;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.PodcastFeedEpisodeFinderTests
{
    public class WhenFindingEpisodesInAFeedWithOutOfDateEpisodes : WhenUsingTheEpisodeFinder
    {
        protected override void SetupData()
        {
            base.SetupData();

            _feedInfo.MaximumDaysOld = 35;

            _podcastFeedItems.Add(new PodcastFeedItem()
                                      {
                                          Address = new Uri("http://test/podcast.mp3"),
                                          EpisodeTitle = "TestEpisode",
                                          Published = _now.AddMonths(-2)
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
            _episodeFinder.FindEpisodesToDownload(_rootFolder, _podcastInfo, _episodesToSync);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            Assert.That(_episodesToSync.Count, Is.EqualTo(1));
            Assert.That(_episodesToSync[0].EpisodeUrl.ToString(), Is.EqualTo("http://test/podcast2.mp3"));
            Assert.That(_episodesToSync[0].DestinationPath, Is.EqualTo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast2.mp3")));
        }
    }
}
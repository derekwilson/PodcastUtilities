#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
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
            _episodesToSync = _episodeFinder.FindEpisodesToDownload(_rootFolder,_retryWaitTime, _podcastInfo, _retainFeedXml);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            Assert.That(_episodesToSync.Count, Is.EqualTo(2));

            Assert.That(_episodesToSync[0].EpisodeUrl.ToString(), Is.EqualTo("http://test/podcast.mp3"));
            Assert.That(_episodesToSync[0].DestinationPath, Is.EqualTo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast.mp3")));
            Assert.That(_episodesToSync[0].StateKey, Is.EqualTo(Path.Combine(_rootFolder, _podcastInfo.Folder)));
            Assert.That(_episodesToSync[0].Published, Is.EqualTo(_now.AddMonths(-1)));
            Assert.That(_episodesToSync[0].PostDownloadCommand, Is.SameAs(_externalCommand));

            Assert.That(_episodesToSync[1].EpisodeUrl.ToString(), Is.EqualTo("http://test/podcast2.mp3"));
            Assert.That(_episodesToSync[1].DestinationPath, Is.EqualTo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast2.mp3")));
            Assert.That(_episodesToSync[1].StateKey, Is.EqualTo(Path.Combine(_rootFolder, _podcastInfo.Folder)));
            Assert.That(_episodesToSync[1].Published, Is.EqualTo(_now.AddMonths(-1)));
            Assert.That(_episodesToSync[1].PostDownloadCommand, Is.SameAs(_externalCommand));
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
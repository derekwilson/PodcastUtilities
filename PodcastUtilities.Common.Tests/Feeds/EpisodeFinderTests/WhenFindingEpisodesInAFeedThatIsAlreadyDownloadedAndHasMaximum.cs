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
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Feeds.EpisodeFinderTests
{
    public abstract class WhenFindingNewEpisodesInAFeedThatIsAlreadyDownloadedAndHasMaximum : WhenUsingTheEpisodeFinder
    {
        protected const int MaxEpisodes = 2;

        protected override void SetupData()
        {
            base.SetupData();

            _feedInfo.DownloadStrategy.Value = PodcastEpisodeDownloadStrategy.HighTide;
            _feedInfo.MaximumNumberOfDownloadedItems.Value = MaxEpisodes;

            // put in one poscast that is out of order - this is the most recent one
            _podcastFeedItems.Add(new PodcastFeedItem()
            {
                Address = new Uri("http://test/podcast-out-of-order.mp3"),
                EpisodeTitle = "TestEpisode-out-of=order",
                Published = _now.AddMonths(-2)
            });
            _podcastFeedItems.Add(new PodcastFeedItem()
            {
                Address = new Uri("http://test/podcast.mp3"),
                EpisodeTitle = "TestEpisode1",
                Published = _now.AddMonths(-10)
            });
            _podcastFeedItems.Add(new PodcastFeedItem()
            {
                Address = new Uri("http://test/podcast2.mp3"),
                EpisodeTitle = "TestEpisode2",
                Published = _now.AddMonths(-9)
            });
            // the high tide is here
            _podcastFeedItems.Add(new PodcastFeedItem()
            {
                Address = new Uri("http://test/podcast3.mp3"),
                EpisodeTitle = "TestEpisode3",
                Published = _now.AddMonths(-8)
            });
            _podcastFeedItems.Add(new PodcastFeedItem()
            {
                Address = new Uri("http://test/podcast4.mp3"),
                EpisodeTitle = "TestEpisode4",
                Published = _now.AddMonths(-7)
            });
            _podcastFeedItems.Add(new PodcastFeedItem()
            {
                Address = new Uri("http://test/podcast5.mp3"),
                EpisodeTitle = "TestEpisode5",
                Published = _now.AddMonths(-6)
            });
            _podcastFeedItems.Add(new PodcastFeedItem()
            {
                Address = new Uri("http://test/podcast6.mp3"),
                EpisodeTitle = "TestEpisode6",
                Published = _now.AddMonths(-5)
            });
        }

        protected override void SetupStubs()
        {
            base.SetupStubs();
            // set the high tide
            _state.Stub(s => s.DownloadHighTide).Return(_now.AddMonths(-8).AddDays(-3));
        }
    }

    public class WhenFindingNewEpisodesInAFeedThatIsAlreadyDownloadedAndHasMaximumWithNothingInCache : WhenFindingNewEpisodesInAFeedThatIsAlreadyDownloadedAndHasMaximum 
    { 
        protected override void When()
        {
            _episodesToSync = _episodeFinder.FindEpisodesToDownload(_rootFolder, _retryWaitTime, _podcastInfo, _retainFeedXml);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            Assert.That(_episodesToSync.Count, Is.EqualTo(MaxEpisodes));
        }

        [Test]
        public void ItShouldReturnTheUrl()
        {
            Assert.That(_episodesToSync[0].EpisodeUrl.ToString(), Is.EqualTo("http://test/podcast3.mp3"));
            Assert.That(_episodesToSync[1].EpisodeUrl.ToString(), Is.EqualTo("http://test/podcast4.mp3"));
        }

        [Test]
        public void ItShouldReturnTheDestinationPath()
        {
            Assert.That(_episodesToSync[0].DestinationPath, Is.EqualTo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast3.mp3")));
            Assert.That(_episodesToSync[1].DestinationPath, Is.EqualTo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast4.mp3")));
        }
    }


    public class WhenFindingNewEpisodesInAFeedThatIsAlreadyDownloadedAndHasMaximumWithCachedItems : WhenFindingNewEpisodesInAFeedThatIsAlreadyDownloadedAndHasMaximum
    {
        protected override void SetupStubs()
        {
            base.SetupStubs();

            _fileUtilities.Stub(utils => utils.FileExists(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast3.mp3"))).Return(true);
        }

        protected override void When()
        {
            _episodesToSync = _episodeFinder.FindEpisodesToDownload(_rootFolder, _retryWaitTime, _podcastInfo, _retainFeedXml);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            // we should download one less as there is one in the cache
            Assert.That(_episodesToSync.Count, Is.EqualTo(MaxEpisodes - 1));
        }

        [Test]
        public void ItShouldReturnTheUrl()
        {
            Assert.That(_episodesToSync[0].EpisodeUrl.ToString(), Is.EqualTo("http://test/podcast4.mp3"));
        }

        [Test]
        public void ItShouldReturnTheDestinationPath()
        {
            Assert.That(_episodesToSync[0].DestinationPath, Is.EqualTo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast4.mp3")));
        }
    }

    public class WhenFindingNewEpisodesInAFeedThatIsAlreadyDownloadedAndHasMaximumWithCacheFull : WhenFindingNewEpisodesInAFeedThatIsAlreadyDownloadedAndHasMaximum
    {
        protected override void SetupStubs()
        {
            base.SetupStubs();

            _fileUtilities.Stub(utils => utils.FileExists(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast3.mp3"))).Return(true);
            _fileUtilities.Stub(utils => utils.FileExists(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast4.mp3"))).Return(true);
        }

        protected override void When()
        {
            _episodesToSync = _episodeFinder.FindEpisodesToDownload(_rootFolder, _retryWaitTime, _podcastInfo, _retainFeedXml);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            // we should not download any items
            Assert.That(_episodesToSync.Count, Is.EqualTo(0));
        }
    }

    public class WhenFindingAllEpisodesInAFeedThatIsAlreadyDownloadedAndHasMaximumWithCachedItems : WhenFindingNewEpisodesInAFeedThatIsAlreadyDownloadedAndHasMaximum
    {
        protected override void SetupData()
        {
            base.SetupData();

            _feedInfo.DownloadStrategy.Value = PodcastEpisodeDownloadStrategy.All;
        }

        protected override void SetupStubs()
        {
            base.SetupStubs();

            _fileUtilities.Stub(utils => utils.FileExists(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast.mp3"))).Return(true);
        }

        protected override void When()
        {
            _episodesToSync = _episodeFinder.FindEpisodesToDownload(_rootFolder, _retryWaitTime, _podcastInfo, _retainFeedXml);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            // we should download one less as there is one in the cache
            Assert.That(_episodesToSync.Count, Is.EqualTo(MaxEpisodes - 1));
        }

        [Test]
        public void ItShouldReturnTheUrl()
        {
            Assert.That(_episodesToSync[0].EpisodeUrl.ToString(), Is.EqualTo("http://test/podcast2.mp3"));
        }

        [Test]
        public void ItShouldReturnTheDestinationPath()
        {
            Assert.That(_episodesToSync[0].DestinationPath, Is.EqualTo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast2.mp3")));
        }
    }

    public class WhenFindingLatestEpisodeInAFeedThatIsAlreadyDownloadedAndHasMaximumWithNothingInCache : WhenFindingNewEpisodesInAFeedThatIsAlreadyDownloadedAndHasMaximum
    {
        protected override void SetupData()
        {
            base.SetupData();

            _feedInfo.DownloadStrategy.Value = PodcastEpisodeDownloadStrategy.Latest;
        }

        protected override void When()
        {
            _episodesToSync = _episodeFinder.FindEpisodesToDownload(_rootFolder, _retryWaitTime, _podcastInfo, _retainFeedXml);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            Assert.That(_episodesToSync.Count, Is.EqualTo(1));
        }

        [Test]
        public void ItShouldReturnTheUrl()
        {
            Assert.That(_episodesToSync[0].EpisodeUrl.ToString(), Is.EqualTo("http://test/podcast-out-of-order.mp3"));
        }

        [Test]
        public void ItShouldReturnTheDestinationPath()
        {
            Assert.That(_episodesToSync[0].DestinationPath, Is.EqualTo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast-out-of-order.mp3")));
        }
    }

    public class WhenFindingLatestEpisodeInAFeedThatIsAlreadyDownloadedAndHasMaximumWithCachedLatestItem : WhenFindingNewEpisodesInAFeedThatIsAlreadyDownloadedAndHasMaximum
    {
        protected override void SetupStubs()
        {
            base.SetupStubs();

            _fileUtilities.Stub(utils => utils.FileExists(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast-out-of-order.mp3"))).Return(true);
        }

        protected override void SetupData()
        {
            base.SetupData();

            _feedInfo.DownloadStrategy.Value = PodcastEpisodeDownloadStrategy.Latest;
        }

        protected override void When()
        {
            _episodesToSync = _episodeFinder.FindEpisodesToDownload(_rootFolder, _retryWaitTime, _podcastInfo, _retainFeedXml);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            Assert.That(_episodesToSync.Count, Is.EqualTo(0));
        }
    }

    public class WhenFindingLatestEpisodeInAFeedThatIsAlreadyDownloadedAndHasMaximumWithCachedItems : WhenFindingNewEpisodesInAFeedThatIsAlreadyDownloadedAndHasMaximum
    {
        protected override void SetupStubs()
        {
            base.SetupStubs();

            // all except the last 2 are in the cache
            _fileUtilities.Stub(utils => utils.FileExists(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast.mp3"))).Return(true);
            _fileUtilities.Stub(utils => utils.FileExists(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast2.mp3"))).Return(true);
            _fileUtilities.Stub(utils => utils.FileExists(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast3.mp3"))).Return(true);
            _fileUtilities.Stub(utils => utils.FileExists(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast4.mp3"))).Return(true);
            _fileUtilities.Stub(utils => utils.FileExists(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast5.mp3"))).Return(true);
        }

        protected override void SetupData()
        {
            base.SetupData();

            _feedInfo.DownloadStrategy.Value = PodcastEpisodeDownloadStrategy.Latest;
        }

        protected override void When()
        {
            _episodesToSync = _episodeFinder.FindEpisodesToDownload(_rootFolder, _retryWaitTime, _podcastInfo, _retainFeedXml);
        }

        [Test]
        public void ItShouldReturnTheList()
        {
            Assert.That(_episodesToSync.Count, Is.EqualTo(1));
        }

        [Test]
        public void ItShouldReturnTheUrl()
        {
            Assert.That(_episodesToSync[0].EpisodeUrl.ToString(), Is.EqualTo("http://test/podcast-out-of-order.mp3"));
        }

        [Test]
        public void ItShouldReturnTheDestinationPath()
        {
            Assert.That(_episodesToSync[0].DestinationPath, Is.EqualTo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "podcast-out-of-order.mp3")));
        }
    }

}
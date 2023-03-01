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
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Multiplatform.Tests.Configuration.PodcastInfoTests.Clone
{
    public class WhenCloningAPodcastInfoWithAFeed : WhenCloningAPodcastInfo
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _podcastInfo.AscendingSort.Value = true;
            _podcastInfo.Folder = "FOLDER";
            _podcastInfo.MaximumNumberOfFiles.Value = 123;
            _podcastInfo.Pattern.Value = "PATTERN";
            _podcastInfo.SortField.Value = PodcastFileSortField.FileName;

            _podcastInfo.Feed = new FeedInfo(_controlFile)
            {
                Address = new Uri("http://test.com"),
            };
            _podcastInfo.Feed.DeleteDownloadsDaysOld.Value = 456;
            _podcastInfo.Feed.MaximumNumberOfDownloadedItems.Value = 123;
            _podcastInfo.Feed.DownloadStrategy.Value = PodcastEpisodeDownloadStrategy.HighTide;
            _podcastInfo.Feed.Format.Value = PodcastFeedFormat.RSS;
            _podcastInfo.Feed.MaximumDaysOld.Value = 789;
            _podcastInfo.Feed.NamingStyle.Value = PodcastEpisodeNamingStyle.UrlFileName;
        }

        protected override void When()
        {
            _clonedPodcast = _podcastInfo.Clone() as PodcastInfo;
        }

        [Test]
        public void ItShouldCloneThePodcastAscendingSort()
        {
            Assert.That(_clonedPodcast.AscendingSort.Value, Is.EqualTo(true));
        }

        [Test]
        public void ItShouldCloneThePodcastFolder()
        {
            Assert.That(_clonedPodcast.Folder, Is.EqualTo("FOLDER"));
        }

        [Test]
        public void ItShouldCloneThePodcastMaximumNumberOfFiles()
        {
            Assert.That(_clonedPodcast.MaximumNumberOfFiles.Value, Is.EqualTo(123));
        }

        [Test]
        public void ItShouldCloneThePodcastSortField()
        {
            Assert.That(_clonedPodcast.SortField.Value, Is.EqualTo(PodcastFileSortField.FileName));
        }

        [Test]
        public void ItShouldCloneThePostDownloadCommand()
        {
            Assert.That(_clonedPodcast.PostDownloadCommand, Is.Null);
        }

        [Test]
        public void ItShouldCloneTheFeedAddress()
        {
            Assert.That(_clonedPodcast.Feed.Address.ToString(), Is.EqualTo("http://test.com/"));
        }

        [Test]
        public void ItShouldCloneTheFeedDeleteDownloadsDaysOld()
        {
            Assert.That(_clonedPodcast.Feed.DeleteDownloadsDaysOld.Value, Is.EqualTo(456));
        }

        [Test]
        public void ItShouldCloneTheFeedMaximumNumberOfDownloadedItems()
        {
            Assert.That(_clonedPodcast.Feed.MaximumNumberOfDownloadedItems.Value, Is.EqualTo(123));
            Assert.That(_clonedPodcast.Feed.MaximumNumberOfDownloadedItems.IsSet, Is.EqualTo(true));
        }

        [Test]
        public void ItShouldCloneTheFeedDownloadStrategy()
        {
            Assert.That(_clonedPodcast.Feed.DownloadStrategy.Value, Is.EqualTo(PodcastEpisodeDownloadStrategy.HighTide));
        }

        [Test]
        public void ItShouldCloneTheFeedFormat()
        {
            Assert.That(_clonedPodcast.Feed.Format.Value, Is.EqualTo(PodcastFeedFormat.RSS));
        }

        [Test]
        public void ItShouldCloneTheFeedMaximumDaysOld()
        {
            Assert.That(_clonedPodcast.Feed.MaximumDaysOld.Value, Is.EqualTo(789));
        }

        [Test]
        public void ItShouldCloneTheFeedNamingStyle()
        {
            Assert.That(_clonedPodcast.Feed.NamingStyle.Value, Is.EqualTo(PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime));
        }
    }
}
using System;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.PodcastInfoTests.Clone
{
    public class WhenCloningAPodcastInfoWithAFeed : WhenCloningAPodcastInfo
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _pocastInfo.AscendingSort.Value = true;
            _pocastInfo.Folder = "FOLDER";
            _pocastInfo.MaximumNumberOfFiles = 123;
            _pocastInfo.Pattern = "PATTERN";
            _pocastInfo.SortField.Value = PodcastFileSortField.FileName;

            _pocastInfo.Feed = new FeedInfo(_controlFile)
                                   {
                                       Address = new Uri("http://test.com"),
                                   };
            _pocastInfo.Feed.DeleteDownloadsDaysOld.Value = 456;
            _pocastInfo.Feed.DownloadStrategy.Value = PodcastEpisodeDownloadStrategy.HighTide;
            _pocastInfo.Feed.Format.Value = PodcastFeedFormat.RSS;
            _pocastInfo.Feed.MaximumDaysOld.Value = 789;
            _pocastInfo.Feed.NamingStyle.Value = PodcastEpisodeNamingStyle.UrlFileName;
        }

        protected override void When()
        {
            _clonedPodcast = _pocastInfo.Clone() as PodcastInfo;
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
            Assert.That(_clonedPodcast.MaximumNumberOfFiles, Is.EqualTo(123));
        }

        [Test]
        public void ItShouldCloneThePodcastSortField()
        {
            Assert.That(_clonedPodcast.SortField.Value, Is.EqualTo(PodcastFileSortField.FileName));
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
            Assert.That(_clonedPodcast.Feed.NamingStyle.Value, Is.EqualTo(PodcastEpisodeNamingStyle.UrlFileName));
        }
    }
}
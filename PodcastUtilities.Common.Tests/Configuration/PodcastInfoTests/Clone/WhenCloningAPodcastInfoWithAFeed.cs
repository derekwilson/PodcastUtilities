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
            _pocastInfo.AscendingSort = true;
            _pocastInfo.Folder = "FOLDER";
            _pocastInfo.MaximumNumberOfFiles = 123;
            _pocastInfo.Pattern = "PATTERN";
            _pocastInfo.SortField = "SORT";

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
        public void ItShouldCloneThePodcast()
        {
            Assert.That(_clonedPodcast.AscendingSort, Is.EqualTo(true));
            Assert.That(_clonedPodcast.Folder, Is.EqualTo("FOLDER"));
            Assert.That(_clonedPodcast.MaximumNumberOfFiles, Is.EqualTo(123));
            Assert.That(_clonedPodcast.Pattern, Is.EqualTo("PATTERN"));
            Assert.That(_clonedPodcast.SortField, Is.EqualTo("SORT"));
        }

        [Test]
        public void ItShouldCloneTheFeed()
        {
            Assert.That(_clonedPodcast.Feed.Address.ToString(), Is.EqualTo("http://test.com/"));
            Assert.That(_clonedPodcast.Feed.DeleteDownloadsDaysOld.Value, Is.EqualTo(456));
            Assert.That(_clonedPodcast.Feed.DownloadStrategy.Value, Is.EqualTo(PodcastEpisodeDownloadStrategy.HighTide));
            Assert.That(_clonedPodcast.Feed.Format.Value, Is.EqualTo(PodcastFeedFormat.RSS));
            Assert.That(_clonedPodcast.Feed.MaximumDaysOld.Value, Is.EqualTo(789));
            Assert.That(_clonedPodcast.Feed.NamingStyle.Value, Is.EqualTo(PodcastEpisodeNamingStyle.UrlFileName));
        }
    }
}
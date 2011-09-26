using System;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.FeedInfoTests.Clone
{
    public class WhenCloningAPopulatedFeedInfo : WhenCloningAFeedInfo
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _feedInfo = new FeedInfo(_controlFile)
                            {
                                Address = new Uri("http://test.com"),
                            };
            _feedInfo.DeleteDownloadsDaysOld.Value = 456;
            _feedInfo.DownloadStrategy.Value = PodcastEpisodeDownloadStrategy.HighTide;
            _feedInfo.Format.Value = PodcastFeedFormat.RSS;
            _feedInfo.MaximumDaysOld.Value = 789;
            _feedInfo.NamingStyle.Value = PodcastEpisodeNamingStyle.UrlFileName;
        }

        protected override void When()
        {
            _clonedFeedInfo = _feedInfo.Clone() as FeedInfo;
        }

        [Test]
        public void ItShouldCloneTheAddress()
        {
            Assert.That(_clonedFeedInfo.Address.ToString(), Is.EqualTo("http://test.com/"));
        }

        [Test]
        public void ItShouldCloneTheDeleteDownloadsDaysOld()
        {
            Assert.That(_clonedFeedInfo.DeleteDownloadsDaysOld.Value, Is.EqualTo(456));
        }

        [Test]
        public void ItShouldCloneTheDownloadStrategy()
        {
            Assert.That(_clonedFeedInfo.DownloadStrategy.Value, Is.EqualTo(PodcastEpisodeDownloadStrategy.HighTide));
        }

        [Test]
        public void ItShouldCloneTheFormat()
        {
            Assert.That(_clonedFeedInfo.Format.Value, Is.EqualTo(PodcastFeedFormat.RSS));
        }

        [Test]
        public void ItShouldCloneTheMaximumDaysOld()
        {
            Assert.That(_clonedFeedInfo.MaximumDaysOld.Value, Is.EqualTo(789));
        }

        [Test]
        public void ItShouldCloneTheNamingStyle()
        {
            Assert.That(_clonedFeedInfo.NamingStyle.Value, Is.EqualTo(PodcastEpisodeNamingStyle.UrlFileName));
        }

    }
}
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
        public void ItShouldCloneTheFeed()
        {
            Assert.That(_clonedFeedInfo.Address.ToString(), Is.EqualTo("http://test.com/"));
            Assert.That(_clonedFeedInfo.DeleteDownloadsDaysOld.Value, Is.EqualTo(456));
            Assert.That(_clonedFeedInfo.DownloadStrategy.Value, Is.EqualTo(PodcastEpisodeDownloadStrategy.HighTide));
            Assert.That(_clonedFeedInfo.Format.Value, Is.EqualTo(PodcastFeedFormat.RSS));
            Assert.That(_clonedFeedInfo.MaximumDaysOld.Value, Is.EqualTo(789));
            Assert.That(_clonedFeedInfo.NamingStyle.Value, Is.EqualTo(PodcastEpisodeNamingStyle.UrlFileName));
        }

    }
}
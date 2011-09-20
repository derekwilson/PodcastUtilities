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
                                DeleteDownloadsDaysOld = 456,
                                DownloadStrategy = PodcastEpisodeDownloadStrategy.HighTide,
                                Format = PodcastFeedFormat.RSS,
                                MaximumDaysOld = 789,
                                NamingStyle = PodcastEpisodeNamingStyle.UrlFileName
                            };
        }

        protected override void When()
        {
            _clonedFeedInfo = _feedInfo.Clone() as FeedInfo;
        }

        [Test]
        public void ItShouldCloneTheFeed()
        {
            Assert.That(_clonedFeedInfo.Address.ToString(), Is.EqualTo("http://test.com/"));
            Assert.That(_clonedFeedInfo.DeleteDownloadsDaysOld, Is.EqualTo(456));
            Assert.That(_clonedFeedInfo.DownloadStrategy, Is.EqualTo(PodcastEpisodeDownloadStrategy.HighTide));
            Assert.That(_clonedFeedInfo.Format, Is.EqualTo(PodcastFeedFormat.RSS));
            Assert.That(_clonedFeedInfo.MaximumDaysOld, Is.EqualTo(789));
            Assert.That(_clonedFeedInfo.NamingStyle, Is.EqualTo(PodcastEpisodeNamingStyle.UrlFileName));
        }

    }
}
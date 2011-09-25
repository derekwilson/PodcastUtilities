using System;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.FeedInfoTests.Clone
{
    public class WhenCloningAnUnpopulatedFeedInfo : WhenCloningAFeedInfo
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _feedInfo = new FeedInfo(_controlFile)
                            {
                                Address = new Uri("http://test.com")
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
            Assert.That(_clonedFeedInfo.DeleteDownloadsDaysOld.Value, Is.EqualTo(88));
            Assert.That(_clonedFeedInfo.DownloadStrategy.Value, Is.EqualTo(PodcastEpisodeDownloadStrategy.Latest));
            Assert.That(_clonedFeedInfo.Format.Value, Is.EqualTo(PodcastFeedFormat.ATOM));
            Assert.That(_clonedFeedInfo.MaximumDaysOld.Value, Is.EqualTo(99));
            Assert.That(_clonedFeedInfo.NamingStyle.Value, Is.EqualTo(PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime));
        }
    }
}
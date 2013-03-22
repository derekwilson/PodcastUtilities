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
        public void ItShouldCloneTheAddress()
        {
            Assert.That(_clonedFeedInfo.Address.ToString(), Is.EqualTo("http://test.com/"));
        }
        [Test]
        public void ItShouldCloneTheDeleteDownloadsDaysOld()
        {
            Assert.That(_clonedFeedInfo.DeleteDownloadsDaysOld.Value, Is.EqualTo(88));
        }
        [Test]
        public void ItShouldCloneTheDownloadStrategy()
        {
            Assert.That(_clonedFeedInfo.DownloadStrategy.Value, Is.EqualTo(PodcastEpisodeDownloadStrategy.Latest));
        }
        [Test]
        public void ItShouldCloneTheFormat()
        {
            Assert.That(_clonedFeedInfo.Format.Value, Is.EqualTo(PodcastFeedFormat.ATOM));
        }
        [Test]
        public void ItShouldCloneTheMaximumDaysOld()
        {
            Assert.That(_clonedFeedInfo.MaximumDaysOld.Value, Is.EqualTo(99));
        }
        [Test]
        public void ItShouldCloneTheNamingStyle()
        {
            Assert.That(_clonedFeedInfo.NamingStyle.Value, Is.EqualTo(PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime));
        }
    }
}
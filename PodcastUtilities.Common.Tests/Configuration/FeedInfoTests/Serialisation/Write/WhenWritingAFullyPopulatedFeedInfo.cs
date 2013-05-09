using System;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.FeedInfoTests.Serialisation.Write
{
    public class WhenWritingAFullyPopulatedFeedInfo : WhenWritingAFeedInfo
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
            _feedInfo.WriteXml(_xmlWriter);
            base.When();
        }

        [Test]
        public void ItShouldWriteTheXml()
        {
            Assert.That(_textReader.ReadToEnd(), Is.EqualTo("<url>http://test.com/</url><downloadStrategy>high_tide</downloadStrategy><format>rss</format><maximumDaysOld>789</maximumDaysOld><namingStyle>url</namingStyle><deleteDownloadsDaysOld>456</deleteDownloadsDaysOld>"));
        }
    }
}
using System;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.FeedInfoTests.Serialisation.Write
{
    public class WhenWritingAnEmptyFeedInfo : WhenWritingAFeedInfo
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _feedInfo = new FeedInfo(_controlFile)
                            {
                                Address = new Uri("http://test.com"),
                            };
        }

        protected override void When()
        {
            _feedInfo.WriteXml(_xmlWriter);
            base.When();
        }

        [Test]
        public void ItShouldWriteTheXml()
        {
            Assert.That(_textReader.ReadToEnd(), Is.EqualTo("<url>http://test.com/</url>"));
        }
    }
}
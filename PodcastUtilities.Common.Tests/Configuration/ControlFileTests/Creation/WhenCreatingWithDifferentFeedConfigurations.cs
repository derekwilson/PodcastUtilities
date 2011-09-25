using System;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Exceptions;

namespace PodcastUtilities.Common.Tests.Configuration.ControlFileTests.Creation
{
    abstract class WhenCreatingAControlfileWithDifferentFeedFormats : WhenTestingAControlFile
    {
        protected string FeedFormatText { get; set; }
        protected PodcastFeedFormat Format1 { get; set; }
        protected PodcastFeedFormat Format2 { get; set; }
        protected Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            XmlNode n = ControlFileXmlDocument.SelectSingleNode("podcasts/global/feed/format");
            n.InnerText = FeedFormatText;
        }

        protected override void When()
        {
            ThrownException = null;
            try
            {
                ControlFile = new ReadOnlyControlFile(ControlFileXmlDocument);

                // this will be the defaulted value from the global section
                Format1 = ControlFile.GetPodcasts().ElementAt(1).Feed.Format.Value;
                // this is set explicitly by the feed
                Format2 = ControlFile.GetPodcasts().ElementAt(2).Feed.Format.Value;
            }
            catch (Exception exception)
            {
                ThrownException = exception;
            }
        }
    }

    class WhenCreatingAControlFileWithRssFeedFormat : WhenCreatingAControlfileWithDifferentFeedFormats
    {
        protected override void GivenThat()
        {
            FeedFormatText = "rss";
            base.GivenThat();
        }

        [Test]
        public void ItShouldNotThorw()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Test]
        public void ItShouldReadTheFormat()
        {
            Assert.That(Format1, Is.EqualTo(PodcastFeedFormat.RSS));
            Assert.That(Format2, Is.EqualTo(PodcastFeedFormat.RSS));
        }
    }

    class WhenCreatingAControlFileWithAtomFeedFormat : WhenCreatingAControlfileWithDifferentFeedFormats
    {
        protected override void GivenThat()
        {
            FeedFormatText = "atom";
            base.GivenThat();
        }

        [Test]
        public void ItShouldNotThorw()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Test]
        public void ItShouldReadTheFormat()
        {
            Assert.That(Format1, Is.EqualTo(PodcastFeedFormat.ATOM));
            Assert.That(Format2, Is.EqualTo(PodcastFeedFormat.RSS));
        }
    }

    class WhenCreatingAControlFileWithAnUnknownFeedFormat : WhenCreatingAControlfileWithDifferentFeedFormats
    {
        protected override void GivenThat()
        {
            FeedFormatText = "unknown";
            base.GivenThat();
        }

        [Test]
        public void ItShouldThorw()
        {
            Assert.IsInstanceOf(typeof(ControlFileValueFormatException), ThrownException);
        }
    }
}

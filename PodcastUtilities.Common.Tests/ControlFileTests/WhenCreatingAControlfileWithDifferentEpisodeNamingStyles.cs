using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.ControlFileTests
{
    abstract class WhenCreatingAControlfileWithDifferentEpisodeNamingStyles : WhenTestingAControlFile
    {
        protected string NamingStyleTextText { get; set; }
        protected PodcastEpisodeNamingStyle NamingStyle1 { get; set; }
        protected PodcastEpisodeNamingStyle NamingStyle2 { get; set; }
        protected Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            XmlNode n = ControlFileXmlDocument.SelectSingleNode("podcasts/global/feed/namingStyle");
            n.InnerText = NamingStyleTextText;
        }

        protected override void When()
        {
            ThrownException = null;
            try
            {
                ControlFile = new ControlFile(ControlFileXmlDocument);

                // this will be the defaulted value from the global section
                NamingStyle1 = ControlFile.Podcasts[1].Feed.NamingStyle;
                // this is set explicitly by the feed
                NamingStyle2 = ControlFile.Podcasts[2].Feed.NamingStyle;
            }
            catch (Exception exception)
            {
                ThrownException = exception;
            }
        }
    }

    class WhenCreatingAControlFileWithNoDefaultEpisodeNamingStyle : WhenCreatingAControlfileWithDifferentEpisodeNamingStyles
    {
        protected override void GivenThat()
        {
            NamingStyleTextText = "";
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
            Assert.That(NamingStyle1, Is.EqualTo(PodcastEpisodeNamingStyle.UrlFilename));
            Assert.That(NamingStyle2, Is.EqualTo(PodcastEpisodeNamingStyle.UrlFilenameFeedTitleAndPublishDateTime));
        }
    }

    class WhenCreatingAControlFileWithDefaultEpisodeNamingStyle : WhenCreatingAControlfileWithDifferentEpisodeNamingStyles
    {
        protected override void GivenThat()
        {
            NamingStyleTextText = "pubdate_url";
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
            Assert.That(NamingStyle1, Is.EqualTo(PodcastEpisodeNamingStyle.UrlFilenameAndPublishDateTime));
            Assert.That(NamingStyle2, Is.EqualTo(PodcastEpisodeNamingStyle.UrlFilenameFeedTitleAndPublishDateTime));
        }
    }
}

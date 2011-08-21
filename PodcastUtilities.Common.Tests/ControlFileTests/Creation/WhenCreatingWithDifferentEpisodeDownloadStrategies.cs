using System;
using System.Xml;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.ControlFileTests.Creation
{
    abstract class WhenCreatingAControlfileWithDifferentEpisodeDownloadStrategies : WhenTestingAControlFile
    {
        protected string StrategyText { get; set; }
        protected PodcastEpisodeDownloadStrategy Strategy1 { get; set; }
        protected PodcastEpisodeDownloadStrategy Strategy2 { get; set; }
        protected Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            XmlNode n = ControlFileXmlDocument.SelectSingleNode("podcasts/global/feed/downloadStrategy");
            n.InnerText = StrategyText;
        }

        protected override void When()
        {
            ThrownException = null;
            try
            {
                ControlFile = new ControlFile(ControlFileXmlDocument);

                // this will be the defaulted value from the global section
                Strategy1 = ControlFile.Podcasts[1].Feed.DownloadStrategy;
                // this is set explicitly by the feed
                Strategy2 = ControlFile.Podcasts[2].Feed.DownloadStrategy;
            }
            catch (Exception exception)
            {
                ThrownException = exception;
            }
        }
    }

    class WhenCreatingAControlFileWithNoDefaultEpisodeDownloadStrategy : WhenCreatingAControlfileWithDifferentEpisodeDownloadStrategies
    {
        protected override void GivenThat()
        {
            StrategyText = "";
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
            Assert.That(Strategy1, Is.EqualTo(PodcastEpisodeDownloadStrategy.All));
            Assert.That(Strategy2, Is.EqualTo(PodcastEpisodeDownloadStrategy.HighTide));
        }
    }

    class WhenCreatingAControlFileWithDefaultEpisodeDownloadStrategy : WhenCreatingAControlfileWithDifferentEpisodeDownloadStrategies
    {
        protected override void GivenThat()
        {
            StrategyText = "latest";
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
            Assert.That(Strategy1, Is.EqualTo(PodcastEpisodeDownloadStrategy.Latest));
            Assert.That(Strategy2, Is.EqualTo(PodcastEpisodeDownloadStrategy.HighTide));
        }
    }


}
using System;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.ControlFileTests.Creation
{
    public abstract class WhenCreatingAControlfileWithDifferentFeedEpisodeExpiry : WhenTestingAControlFile
    {
        protected string FeedMaximumDaysOldText { get; set; }
        protected int MaxDaysOld1 { get; set; }
        protected int MaxDaysOld2 { get; set; }
        protected Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            XmlNode n = ControlFileXmlDocument.SelectSingleNode("podcasts/global/feed/maximumDaysOld");
            n.InnerText = FeedMaximumDaysOldText;
        }

        protected override void When()
        {
            ThrownException = null;
            try
            {
                ControlFile = new ReadOnlyControlFile(ControlFileXmlDocument);

                // this will be the defaulted value from the global section
                MaxDaysOld1 = ControlFile.GetPodcasts().ElementAt(1).Feed.MaximumDaysOld.Value;
                // this is set explicitly by the feed
                MaxDaysOld2 = ControlFile.GetPodcasts().ElementAt(2).Feed.MaximumDaysOld.Value;
            }
            catch (Exception exception)
            {
                ThrownException = exception;
            }
        }
    }

    public class WhenCreatingAControlFileWithNoDefaultMaxDaysOld : WhenCreatingAControlfileWithDifferentFeedEpisodeExpiry
    {
        protected override void GivenThat()
        {
            FeedMaximumDaysOldText = "";
            base.GivenThat();
        }

        [Test]
        public void ItShouldNotThorw()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Test]
        public void ItShouldReadTheValue()
        {
            Assert.That(MaxDaysOld1, Is.EqualTo(int.MaxValue));
            Assert.That(MaxDaysOld2, Is.EqualTo(11));
        }
    }

    public class WhenCreatingAControlFileWithDefaultMaxDaysOld : WhenCreatingAControlfileWithDifferentFeedEpisodeExpiry
    {
        protected override void GivenThat()
        {
            FeedMaximumDaysOldText = "88";
            base.GivenThat();
        }

        [Test]
        public void ItShouldNotThorw()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Test]
        public void ItShouldReadTheValue()
        {
            Assert.That(MaxDaysOld1, Is.EqualTo(88));
            Assert.That(MaxDaysOld2, Is.EqualTo(11));
        }
    }
}


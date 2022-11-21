using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PodcastUtilities.Common.Tests.Configuration.ControlFileTests.Creation
{
    public abstract class WhenCreatingAControlfileWithDifferentFeedMaximumNumberOfDownloadedItems : WhenTestingAControlFile
    {
        protected string FeedMaxNumberOfItemsText { get; set; }
        protected int MaxNumberOfItems1 { get; set; }
        protected int MaxNumberOfItems2 { get; set; }
        protected Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            XmlNode n = ControlFileXmlDocument.SelectSingleNode("podcasts/global/feed/maximumNumberOfDownloadedItems");
            n.InnerText = FeedMaxNumberOfItemsText;
        }

        protected override void When()
        {
            ThrownException = null;
            try
            {
                ControlFile = new ReadOnlyControlFile(ControlFileXmlDocument);

                // this will be the defaulted value from the global section
                MaxNumberOfItems1 = ControlFile.GetPodcasts().ElementAt(1).Feed.MaximumNumberOfDownloadedItems.Value;
                // this is set explicitly by the feed
                MaxNumberOfItems2 = ControlFile.GetPodcasts().ElementAt(2).Feed.MaximumNumberOfDownloadedItems.Value;
            }
            catch (Exception exception)
            {
                ThrownException = exception;
            }
        }
    }


    public class WhenCreatingAControlFileWithNoDefaultMaximumNumberOfDownloadedItems : WhenCreatingAControlfileWithDifferentFeedMaximumNumberOfDownloadedItems
    {
        protected override void GivenThat()
        {
            FeedMaxNumberOfItemsText = "";
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
            Assert.That(MaxNumberOfItems1, Is.EqualTo(int.MaxValue),"Global Section");
            Assert.That(MaxNumberOfItems2, Is.EqualTo(15), "Feed section");
        }
    }

    public class WhenCreatingAControlFileWithDefaultMaximumNumberOfDownloadedItems : WhenCreatingAControlfileWithDifferentFeedMaximumNumberOfDownloadedItems
    {
        protected override void GivenThat()
        {
            FeedMaxNumberOfItemsText = "88";
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
            Assert.That(MaxNumberOfItems1, Is.EqualTo(88), "Global Section");
            Assert.That(MaxNumberOfItems2, Is.EqualTo(15), "Feed Section");
        }
    }

    public class WhenCreatingAControlFileWithDefaultMaximumNumberOfDownloadedItemsZero : WhenCreatingAControlfileWithDifferentFeedMaximumNumberOfDownloadedItems
    {
        protected override void GivenThat()
        {
            FeedMaxNumberOfItemsText = "0";
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
            Assert.That(MaxNumberOfItems1, Is.EqualTo(0), "Global Section");
            Assert.That(MaxNumberOfItems2, Is.EqualTo(15), "Feed Section");
        }
    }
}

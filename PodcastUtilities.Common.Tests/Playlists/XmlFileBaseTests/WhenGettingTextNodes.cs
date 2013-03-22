using System;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.Playlists.XmlFileBaseTests
{
    public abstract class WhenGettingTextNodes : WhenTestingAnXmlFile
    {
        protected string Result { get; set; }
        protected string XPath { get; set; }
        protected Exception Exception { get; set; }

        protected override void When()
        {
            Exception = null;
            try
            {
                Result = XmlFile.GetNodeText(XPath);
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
        }
    }

    class WhenGettingTextNodesThatExist : WhenGettingTextNodes
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            XPath = "xmlfile/element/subelement";
        }

        [Test]
        public void ItShouldReturnTheCorrectResult()
        {
            Assert.That(Result, Is.EqualTo("subelementvalue1"));
        }

        [Test]
        public void ItShouldNotThrow()
        {
            Assert.That(Exception, Is.Null);
        }
    }

    class WhenGettingTextNodesThatDoesNotExist : WhenGettingTextNodes
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            XPath = "xmlfile/element/XXXXX";
        }

        [Test]
        public void ItShouldThrow()
        {
            Assert.That(Exception, Is.InstanceOf<Exception>());
        }
    }
}

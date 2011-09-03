using System;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.Playlists.XmlFileBaseTests
{
	public abstract class WhenSettingTextNodes : WhenTestingAnXmlFile
    {
        protected string TextValue { get; set; }
        protected string Result { get; set; }
        protected string XPath { get; set; }
        protected Exception Exception { get; set; }

        protected override void When()
        {
            Exception = null;
            try
            {
                XmlFile.SetNodeText(XPath,TextValue);
            }
            catch (Exception ex)
            {
                Exception = ex;
            }

            if (Exception == null)
            {
                // any exception here should cause the test to fail
                Result = XmlFile.GetNodeText(XPath);
            }
        }
    }

    class WhenSettingTextNodesThatExist : WhenSettingTextNodes
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            XPath = "xmlfile/element/subelement";
            TextValue = "testreplacementvalue";
        }

        [Test]
        public void ItShouldReturnTheCorrectResult()
        {
            Assert.That(Result, Is.EqualTo("testreplacementvalue"));
        }

        [Test]
        public void ItShouldNotThrow()
        {
            Assert.That(Exception, Is.Null);
        }
    }

    class WhenSettingTextNodesThatDoesNotExist : WhenSettingTextNodes
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            XPath = "xmlfile/element/XXXX";
            TextValue = "testreplacementvalue";
        }

        [Test]
        public void ItShouldThrow()
        {
            Assert.That(Exception, Is.InstanceOf<Exception>());
        }
    }
}

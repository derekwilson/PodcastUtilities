using System;
using System.Xml;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.ControlFileTests.Creation
{
    public class WhenCreatingAnInvalidControlFile : WhenTestingAControlFile
    {
        protected Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            XmlNode n = ControlFileXmlDocument.SelectSingleNode("podcasts/global/sourceRoot");
            n.ParentNode.RemoveChild(n);
        }

        protected override void When()
        {
            ThrownException = null;
            try
            {
                ControlFile = new ReadOnlyControlFile(ControlFileXmlDocument);
            }
            catch (Exception exception)
            {
                ThrownException = exception;
            }
        }

        //[Test]
        //public void ItShouldThorw()
        //{
        //    Assert.That(ThrownException, Is.InstanceOf<Exception>());
        //}

    }


}

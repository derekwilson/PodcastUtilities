using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.ControlFileTests
{
    public class WhenCreatingAnInvalidControlFile : WhenTestingAControlFile
    {
        protected Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            XmlNode n = ControlFileXmlDocument.SelectSingleNode("podcasts/global/sourceRoot");
            n.ParentNode.RemoveChild(n);

            ControlFile = new ControlFile(ControlFileXmlDocument);
        }

        protected override void When()
        {
            ThrownException = null;
            try
            {
                var sourceRoot = ControlFile.SourceRoot;
            }
            catch (Exception exception)
            {
                ThrownException = exception;
            }
        }

        [Test]
        public void ItShouldThorw()
        {
            Assert.That(ThrownException, Is.InstanceOf<Exception>());
        }

    }


}

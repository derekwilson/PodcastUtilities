using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.ControlFileTests
{
    public abstract class WhenTestingAControlFile
        : WhenTestingBehaviour
    {
        protected IControlFile ControlFile { get; set; }
        protected string TestControlFileResourcePath { get; set; }
        public XmlDocument ControlFileXmlDocument { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            TestControlFileResourcePath = "PodcastUtilities.Common.Tests.XML.testcontrolfile.xml";

            Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(TestControlFileResourcePath);
            ControlFileXmlDocument = new XmlDocument();
            ControlFileXmlDocument.Load(s);
        }
    }
}

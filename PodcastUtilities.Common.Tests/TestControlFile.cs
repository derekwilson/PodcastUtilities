using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests
{
    public class TestControlFileFactory
    {
        public static IReadOnlyControlFile CreateControlFile()
        {
            var testControlFileResourcePath = "PodcastUtilities.Common.Tests.XML.testcontrolfile.xml";

            Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(testControlFileResourcePath);
            var controlFileXmlDocument = new XmlDocument();
            controlFileXmlDocument.Load(s);

            return new ReadOnlyControlFile(controlFileXmlDocument);
        }
    }
}

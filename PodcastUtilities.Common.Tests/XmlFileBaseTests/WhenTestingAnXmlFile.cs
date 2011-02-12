using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PodcastUtilities.Common.Tests.XmlFileBaseTests
{
    public class TestXmlFile : XmlFileBase
    {
        public TestXmlFile(string filename, bool create, string emptyResource) : base(filename,create,emptyResource)
        {
        }

        protected override System.IO.Stream GetXmlStream(string xmlfile)
        {
            // we need to do this because the xml file is in the tests assembly
            Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(xmlfile);
            return s;
        }

        public new string GetNodeText(string xpath)
        {
            return base.GetNodeText(xpath);
        }

        public new void SetNodeText(string xpath, string value)
        {
            base.SetNodeText(xpath,value);
        }
    }

    public abstract class WhenTestingAnXmlFile
        : WhenTestingBehaviour
    {
        protected TestXmlFile XmlFile { get; set; }

        protected string Filename { get; set; }
        protected string ResourcePath { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();
            Filename = "testrun.xml";       // we never actually save the file
            ResourcePath = "PodcastUtilities.Common.Tests.XML.testfile.xml";

            XmlFile = new TestXmlFile(Filename,true,ResourcePath);
        }
    }

}

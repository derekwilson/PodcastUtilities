using System.Reflection;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.Playlists.XmlFileBaseTests
{
    public class TestXmlFile : XmlFileBase
    {
        public TestXmlFile(string filename, bool create, string emptyResource) : base(filename,create,emptyResource,Assembly.GetExecutingAssembly())
        {
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

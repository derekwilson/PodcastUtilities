using System.IO;
using System.Reflection;
using System.Xml;

namespace PodcastUtilities.Common.Tests.XmlStateTests
{
    public abstract class WhenUsingTestState
        : WhenTestingBehaviour
    {
        protected XmlState _state;
        protected string _testStateFileResourcePath;
        protected XmlDocument _testXmlDocument;

        protected override void GivenThat()
        {
            base.GivenThat();

            _testStateFileResourcePath = "PodcastUtilities.Common.Tests.XML.teststate.xml";

            Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testStateFileResourcePath);
            _testXmlDocument = new XmlDocument();
            _testXmlDocument.Load(s);
        }
    }
}
using System.IO;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.DataObjectUriExtractorTests
{
    public class WhenDataObjectIsUrlFormat : WhenTestingDataObjectUriExtractor
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            // memory stream may be padded with \0 - we need to terminate correctly
            var addressBytes = Encoding.ASCII.GetBytes("http://www.abc.com/def\0\0\0\0");

            DataObject.Stub(data => data.GetData("UniformResourceLocator"))
                .Return(new MemoryStream(addressBytes));
        }

        [Test]
        public void ItShouldContainUri()
        {
            Assert.That(Extractor.ContainsUri(DataObject));
        }

        [Test]
        public void ItShouldExtractUri()
        {
            Assert.That(ExtractedUri, Is.EqualTo("http://www.abc.com/def"));
        }
    }
}
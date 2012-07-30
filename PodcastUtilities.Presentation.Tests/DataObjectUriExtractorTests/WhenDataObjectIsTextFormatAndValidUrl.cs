using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.DataObjectUriExtractorTests
{
    public class WhenDataObjectIsTextFormatAndValidUrl : WhenTestingDataObjectUriExtractor
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            DataObject.Stub(data => data.GetData("Text"))
                .Return("http://www.xyz.com/123");
        }

        [Test]
        public void ItShouldContainUri()
        {
            Assert.That(Extractor.ContainsUri(DataObject));
        }

        [Test]
        public void ItShouldExtractUri()
        {
            Assert.That(ExtractedUri, Is.EqualTo("http://www.xyz.com/123"));
        }
    }
}
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.DataObjectUriExtractorTests
{
    public class WhenDataObjectIsTextFormatButNotUrl : WhenTestingDataObjectUriExtractor
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            DataObject.Stub(data => data.GetData("Text"))
                .Return("Aardvark");
        }

        [Test]
        public void ItShouldNotContainUri()
        {
            Assert.That(!Extractor.ContainsUri(DataObject));
        }

        [Test]
        public void ItShouldNotBeAbleToExtractUri()
        {
            Assert.That(ExtractedUri, Is.Null);
        }
    }
}
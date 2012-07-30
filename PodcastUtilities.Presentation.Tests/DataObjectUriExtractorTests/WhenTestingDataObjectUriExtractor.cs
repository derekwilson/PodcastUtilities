using System.Windows;
using PodcastUtilities.Common.Tests;

namespace PodcastUtilities.Presentation.Tests.DataObjectUriExtractorTests
{
    public abstract class WhenTestingDataObjectUriExtractor
        : WhenTestingBehaviour
    {
        protected DataObjectUriExtractor Extractor { get; set; }

        protected IDataObject DataObject { get; set; }

        protected string ExtractedUri { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Extractor = new DataObjectUriExtractor();

            DataObject = GenerateMock<IDataObject>();
        }

        protected override void When()
        {
            ExtractedUri = Extractor.GetUri(DataObject);
        }
    }
}
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenAddingPodcastWithNonUriDataObject : WhenAddingPodcastWithDataObject
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            DataObjectUriExtractor.Stub(extractor => extractor.GetUri(DataObject))
                .Return(null);
        }


        [Test]
        public void ItShouldNotTryToUseTheDataObjectAsThePodcastAddress()
        {
            Assert.That(CreatedPodcast.Feed.Address, Is.Null);
        }
    }
}
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenAddingPodcastWithUriDataObject : WhenAddingPodcastWithDataObject
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            DataObjectUriExtractor.Stub(extractor => extractor.GetUri(DataObject))
                .Return("http://www.test.com/podcast.xml");
        }

        [Test]
        public void ItShouldCreatePodcastWithAddressFromTheDataObject()
        {
            Assert.That(CreatedPodcast.Feed.Address, Is.Not.Null);
            Assert.That(CreatedPodcast.Feed.Address.AbsoluteUri, Is.EqualTo("http://www.test.com/podcast.xml"));
        }
    }
}
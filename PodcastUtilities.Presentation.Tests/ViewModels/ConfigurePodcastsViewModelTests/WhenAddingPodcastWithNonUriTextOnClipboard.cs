using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenAddingPodcastWithNonUriTextOnClipboard : WhenExecutingAddPodcastCommand
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            ClipboardService.Stub(s => s.GetText())
                .Return("banana");
        }

        [Test]
        public void ItShouldNotTryToUseTheTextAsThePodcastAddress()
        {
            Assert.That(CreatedPodcast.Feed.Address, Is.Null);
        }
    }
}
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenAddingPodcastWithUriTextOnClipboard : WhenExecutingAddPodcastCommand
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            ClipboardService.Stub(s => s.GetText())
                .Return("http://www.blah.com/podcast.xml");
        }

        [Test]
        public void ItShouldCreatePodcastWithAddressFromTheClipboard()
        {
            Assert.That(CreatedPodcast.Feed.Address, Is.Not.Null);
            Assert.That(CreatedPodcast.Feed.Address.AbsoluteUri, Is.EqualTo("http://www.blah.com/podcast.xml"));
        }
    }
}
using NUnit.Framework;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenAddPodcastIsAccepted : WhenExecutingAddPodcastCommand
    {
        protected override bool EditPodcastDialogReturn
        {
            get { return true; }
        }

        [Test]
        public void ItShouldAddTheNewPodcastToTheCollection()
        {
            Assert.That(ViewModel.Podcasts.Count, Is.EqualTo(2));
            Assert.That(ViewModel.Podcasts[1], Is.SameAs(CreatedPodcastViewModel));
        }
    }
}
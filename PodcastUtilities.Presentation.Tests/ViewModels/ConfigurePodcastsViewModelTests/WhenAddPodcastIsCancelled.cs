using NUnit.Framework;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenAddPodcastIsCancelled : WhenExecutingAddPodcastCommand
    {
        protected override bool EditPodcastDialogReturn
        {
            get { return false; }
        }

        [Test]
        public void ItShouldNotAddTheNewPodcastToTheCollection()
        {
            Assert.That(ViewModel.Podcasts.Count, Is.EqualTo(1));
            Assert.That(!ViewModel.Podcasts.Contains(CreatedPodcastViewModel));
        }
    }
}
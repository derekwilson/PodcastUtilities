using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenEditPodcastIsAccepted : WhenExecutingEditPodcastCommand
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            DialogService.Stub(s => s.ShowEditPodcastDialog(SelectedPodcast))
                .WhenCalled(invocation => SelectedPodcast.Name = "New name")
                .Return(true);
        }

        [Test]
        public void ItShouldAcceptTheNewSettings()
        {
            SelectedPodcast.AssertWasCalled(p => p.AcceptEdit());
        }
    }
}
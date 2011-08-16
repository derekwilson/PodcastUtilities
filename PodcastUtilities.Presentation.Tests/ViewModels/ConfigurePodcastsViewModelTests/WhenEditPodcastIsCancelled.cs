using System;
using NUnit.Framework;
using PodcastUtilities.Presentation.ViewModels;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenEditPodcastIsCancelled : WhenExecutingEditPodcastCommand
    {
        protected PodcastViewModel OriginalPodcast { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            OriginalPodcast = new PodcastViewModel(null);

            DialogService.Stub(s => s.ShowEditPodcastDialog(SelectedPodcast))
                .WhenCalled(invocation => SelectedPodcast.Name = "New name")
                .Return(false);
        }

        [Test]
        public void ItShouldRevertToTheOriginalSettings()
        {
            SelectedPodcast.AssertWasCalled(p => p.CancelEdit());
        }
    }
}
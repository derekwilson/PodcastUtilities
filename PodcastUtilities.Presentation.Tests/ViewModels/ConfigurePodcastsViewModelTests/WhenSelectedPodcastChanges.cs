using NUnit.Framework;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Presentation.ViewModels;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenSelectedPodcastChanges
        : WhenTestingConfigurePodcastsViewModel
    {
        protected bool CanExecuteChangedRaised { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            ViewModel.EditPodcastCommand.CanExecuteChanged += 
                (sender, e) => CanExecuteChangedRaised = true;
        }

        protected override void When()
        {
            ViewModel.SelectedPodcast = new PodcastViewModel(new PodcastInfo(ControlFile));
        }

        [Test]
        public void ItShouldRaiseCanExecuteChangedOnEditPodcastCommand()
        {
            Assert.That(CanExecuteChangedRaised, Is.True);
        }
    }
}
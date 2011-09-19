using NUnit.Framework;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Presentation.ViewModels;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenSelectedPodcastIsNotNull
        : WhenTestingConfigurePodcastsViewModel
    {
        protected bool CanExecuteEdit { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            ViewModel.SelectedPodcast = new PodcastViewModel(new PodcastInfo(ControlFile));
        }

        protected override void When()
        {
            CanExecuteEdit = ViewModel.EditPodcastCommand.CanExecute(null);
        }

        [Test]
        public void ItShouldBeAbleToExecuteEditCommand()
        {
            Assert.That(CanExecuteEdit, Is.True);
        }
    }
}
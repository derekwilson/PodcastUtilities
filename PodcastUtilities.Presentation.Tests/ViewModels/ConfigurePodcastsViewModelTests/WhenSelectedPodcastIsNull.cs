using NUnit.Framework;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenSelectedPodcastIsNull
        : WhenTestingConfigurePodcastsViewModel
    {
        protected bool CanExecuteEdit { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            ViewModel.SelectedPodcast = null;
        }

        protected override void When()
        {
            CanExecuteEdit = ViewModel.EditPodcastCommand.CanExecute(null);
        }

        [Test]
        public void ItShouldNotBeAbleToExecuteEditCommand()
        {
            Assert.That(CanExecuteEdit, Is.False);
        }
    }
}
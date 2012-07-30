using NUnit.Framework;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenAskingCanExecuteAddPodcastWithNonDataObject : WhenAskingCanExecuteAddPodcastCommand
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            CommandParameter = "";
        }

        [Test]
        public void ItShouldBeAbleToExecuteAddCommand()
        {
            Assert.That(CanExecuteAdd);
        }
    }
}
using NUnit.Framework;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenAskingCanExecuteAddPodcastWithNull : WhenAskingCanExecuteAddPodcastCommand
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            CommandParameter = null;
        }

        [Test]
        public void ItShouldBeAbleToExecuteAddCommand()
        {
            Assert.That(CanExecuteAdd);
        }
    }
}
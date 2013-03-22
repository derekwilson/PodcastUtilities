using PodcastUtilities.Common.Tests;

namespace PodcastUtilities.Presentation.Tests.DelegateCommandTests
{
    public abstract class WhenTestingDelegateCommand
        : WhenTestingBehaviour
    {
        protected DelegateCommand Command { get; set; }

        protected object ExecuteParameter { get; set; }

        protected object CanExecuteParameter { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Command = new DelegateCommand(
                parameter => ExecuteParameter = parameter,
                parameter =>
                    {
                        CanExecuteParameter = parameter;
                        return true;
                    });
        }
    }
}
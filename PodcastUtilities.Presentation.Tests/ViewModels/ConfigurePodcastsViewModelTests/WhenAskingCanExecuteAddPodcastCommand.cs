namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public abstract class WhenAskingCanExecuteAddPodcastCommand
        : WhenTestingConfigurePodcastsViewModel
    {
        protected bool CanExecuteAdd { get; set; }

        protected object CommandParameter { get; set; }

        protected override void When()
        {
            CanExecuteAdd = ViewModel.AddPodcastCommand.CanExecute(CommandParameter);
        }
    }
}
using PodcastUtilities.Presentation.ViewModels;

namespace PodcastUtilities.Presentation.Services
{
	public interface IDialogService
	{
		bool ShowEditPodcastDialog(PodcastViewModel viewModel);
	}
}
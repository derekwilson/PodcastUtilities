using PodcastUtilities.Presentation.Services;
using PodcastUtilities.Presentation.ViewModels;

namespace PodcastUtilities.App.Services
{
	public class DialogServiceWpf
		: IDialogService
	{
		#region Implementation of IDialogService

		public bool ShowEditPodcastDialog(PodcastViewModel viewModel)
		{
			var editDialog = new EditPodcastWindow(viewModel);

			return editDialog.ShowDialog().GetValueOrDefault(false);
		}

		#endregion
	}
}
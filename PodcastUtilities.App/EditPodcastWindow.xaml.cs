using System.Windows;
using PodcastUtilities.Presentation.ViewModels;

namespace PodcastUtilities.App
{
	/// <summary>
	/// Interaction logic for EditPodcastWindow.xaml
	/// </summary>
	public partial class EditPodcastWindow : Window
	{
		private readonly PodcastViewModel _originalPodcastViewModel;

		public EditPodcastWindow(PodcastViewModel podcastViewModel)
		{
			//_originalPodcastViewModel = podcastViewModel.Clone();

			DataContext = new EditPodcastViewModel(podcastViewModel);

			InitializeComponent();
		}
	}
}

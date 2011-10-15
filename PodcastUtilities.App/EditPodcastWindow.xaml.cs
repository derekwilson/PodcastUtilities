using System.Windows;
using PodcastUtilities.Presentation.ViewModels;

namespace PodcastUtilities.App
{
	/// <summary>
	/// Interaction logic for EditPodcastWindow.xaml
	/// </summary>
	public partial class EditPodcastWindow : Window
	{
		public EditPodcastWindow(PodcastViewModel podcastViewModel)
		{
			DataContext = new EditPodcastViewModel(podcastViewModel);

			InitializeComponent();
		}
	}
}

using System.Windows;
using PodcastUtilities.Common;
using PodcastUtilities.Presentation.Services;
using PodcastUtilities.Presentation.ViewModels;

namespace PodcastUtilities.App
{
    /// <summary>
    /// Interaction logic for ConfigurePodcastsWindow.xaml
    /// </summary>
    public partial class ConfigurePodcastsWindow : Window
    {
        public ConfigurePodcastsWindow()
        {
            InitializeComponent();

        	var viewModel = new ConfigurePodcastsViewModel(
        		AppIocContainer.Container.Resolve<IControlFileFactory>(),
        		AppIocContainer.Container.Resolve<IBrowseForFileService>());

            DataContext = viewModel;
        }
    }
}

using System.Windows;
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

            var viewModel = AppIocContainer.Container.Resolve<ConfigurePodcastsViewModel>();

            DataContext = viewModel;
        }
    }
}

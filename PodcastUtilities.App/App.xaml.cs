using System.Windows;

namespace PodcastUtilities.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    	public App()
    	{
    		AppIocContainer.Initialize();
    	}
    }
}

using System.Windows;
using PodcastUtilities.Presentation.Services;

namespace PodcastUtilities.App.Services
{
	public class ApplicationServiceWpf
		: IApplicationService
	{
		#region Implementation of IApplicationService

		public void ShutdownApplication()
		{
			Application.Current.Shutdown();
		}

		#endregion
	}
}
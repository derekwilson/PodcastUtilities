using PodcastUtilities.App.Services;
using PodcastUtilities.Common;
using PodcastUtilities.Ioc;
using PodcastUtilities.Presentation.Services;

namespace PodcastUtilities.App
{
	public static class AppIocContainer
	{
		public static IIocContainer Container { get; private set; }

		public static void Initialize()
		{
			Container = new LinFuIocContainer();

			IocRegistration.RegisterFileServices(Container);
			IocRegistration.RegisterSystemServices(Container);

			RegisterPresentationServices();
		}

		private static void RegisterPresentationServices()
		{
			Container.Register<IApplicationService, ApplicationServiceWpf>();
			Container.Register<IBrowseForFileService, BrowseForFileServiceWpf>();
			Container.Register<IDialogService, DialogServiceWpf>();
		}
	}
}
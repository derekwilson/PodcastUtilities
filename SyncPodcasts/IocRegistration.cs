using PodcastUtilities.Common;
using PodcastUtilities.Common.IO;

namespace SyncPodcasts
{
	public static class IocRegistration
	{
		public static void RegisterServices(IIocContainer container)
		{
			container.Register<IDriveInfoProvider, SystemDriveInfoProvider>();
			container.Register<IDirectoryInfoProvider, SystemDirectoryInfoProvider>();
			container.Register<IFileUtilities, FileUtilities>();
			container.Register<IFileCopier, FileCopier>();
			container.Register<IFileFinder, FileFinder>();
			container.Register<IUnwantedFileRemover, UnwantedFileRemover>();
		}
	}
}

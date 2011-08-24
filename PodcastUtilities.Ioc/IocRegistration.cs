using PodcastUtilities.Common;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Ioc
{
	public static class IocRegistration
	{
		public static void RegisterFileServices(IIocContainer container)
		{
			container.Register<IDriveInfoProvider, SystemDriveInfoProvider>();
			container.Register<IDirectoryInfoProvider, SystemDirectoryInfoProvider>();
			container.Register<IFileUtilities, FileUtilities>();
			container.Register<IFileCopier, FileCopier>();
			container.Register<IFileFinder, FileFinder>();
			container.Register<IFileSorter, FileSorter>();
            container.Register<IUnwantedFileRemover, UnwantedFileRemover>();
            container.Register<IPodcastEpisodePurger, PodcastEpisodePurger>();
            container.Register<IControlFileFactory, ControlFileFactory>();
        }

        public static void RegisterSystemServices(IIocContainer container)
        {
            container.Register<ITimeProvider, SystemDateTimeProvider>();
        }

        public static void RegisterPlaylistServices(IIocContainer container)
        {
            container.Register<IPlaylistFactory, PlaylistFactory>();
        }

        public static void RegisterPodcastServices(IIocContainer container)
        {
            container.Register<IPodcastFactory, PodcastFactory>();
            container.Register<IPodcastDefaultsProvider, HardcodedPodcastDefaultsProvider>();
        }

        public static void RegisterFeedServices(IIocContainer container)
        {
            container.Register<IPodcastFeedFactory, PodcastFeedFactory>();
            container.Register<IWebClientFactory, WebClientFactory>();
            container.Register<IPodcastFeedEpisodeFinder, PodcastFeedEpisodeFinder>();
            container.Register<IFeedSyncItemToPodcastEpisodeDownloaderTaskConverter, FeedSyncItemToPodcastEpisodeDownloaderTaskConverter>();
            container.Register<IPodcastEpisodeDownloaderFactory, PodcastEpisodeDownloaderFactory>();
            container.Register<ITaskPool, TaskPool>();
            container.Register<IStateProvider, StateProvider>();
        }
    }
}

#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
using System;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Perfmon;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Common.Playlists;
using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Ioc
{
	public static class IocRegistration
	{
        [CLSCompliant(false)]
        public static void RegisterFileServices(IIocContainer container)
		{
			container.Register<IDriveInfoProvider, FileSystemAwareDriveInfoProvider>();
			container.Register<IDirectoryInfoProvider, FileSystemAwareDirectoryInfoProvider>();
			container.Register<IFileInfoProvider, FileSystemAwareFileInfoProvider>();
			container.Register<IFileUtilities, FileSystemAwareFileUtilities>();
			container.Register<IPathUtilities, FileSystemAwarePathUtilities>();
			container.Register<ICopier, Copier>();
			container.Register<IFinder, Finder>();
			container.Register<ISorter, Sorter>();
            container.Register<IUnwantedFileRemover, UnwantedFileRemover>();
            container.Register<IUnwantedFolderRemover, UnwantedFolderRemover>();
            container.Register<IEpisodePurger, EpisodePurger>();
            container.Register<IControlFileFactory, ControlFileFactory>();
            container.Register<IStreamHelper, StreamHelper>();
        }

        [CLSCompliant(false)]
        public static void RegisterPortableDeviceServices(IIocContainer container)
        {
            container.Register<IDeviceManager, DeviceManager>(IocLifecycle.Singleton);
        }

        [CLSCompliant(false)]
        public static void RegisterSystemServices(IIocContainer container)
        {
            container.Register<ITimeProvider, SystemDateTimeProvider>();
            container.Register<IPerfmonCounterUtilities, SystemPerfmonCounterUtilities>();
            container.Register<IPerfmonCounterCreationDataProvider, SystemPerfmonCounterCreationDataProvider>();
            container.Register<ICategoryInstaller, CategoryInstaller>();
            container.Register<ICounterFactory, CounterFactory>();
            container.Register<ICommandExecuter, WindowsCommandExecuter>();
            container.Register<IEnvironmentInformationProvider, WindowsEnvironmentInformationProvider>();
        }

        [CLSCompliant(false)]
        public static void RegisterPlaylistServices(IIocContainer container)
        {
            container.Register<IPlaylistFactory, PlaylistFactory>();
        }

        [CLSCompliant(false)]
        public static void RegisterPodcastServices(IIocContainer container)
        {
            container.Register<IPodcastFactory, PodcastFactory>();
        }

        [CLSCompliant(false)]
        public static void RegisterFeedServices(IIocContainer container)
        {
            container.Register<ICommandGenerator, CommandGenerator>();
            container.Register<IPodcastFeedFactory, PodcastFeedFactory>();
            container.Register<IWebClientFactory, WebClientFactory>();
            container.Register<IEpisodeFinder, EpisodeFinder>();
            container.Register<ISyncItemToEpisodeDownloaderTaskConverter, SyncItemToEpisodeDownloaderTaskConverter>();
            container.Register<IEpisodeDownloaderFactory, EpisodeDownloaderFactory>();
            container.Register<ITaskPool, TaskPool>();
            container.Register<IStateProvider, StateProvider>();
        }
    }
}

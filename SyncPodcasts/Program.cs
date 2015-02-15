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
using System.Reflection;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Common.Playlists;
using PodcastUtilities.Ioc;

namespace SyncPodcasts
{
	class Program
	{
		static private void DisplayBanner()
		{
			// do not move the GetExecutingAssembly call from here into a supporting DLL
			Assembly me = System.Reflection.Assembly.GetExecutingAssembly();
			AssemblyName name = me.GetName();
			Console.WriteLine("SyncPodcasts v{0}", name.Version);
		}

		static private void DisplayHelp()
		{
			Console.WriteLine("Usage: SyncPodcasts <controlfile>");
			Console.WriteLine("Where");
			Console.WriteLine("  <controlfile> = XML control file eg. podcasts.xml");
		}

		static void Main(string[] args)
		{
			DisplayBanner();
			if (args.Length < 1)
			{
				DisplayHelp();
				return;
			}

			LinFuIocContainer iocContainer = InitializeIocContainer();

			ReadOnlyControlFile control = new ReadOnlyControlFile(args[0]);
			IFinder finder = iocContainer.Resolve<IFinder>();
			ICopier copier = iocContainer.Resolve<ICopier>();
			IUnwantedFileRemover remover = iocContainer.Resolve<IUnwantedFileRemover>();
		    IUnwantedFolderRemover folderRemover = iocContainer.Resolve<IUnwantedFolderRemover>();
			IFileUtilities fileUtilities = iocContainer.Resolve<IFileUtilities>();
			IPathUtilities pathUtilities = iocContainer.Resolve<IPathUtilities>();
			IPlaylistFactory playlistFactory = iocContainer.Resolve<IPlaylistFactory>();

            Generator generator = new Generator(finder, fileUtilities, pathUtilities, playlistFactory);
            generator.StatusUpdate += new EventHandler<StatusUpdateEventArgs>(StatusUpdate);

			Synchronizer synchronizer = new Synchronizer(finder, copier, remover, folderRemover);
			synchronizer.StatusUpdate += new EventHandler<StatusUpdateEventArgs>(StatusUpdate);

			synchronizer.Synchronize(control, false);

			if (!string.IsNullOrEmpty(control.GetPlaylistFileName()))
				generator.GeneratePlaylist(control, true);
		}

		private static LinFuIocContainer InitializeIocContainer()
		{
			LinFuIocContainer container =  new LinFuIocContainer();

            IocRegistration.RegisterPortableDeviceServices(container);
            IocRegistration.RegisterFileServices(container);
            IocRegistration.RegisterPlaylistServices(container);

			return container;
		}

		static void StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            // maybe we want to optionally filter verbose message
            Console.WriteLine(e.Message);
        }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

using PodcastUtilities.Common;
using System.Xml;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Common.Playlists;
using PodcastUtilities.Ioc;

namespace GeneratePlaylist
{
	class Program
	{
		static private void DisplayBanner()
		{
			// do not move the GetExecutingAssembly call from here into a supporting DLL
			Assembly me = System.Reflection.Assembly.GetExecutingAssembly();
			AssemblyName name = me.GetName();
			Console.WriteLine("GeneratePlaylist v{0}", name.Version);
		}

        static private void DisplayHelp()
        {
            Console.WriteLine("Usage: GeneratePlaylist <controlfile>");
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

            var control = new ReadOnlyControlFile(args[0]);
            var finder = iocContainer.Resolve<IFinder>();
            var fileUtilities = iocContainer.Resolve<IFileUtilities>();
            var playlistFactory = iocContainer.Resolve<IPlaylistFactory>();

			var generator = new Generator(finder, fileUtilities, playlistFactory);
            generator.StatusUpdate += new EventHandler<StatusUpdateEventArgs>(GeneratorStatusUpdate);

            if (!string.IsNullOrEmpty(control.PlaylistFileName))
                generator.GeneratePlaylist(control, false);
        }

        private static LinFuIocContainer InitializeIocContainer()
        {
            var container = new LinFuIocContainer();

            IocRegistration.RegisterFileServices(container);
            IocRegistration.RegisterPlaylistServices(container);

            return container;
        }

        static void GeneratorStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            // maybe we want to optionally filter verbose message
            Console.WriteLine(e.Message);
        }
    }
}

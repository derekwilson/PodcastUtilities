using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml;

using PodcastUtilities.Common;

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

			ControlFile control = new ControlFile(args[0]);
            FileFinder finder = new FileFinder();
            finder.StatusUpdate += new EventHandler<StatusUpdateEventArgs>(StatusUpdate);
            FileCopier copier = new FileCopier();
            copier.StatusUpdate += new EventHandler<StatusUpdateEventArgs>(StatusUpdate);
            PlaylistGenerator generator = new PlaylistGenerator();
            generator.StatusUpdate += new EventHandler<StatusUpdateEventArgs>(StatusUpdate);

			List<SyncItem> sourceFiles = finder.GetAllSourceFiles(control,true);

			copier.CopyFilesToTarget(sourceFiles, control, false);

			if (!string.IsNullOrEmpty(control.PlaylistFilename))
				generator.GeneratePlaylist(control, true);
		}

        static void StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            // maybe we want to optionally filter verbose message
            Console.WriteLine(e.Message);
        }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

using PodcastUtilities.Common;
using System.Xml;

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

            ControlFile control = new ControlFile(args[0]);
            PlaylistGenerator generator = new PlaylistGenerator(new FileFinder());
            generator.StatusUpdate += new EventHandler<StatusUpdateEventArgs>(generator_StatusUpdate);

            if (!string.IsNullOrEmpty(control.PlaylistFilename))
                generator.GeneratePlaylist(control, false);
        }

        static void generator_StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            // maybe we want to optionally filter verbose message
            Console.WriteLine(e.Message);
        }
    }
}

using System;
using System.Reflection;

namespace PodcastUtilities.Integration.Tests
{
    class Program
    {
        static private void DisplayBanner()
        {
            // do not move the GetExecutingAssembly call from here into a supporting DLL
            Assembly me = System.Reflection.Assembly.GetExecutingAssembly();
            AssemblyName name = me.GetName();
            Console.WriteLine("PodcastUtilities.Integration.Tests v{0}", name.Version);
        }

        static private void DisplayHelp()
        {
            Console.WriteLine("Usage: PodcastUtilities.Integration.Tests <controlfile>");
            Console.WriteLine("Where");
            Console.WriteLine("  <controlfile> = (optional) XML control file eg. podcasts.xml");
        }

        static void Main(string[] args)
        {
            DisplayBanner();
            string controlFilename = null;
            if (args.Length < 1)
            {
                DisplayHelp();
            }
            else
            {
                controlFilename = args[0];
            }

            var controlFileTests = new ControlFile.Runner(controlFilename);
            controlFileTests.RunAllTests();

            var portableDevicesTests = new PortableDevices.Runner(controlFilename);
            portableDevicesTests.RunAllTests();

            Console.WriteLine("Done");
        }
    }
}

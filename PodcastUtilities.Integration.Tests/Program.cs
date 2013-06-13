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
            Console.WriteLine("  <tests> = all: run all tests");
            Console.WriteLine("          = mtp: run portable device tests");
            Console.WriteLine("          = config: run config file tests");
        }

        static void Main(string[] args)
        {
            DisplayBanner();
            string testToRun = null;
            if (args.Length < 1)
            {
                DisplayHelp();
                return;
            }
            else
            {
                testToRun = args[0];
            }

            var controlFileTests = new ControlFile.Runner(testToRun);
            controlFileTests.RunAllTests();

            var portableDevicesTests = new PortableDevices.Runner(testToRun);
            portableDevicesTests.RunAllTests();

            Console.WriteLine("Done");
        }
    }
}

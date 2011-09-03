using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Ioc;

namespace PurgePodcasts
{
    class Program
    {
        static LinFuIocContainer _iocContainer;
        static ControlFile _control;
        private static bool _quiet = false;

        static private void DisplayBanner()
        {
            // do not move the GetExecutingAssembly call from here into a supporting DLL
            Assembly me = System.Reflection.Assembly.GetExecutingAssembly();
            AssemblyName name = me.GetName();
            Console.WriteLine("PurgePodcasts v{0}", name.Version);
        }

        static private void DisplayHelp()
        {
            Console.WriteLine("Usage: PurgePodcasts <controlfile>");
            Console.WriteLine("Where");
            Console.WriteLine("  <controlfile> = XML control file eg. podcasts.xml");
        }

        private static LinFuIocContainer InitializeIocContainer()
        {
            var container = new LinFuIocContainer();

            IocRegistration.RegisterSystemServices(container);
            IocRegistration.RegisterFileServices(container);

            return container;
        }

        static void Main(string[] args)
        {
            DisplayBanner();
            if (args.Length < 1)
            {
                DisplayHelp();
                return;
            }

            _iocContainer = InitializeIocContainer();
            _control = new ControlFile(args[0]);
            if (args.Count() > 1)
            {
                _quiet = args[1].Contains('q');
            }

            // find the episodes to delete
            var allFilesToDelete = new List<IFileInfo>(20);
            var podcastEpisodePurger = _iocContainer.Resolve<IEpisodePurger>();
            foreach (var podcastInfo in _control.Podcasts)
            {
                var filesToDeleteFromThisFeed = podcastEpisodePurger.FindEpisodesToPurge(_control.SourceRoot, podcastInfo);
                allFilesToDelete.AddRange(filesToDeleteFromThisFeed);
            }

            if (allFilesToDelete.Count == 0)
            {
                Console.WriteLine("There are no files to delete");
                return;
            }

            var fileUtilities = _iocContainer.Resolve<IFileUtilities>();
            if (_quiet)
            {
                foreach (var fileInfo in allFilesToDelete)
                {
                    Console.WriteLine("Deleted: {0}", fileInfo.FullName);
                    fileUtilities.FileDelete(fileInfo.FullName);
                }
            }
            else
            {
                foreach (var fileInfo in allFilesToDelete)
                {
                    Console.WriteLine("{0}", fileInfo.FullName);
                }
                Console.WriteLine("OK to delete ALL the above files? (y/n) ");
                string answer;
                do
                {
                    char key = Convert.ToChar(Console.Read());
                    answer = key.ToString().ToLower();
                } while (answer != "y" && answer != "n");
                if (answer == "y")
                {
                    Console.WriteLine("Deleting {0} files",allFilesToDelete.Count);
                    foreach (var fileInfo in allFilesToDelete)
                    {
                        fileUtilities.FileDelete(fileInfo.FullName);
                    }
                }
                else
                {
                    Console.WriteLine("No files deleted");
                }
            }

            Console.WriteLine("Done");
        }
    }
}

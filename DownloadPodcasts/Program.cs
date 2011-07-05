using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using PodcastUtilities.Common;
using PodcastUtilities.Ioc;

namespace DownloadPodcasts
{
    class Program
    {
        private static ITaskPool _taskPool;
        static object _synclock = new object();
        private static bool _verbose = false;

        static private void DisplayBanner()
        {
            // do not move the GetExecutingAssembly call from here into a supporting DLL
            Assembly me = System.Reflection.Assembly.GetExecutingAssembly();
            AssemblyName name = me.GetName();
            Console.WriteLine("DownloadPodcasts v{0}", name.Version);
        }

        static private void DisplayHelp()
        {
            Console.WriteLine("Usage: DownloadPodcasts <controlfile>");
            Console.WriteLine("Where");
            Console.WriteLine("  <controlfile> = XML control file eg. podcasts.xml");
        }

        private static LinFuIocContainer InitializeIocContainer()
        {
            var container = new LinFuIocContainer();

            IocRegistration.RegisterSystemServices(container);
            IocRegistration.RegisterFileServices(container);
            IocRegistration.RegisterFeedServices(container);

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

            LinFuIocContainer iocContainer = InitializeIocContainer();

            var control = new ControlFile(args[0]);
            if (args.Count() > 1)
            {
                _verbose = args[1].Contains('v');
            }

            int numberOfConnections = control.MaximumNumberOfConcurrentDownloads;
            System.Net.ServicePointManager.DefaultConnectionLimit = numberOfConnections;

            // find the episodes to download
            var episodes = new List<IFeedSyncItem>(20);
            var podcastEpisodeFinder = iocContainer.Resolve<IPodcastFeedEpisodeFinder>();
            podcastEpisodeFinder.StatusUpdate += StatusUpdate;
            foreach (var podcastInfo in control.Podcasts)
            {
                podcastEpisodeFinder.FindEpisodesToDownload(control.SourceRoot, podcastInfo, episodes);
            }

            if (episodes.Count > 0)
            {
                // convert them to tasks
                var converter = iocContainer.Resolve<IFeedSyncItemToPodcastEpisodeDownloaderTaskConverter>();
                IPodcastEpisodeDownloader[] downloadTasks = converter.ConvertItemsToTasks(episodes, StatusUpdate, ProgressUpdate);

                // run them in a task pool
                _taskPool = iocContainer.Resolve<ITaskPool>();
                Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
                _taskPool.RunAllTasks(numberOfConnections, downloadTasks);
            }

            Console.WriteLine("Done");
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("CTRL C pressed");
            if (_taskPool != null)
            {
                _taskPool.CancelAllTasks();
            }
            e.Cancel = true;
        }

        static void ProgressUpdate(object sender, ProgressEventArgs e)
        {
            IFeedSyncItem syncItem = e.UserState as IFeedSyncItem;
            if (e.ProgressPercentage % 10 == 0)
            {
                Console.WriteLine(string.Format("{0} ({1} of {2}) {3}%", syncItem.EpisodeTitle,
                                                DisplayFormatter.RenderFileSize(e.ItemsProcessed),
                                                DisplayFormatter.RenderFileSize(e.TotalItemsToProcess),
                                                e.ProgressPercentage));
            }
        }

        static void StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            if (e.MessageLevel == StatusUpdateEventArgs.Level.Verbose && !_verbose)
            {
                return;
            }

            if (e.Exception != null)
            {
                lock (_synclock)
                {
                    // keep all the message together
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.Exception.StackTrace);
                }
            }
            else
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

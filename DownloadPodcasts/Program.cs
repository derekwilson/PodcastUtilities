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
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Ioc;

namespace DownloadPodcasts
{
    class Program
    {
        private static ITaskPool _taskPool;
        static object _synclock = new object();
        private static bool _verbose = false;
        static LinFuIocContainer _iocContainer;
        static IDriveInfoProvider _driveInfoProvider;
        static ControlFile _control;

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

            Console.WriteLine("Started - {0}", DateTime.Now.ToString());

            _iocContainer = InitializeIocContainer();
            _control = new ControlFile(args[0]);
            if (args.Count() > 1)
            {
                _verbose = args[1].Contains('v');
            }

            _driveInfoProvider = _iocContainer.Resolve<IDriveInfoProvider>();

            int numberOfConnections = _control.MaximumNumberOfConcurrentDownloads;
            System.Net.ServicePointManager.DefaultConnectionLimit = numberOfConnections;

            // find the episodes to download
            var allEpisodes = new List<IFeedSyncItem>(20);
            var podcastEpisodeFinder = _iocContainer.Resolve<IPodcastFeedEpisodeFinder>();
            podcastEpisodeFinder.StatusUpdate += StatusUpdate;
            foreach (var podcastInfo in _control.Podcasts)
            {
                var episodesInThisFeed = podcastEpisodeFinder.FindEpisodesToDownload(_control.SourceRoot, podcastInfo);
                allEpisodes.AddRange(episodesInThisFeed);
            }

            if (allEpisodes.Count > 0)
            {
                // convert them to tasks
                var converter = _iocContainer.Resolve<IFeedSyncItemToPodcastEpisodeDownloaderTaskConverter>();
                IPodcastEpisodeDownloader[] downloadTasks = converter.ConvertItemsToTasks(allEpisodes, StatusUpdate, ProgressUpdate);

                // run them in a task pool
                _taskPool = _iocContainer.Resolve<ITaskPool>();
                Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
                _taskPool.RunAllTasks(numberOfConnections, downloadTasks);
            }

            Console.WriteLine("Done - {0}",DateTime.Now.ToString());
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
            if (IsDestinationDriveFull(_control.SourceRoot,_control.FreeSpaceToLeaveOnDestination))
            {
                if (_taskPool != null)
                {
                    _taskPool.CancelAllTasks();
                }
            }
        }

        static bool IsDestinationDriveFull(string destinationRootPath, long freeSpaceToLeaveOnDestination)
        {
            var driveInfo = _driveInfoProvider.GetDriveInfo(Path.GetPathRoot(Path.GetFullPath(destinationRootPath)));
            long availableFreeSpace = driveInfo.AvailableFreeSpace;

            long freeKb = 0;
            double freeMb = 0;
            if (availableFreeSpace > 0)
                freeKb = (availableFreeSpace / 1024);
            if (freeKb > 0)
                freeMb = (freeKb / 1024);

            if (freeMb < freeSpaceToLeaveOnDestination)
            {
                Console.WriteLine(string.Format("Destination drive is full leaving {0:#,0.##} MB free", freeMb));
                return true;
            }
            return false;
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

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
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
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
        static ReadOnlyControlFile _control;
        private static int _number_of_files_to_download;
        private static int _number_of_files_downloaded;

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

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Started - {0}", DateTime.Now.ToString());
            Console.ResetColor();

            _iocContainer = InitializeIocContainer();
            _control = new ReadOnlyControlFile(args[0]);
            if (args.Count() > 1)
            {
                _verbose = args[1].Contains('v');
            }

            _driveInfoProvider = _iocContainer.Resolve<IDriveInfoProvider>();

            int numberOfConnections = _control.MaximumNumberOfConcurrentDownloads;
            System.Net.ServicePointManager.DefaultConnectionLimit = numberOfConnections;

            // find the episodes to download
            var allEpisodes = new List<ISyncItem>(20);
            var podcastEpisodeFinder = _iocContainer.Resolve<IEpisodeFinder>();
            podcastEpisodeFinder.StatusUpdate += StatusUpdate;
            foreach (var podcastInfo in _control.Podcasts)
            {
                var episodesInThisFeed = podcastEpisodeFinder.FindEpisodesToDownload(_control.SourceRoot, _control.RetryWaitInSeconds, podcastInfo);
                allEpisodes.AddRange(episodesInThisFeed);
            }

            _number_of_files_to_download = allEpisodes.Count;
            _number_of_files_downloaded = 0;
            if (_number_of_files_to_download > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Downloading {0} episodes", _number_of_files_to_download);
                Console.ResetColor();

                // convert them to tasks
                var converter = _iocContainer.Resolve<ISyncItemToEpisodeDownloaderTaskConverter>();
                IEpisodeDownloader[] downloadTasks = converter.ConvertItemsToTasks(allEpisodes, StatusUpdate, ProgressUpdate);

                // run them in a task pool
                _taskPool = _iocContainer.Resolve<ITaskPool>();
                Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
                _taskPool.RunAllTasks(numberOfConnections, downloadTasks);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done - {0}", DateTime.Now.ToString());
            Console.ResetColor();
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
            ISyncItem syncItem = e.UserState as ISyncItem;
            if (e.ProgressPercentage % 10 == 0)
            {
                Console.WriteLine(string.Format("{0} ({1} of {2}) {3}%", syncItem.EpisodeTitle,
                                                DisplayFormatter.RenderFileSize(e.ItemsProcessed),
                                                DisplayFormatter.RenderFileSize(e.TotalItemsToProcess),
                                                e.ProgressPercentage));
            }
            if (e.ProgressPercentage == 100)
            {
                _number_of_files_downloaded++;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Completed {0} of {1} downloads",_number_of_files_downloaded, _number_of_files_to_download);
                Console.ResetColor();
            }

            if (IsDestinationDriveFull(_control.SourceRoot,_control.FreeSpaceToLeaveOnDownload))
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
            if (e.MessageLevel == StatusUpdateLevel.Verbose && !_verbose)
            {
                return;
            }

            if (e.Exception != null)
            {
                lock (_synclock)
                {
                    // keep all the message together
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.Exception.StackTrace);
                    Console.ResetColor();
                }
            }
            else
            {
                if (e.MessageLevel == StatusUpdateLevel.Error)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (e.MessageLevel == StatusUpdateLevel.Warning)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }
        }
    }
}

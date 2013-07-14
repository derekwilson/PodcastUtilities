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
using PodcastUtilities.Common.Perfmon;
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
        private static bool _reported_driveinfo_error = false;

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
            IocRegistration.RegisterPortableDeviceServices(container);
            IocRegistration.RegisterFileServices(container);
            IocRegistration.RegisterFeedServices(container);

            return container;
        }

        private static void ResetCounters()
        {
            var factory = _iocContainer.Resolve<ICounterFactory>();

            var aveCounter1 = factory.CreateAverageCounter(CategoryInstaller.PodcastUtilitiesCommonCounterCategory,
                                                           CategoryInstaller.AverageTimeToDownload,
                                                           CategoryInstaller.NumberOfDownloads);
            aveCounter1.Reset();
            var aveCounter2 = factory.CreateAverageCounter(CategoryInstaller.PodcastUtilitiesCommonCounterCategory,
                                                           CategoryInstaller.AverageMBDownload,
                                                           CategoryInstaller.SizeOfDownloads);
            aveCounter2.Reset();
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
            _verbose = _control.GetDiagnosticOutput() == DiagnosticOutputLevel.Verbose;

            _driveInfoProvider = _iocContainer.Resolve<IDriveInfoProvider>();

            int numberOfConnections = _control.GetMaximumNumberOfConcurrentDownloads();
            System.Net.ServicePointManager.DefaultConnectionLimit = numberOfConnections;

            ResetCounters();

            // find the episodes to download
            var allEpisodes = new List<ISyncItem>(20);
            var podcastEpisodeFinder = _iocContainer.Resolve<IEpisodeFinder>();
            podcastEpisodeFinder.StatusUpdate += StatusUpdate;
            foreach (var podcastInfo in _control.GetPodcasts())
            {
                var episodesInThisFeed = podcastEpisodeFinder.FindEpisodesToDownload(_control.GetSourceRoot(), _control.GetRetryWaitInSeconds(), podcastInfo, _control.GetDiagnosticRetainTemporaryFiles());
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
            lock (_synclock)
            {
                // keep all the message together

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

                if (IsDestinationDriveFull(_control.GetSourceRoot(),_control.GetFreeSpaceToLeaveOnDownload()))
                {
                    if (_taskPool != null)
                    {
                        _taskPool.CancelAllTasks();
                    }
                }
            }
        }

        static bool IsDestinationDriveFull(string destinationRootPath, long freeSpaceToLeaveOnDestination)
        {
            long availableFreeSpace = 0;
            try
            {
                var driveInfo = _driveInfoProvider.GetDriveInfoForPath(Path.GetPathRoot(Path.GetFullPath(destinationRootPath)));
                availableFreeSpace = driveInfo.AvailableFreeSpace;
            }
            catch (Exception ex)
            {
                if (!_reported_driveinfo_error)
                {
                    Console.WriteLine(string.Format("Cannot find available free space on drive, will continue to download. Error: {0}", ex.Message));
                }
                _reported_driveinfo_error = true;
                return false;
            }

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

            lock (_synclock)
            {
                // keep all the message together
                if (e.Exception != null)
                {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(e.Exception.ToString());
                        Console.ResetColor();
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
}

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

        static Stream GetStreamUsingWebClient(string url)
        {
            using (WebClient client = new System.Net.WebClient())
            {
                // some servers can die without a user-agent
                client.Headers.Add("User-Agent", "Mozilla/4.0+");
                return client.OpenRead(url);
            }
        }

        static void GetFileUsingWebClient(string url, string filename)
        {
            using (WebClient client = new System.Net.WebClient())
            {
                // some servers can die without a user-agent
                client.Headers.Add("User-Agent", "Mozilla/4.0+"); 
                client.DownloadFile(url, filename);
            }
        }

        static void GetFileUsingWebClientAsync(FeedSyncItem syncItem)
        {
            using (WebClient client = new System.Net.WebClient())
            {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(syncItem.EpisodeUrl, syncItem.DestinationPath, syncItem);
                while (client.IsBusy)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        static void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var syncItem = e.UserState as FeedSyncItem;
            if (syncItem == null)
            {
                throw new Exception("Missing token from download completed");
            }
            lock (_synclock)
            {
                Console.WriteLine("\nCompleted: {0}", syncItem.EpisodeTitle);
                if (e.Cancelled)
                {
                    Console.WriteLine("Download Cancelled.");
                }
                else if (e.Error != null && e.Error.InnerException != null)
                {
                    Console.WriteLine("Error: {0} {1}", e.Error.InnerException.Message,
                                      e.Error.InnerException.StackTrace);
                }
                else if (e.Error != null)
                {
                    Console.WriteLine("Error: {0} {1}", e.Error.Message, e.Error.StackTrace);
                }
            }
        }

        static void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.Write("\rDownloaded {0}%",e.ProgressPercentage);
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

            var episodes = new List<IFeedSyncItem>(20);
            var podcastEpisodeFinder = iocContainer.Resolve<IPodcastFeedEpisodeFinder>();
            podcastEpisodeFinder.StatusUpdate += StatusUpdate;
            foreach (var podcastInfo in control.Podcasts)
            {
                podcastEpisodeFinder.FindEpisodesToDownload(control.SourceRoot, podcastInfo, episodes);
            }

            if (episodes.Count > 0)
            {
                var converter = iocContainer.Resolve<IFeedSyncItemToPodcastEpisodeDownloaderTaskConverter>();
                IPodcastEpisodeDownloader[] downloadTasks = converter.ConvertItemsToTasks(episodes, StatusUpdate);

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

        static void StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            if (e.MessageLevel == StatusUpdateEventArgs.Level.Verbose && !_verbose)
            {
                return;
            }

            if (e.MessageLevel == StatusUpdateEventArgs.Level.Progress)
            {
                IPodcastEpisodeDownloader downloader = sender as IPodcastEpisodeDownloader;
                int percentage = Convert.ToInt32(e.Message);
                if (downloader != null && percentage % 10 == 0)
                {
                    Console.WriteLine(string.Format("{0} {1}%",downloader.SyncItem.EpisodeTitle, percentage));
                }
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

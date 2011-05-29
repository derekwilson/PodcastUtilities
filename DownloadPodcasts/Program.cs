using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using PodcastUtilities.Common;
using PodcastUtilities.Common.IO;
using PodcastUtilities.Ioc;

namespace DownloadPodcasts
{
    class Program
    {
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

        static void GetFileUsingWebClientAsync(string url, string filename)
        {
            using (WebClient client = new System.Net.WebClient())
            {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(client_DownloadDataCompleted);
                client.DownloadFileAsync(new Uri(url), filename);
            }
        }

        static void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine("Downloaded {0}%",e.ProgressPercentage);
        }

        static void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            Console.WriteLine("Download Complete.");
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

            //var rssxml = new XmlDocument();
            //rssxml.Load(GetStreamUsingWebClient("http://www.thenakedscientists.com/naked_scientists_podcast.xml"));

            //GetFileUsingWebClient("http://www.thenakedscientists.com/naked_scientists_podcast.xml", "test.xml");
            //GetFileUsingWebClientAsync("http://www.thenakedscientists.com/naked_scientists_podcast.xml", "test.xml");

            var episodes = new List<FeedSyncItem>(20);
            var podcastEpisodeFinder = iocContainer.Resolve<IPodcastFeedEpisodeFinder>();
            podcastEpisodeFinder.StatusUpdate += StatusUpdate;
            foreach (var podcastInfo in control.Podcasts)
            {
                podcastEpisodeFinder.FindEpisodesToDownload(control.SourceRoot, podcastInfo, episodes);
            }

            foreach (var feedSyncItem in episodes)
            {
                Console.WriteLine("Download {0} -> {1}", feedSyncItem.EpisodeUrl, feedSyncItem.DestinationPath);
                GetFileUsingWebClient(feedSyncItem.EpisodeUrl.ToString(),feedSyncItem.DestinationPath);
            }

            //GetFileUsingWebClientAsync();
            
            //var webClientFactory = iocContainer.Resolve<IWebClientFactory>();
            //using (var webClient = webClientFactory.GetWebClient())
            //{
            //    var feedFactory = iocContainer.Resolve<IPodcastFeedFactory>();

            //    var downloader = new PodcastFeedDownloader(webClient,feedFactory);

            //    var feed1 = downloader.DownLoadFeed(PodcastFeedFormat.RSS, new Uri("http://feeds.feedburner.com/thisdeveloperslife"));
            //    feed1.StatusUpdate += StatusUpdate;
            //    feed1.GetFeedEpisodes();

            //    var feed2 = downloader.DownLoadFeed(PodcastFeedFormat.RSS, new Uri("http://downloads.bbc.co.uk/podcasts/radio4/fricomedy/rss.xml"));
            //    feed2.StatusUpdate += StatusUpdate;
            //    feed2.GetFeedEpisodes();
            //}

            Console.WriteLine("Done");
        }

        static void StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            if (e.MessageLevel == StatusUpdateEventArgs.Level.Verbose && !_verbose)
            {
                return;
            }
            Console.WriteLine(e.Message);
        }
    }
}

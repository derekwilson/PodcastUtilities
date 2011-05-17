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

            //var rssxml = new XmlDocument();
            //rssxml.Load(GetStreamUsingWebClient("http://www.thenakedscientists.com/naked_scientists_podcast.xml"));

            //GetFileUsingWebClient("http://www.thenakedscientists.com/naked_scientists_podcast.xml", "test.xml");
            //GetFileUsingWebClientAsync("http://www.thenakedscientists.com/naked_scientists_podcast.xml", "test.xml");


            using (var webClient = new SystemNetWebClient())
            {
                var factory = iocContainer.Resolve<IPodcastFeedFactory>();

                var downloader = new PodcastFeedDownloader(webClient,factory);

                var feed1 = downloader.DownLoadFeed(PodcastFeedFormat.RSS, new Uri("http://feeds.feedburner.com/thisdeveloperslife"));
                Console.WriteLine("Feed1 title: {0}", feed1.Title);
                foreach (var podcastFeedItem in feed1.GetFeedEpisodes())
                {
                    Console.WriteLine(" Episode: {0}", podcastFeedItem.Title);
                    Console.WriteLine("  Url: {0}", podcastFeedItem.Address);
                    Console.WriteLine("  FIlename: {0}", podcastFeedItem.GetFilename());
                    Console.WriteLine("  Published: {0} {1}", podcastFeedItem.Published.ToShortDateString(), podcastFeedItem.Published.ToShortTimeString());
                }

                var feed2 = downloader.DownLoadFeed(PodcastFeedFormat.RSS, new Uri("http://downloads.bbc.co.uk/podcasts/radio4/fricomedy/rss.xml"));
                Console.WriteLine("Feed1 title: {0}", feed2.Title);
                foreach (var podcastFeedItem in feed2.GetFeedEpisodes())
                {
                    Console.WriteLine(" Episode: {0}", podcastFeedItem.Title);
                    Console.WriteLine("  Url: {0}", podcastFeedItem.Address);
                    Console.WriteLine("  FIlename: {0}", podcastFeedItem.GetFilename());
                    Console.WriteLine("  Published: {0} {1}", podcastFeedItem.Published.ToShortDateString(), podcastFeedItem.Published.ToShortTimeString());
                }
            }

            Console.WriteLine("Done");
        }

        static void StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            // maybe we want to optionally filter verbose message
            Console.WriteLine(e.Message);
        }
    }
}

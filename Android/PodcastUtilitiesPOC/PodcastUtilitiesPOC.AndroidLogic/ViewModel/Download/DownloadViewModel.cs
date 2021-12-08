using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Lifecycle;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using PodcastUtilitiesPOC.AndroidLogic.Logging;
using PodcastUtilitiesPOC.AndroidLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastUtilitiesPOC.AndroidLogic.ViewModel.Download
{
    public class DownloadViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> Title;
            public EventHandler<int> StartProgress;
            public EventHandler<int> UpdateProgress;
            public EventHandler EndPorgress;
            public EventHandler<List<RecyclerSyncItem>> SetSynItems;
            public EventHandler<StatusUpdateEventArgs> DownloadStatusUpdate;
            public EventHandler<ProgressEventArgs> DownloadProgressUpdate;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IEpisodeFinder PodcastEpisodeFinder;
        private ISyncItemToEpisodeDownloaderTaskConverter Converter;

        private ReadOnlyControlFile ControlFile;
        private List<RecyclerSyncItem> AllSyncItems = new List<RecyclerSyncItem>(20);
        private ITaskPool TaskPool;

        private bool initialised = false;

        public DownloadViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IEpisodeFinder podcastEpisodeFinder,
            ISyncItemToEpisodeDownloaderTaskConverter converter, 
            ITaskPool taskPool) : base(app)
        {
            Logger = logger;
            ResourceProvider = resProvider;
            PodcastEpisodeFinder = podcastEpisodeFinder;
            Converter = converter;
            TaskPool = taskPool;
            Logger.Debug(() => $"DownloadViewModel:ctor");
        }

        public void Initialise(ReadOnlyControlFile controlFile)
        {
            Logger.Debug(() => $"DownloadViewModel:Initialise");
            ControlFile = controlFile;
            var title = ResourceProvider.GetString(Resource.String.download_activity_title);
            Observables.Title?.Invoke(this, title);
        }

        [Lifecycle.Event.OnResume]
        [Java.Interop.Export]
        public void OnResume()
        {
            Logger.Debug(() => $"DownloadViewModel:OnResume");
        }

        public void FindEpisodesToDownload()
        {
            Logger.Debug(() => $"DownloadViewModel:FindEpisodesToDownload");
            if (initialised)
            {
                Logger.Warning(() => $"DownloadViewModel:FindEpisodesToDownload - already initialised");
                return;
            }
            if (ControlFile == null)
            {
                Logger.Warning(() => $"DownloadViewModel:FindEpisodesToDownload - no control file");
                return;
            }

            int feedCount = 0;
            foreach (var item in ControlFile.GetPodcasts())
            {
                feedCount++;
            }

            Observables.StartProgress?.Invoke(this, feedCount);

            // find the episodes to download
            AllSyncItems.Clear();
            int count = 0;
            foreach (var podcastInfo in ControlFile.GetPodcasts())
            {
                var episodesInThisFeed = PodcastEpisodeFinder.FindEpisodesToDownload(
                    ControlFile.GetSourceRoot(),
                    ControlFile.GetRetryWaitInSeconds(),
                    podcastInfo,
                    ControlFile.GetDiagnosticRetainTemporaryFiles());
                foreach (var episode in episodesInThisFeed)
                {
                    Logger.Debug(() => $"DownloadViewModel:FindEpisodesToDownload {episode.Id}, {episode.EpisodeTitle}");
                    var item = new RecyclerSyncItem()
                    {
                        SyncItem = episode,
                        ProgressPercentage = 0,
                        Podcast = podcastInfo
                    };
                    AllSyncItems.Add(item);
                }
                count++;
                Observables.UpdateProgress?.Invoke(this, count);
            }
            Observables.EndPorgress?.Invoke(this, null);
            Observables.SetSynItems?.Invoke(this, AllSyncItems);
            var title = string.Format(ResourceProvider.GetString(Resource.String.download_activity_after_load_title), AllSyncItems.Count);
            Logger.Debug(() => $"DownloadViewModel:DownloadAllPodcasts done - {title}");
            Observables.Title?.Invoke(this, title);
            initialised = true;
        }

        public void DownloadAllPodcasts()
        {
            Logger.Debug(() => $"DownloadViewModel:DownloadAllPodcasts");

            List<ISyncItem> AllEpisodes = new List<ISyncItem>(AllSyncItems.Count);
            AllSyncItems.ForEach(item => AllEpisodes.Add(item.SyncItem));

            IEpisodeDownloader[] downloadTasks = Converter.ConvertItemsToTasks(AllEpisodes, Observables.DownloadStatusUpdate, Observables.DownloadProgressUpdate);
            foreach (var task in downloadTasks)
            {
                Logger.Debug(() => $"DownloadViewModel:Download to: {task.SyncItem.DestinationPath}");
            }

            // run them in a task pool
            TaskPool.RunAllTasks(ControlFile.GetMaximumNumberOfConcurrentDownloads(), downloadTasks);
        }
    }
}
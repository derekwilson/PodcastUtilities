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
using PodcastUtilitiesPOC.AndroidLogic.Converter;
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
            public EventHandler<List<RecyclerSyncItem>> SetSyncItems;
            public EventHandler<Tuple<ISyncItem, int>> UpdateItemProgress;
            public EventHandler<string> DisplayMessage;
            public EventHandler StartDownloading;
            public EventHandler<string> EndDownloading;
            public EventHandler Exit;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IFileSystemHelper FileSystemHelper;
        private IByteConverter ByteConverter;
        private IEpisodeFinder PodcastEpisodeFinder;
        private ISyncItemToEpisodeDownloaderTaskConverter Converter;
        private ITaskPool TaskPool;

        private ReadOnlyControlFile ControlFile;
        private List<RecyclerSyncItem> AllSyncItems = new List<RecyclerSyncItem>(20);

        // do not make this anything other than private
        private object SyncLock = new object();
        private bool StartedFindingPodcasts = false;
        private bool CompletedFindingPodcasts = false;
        private int FeedCount = 0;

        // do not make this anything other than private
        private object MessageSyncLock = new object();

        public DownloadViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IEpisodeFinder podcastEpisodeFinder,
            ISyncItemToEpisodeDownloaderTaskConverter converter,
            ITaskPool taskPool,
            IFileSystemHelper fileSystemHelper, 
            IByteConverter byteConverter) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"DownloadViewModel:ctor");

            ResourceProvider = resProvider;
            PodcastEpisodeFinder = podcastEpisodeFinder;
            Converter = converter;
            TaskPool = taskPool;
            FileSystemHelper = fileSystemHelper;
            ByteConverter = byteConverter;
        }

        public void Initialise(ReadOnlyControlFile controlFile)
        {
            Logger.Debug(() => $"DownloadViewModel:Initialise");
            ControlFile = controlFile;
            Observables.Title?.Invoke(this, ResourceProvider.GetString(Resource.String.download_activity_title));
        }

        [Lifecycle.Event.OnResume]
        [Java.Interop.Export]
        public void OnResume()
        {
            Logger.Debug(() => $"DownloadViewModel:OnResume - finding {StartedFindingPodcasts}");
        }

        public void FindEpisodesToDownload()
        {
            Logger.Debug(() => $"DownloadViewModel:FindEpisodesToDownload");
            if (ControlFile == null)
            {
                Logger.Warning(() => $"DownloadViewModel:FindEpisodesToDownload - no control file");
                return;
            }

            lock (SyncLock)
            {
                if (StartedFindingPodcasts)
                {
                    Logger.Warning(() => $"DownloadViewModel:FindEpisodesToDownload - ignoring, already initialised");
                    if (CompletedFindingPodcasts)
                    {
                        Observables.SetSyncItems?.Invoke(this, AllSyncItems);
                        SetTitle();
                    }
                    else
                    {
                        Observables.StartProgress?.Invoke(this, FeedCount);
                    }
                    return;
                }
                StartedFindingPodcasts = true;
            }

            foreach (var item in ControlFile.GetPodcasts())
            {
                FeedCount++;
            }

            Observables.StartProgress?.Invoke(this, FeedCount);

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
                        Podcast = podcastInfo,
                        Selected = true
                    };
                    AllSyncItems.Add(item);
                }
                count++;
                Observables.UpdateProgress?.Invoke(this, count);
            }
            CompletedFindingPodcasts = true;
            Observables.EndPorgress?.Invoke(this, null);
            Observables.SetSyncItems?.Invoke(this, AllSyncItems);
            SetTitle();
        }

        private void SetTitle()
        {
            var title = string.Format(ResourceProvider.GetString(Resource.String.download_activity_after_load_title), AllSyncItems.Count);
            Logger.Debug(() => $"DownloadViewModel:SetTitle - {title}");
            Observables.Title?.Invoke(this, title);
        }

        public void DownloadAllPodcasts()
        {
            Logger.Debug(() => $"DownloadViewModel:DownloadAllPodcasts");

            if (AllSyncItems.Count < 1)
            {
                Observables.DisplayMessage?.Invoke(this, "Nothing to download");
                return;
            }

            Observables.StartDownloading?.Invoke(this, null);

            List<ISyncItem> AllEpisodesToDownload = new List<ISyncItem>(AllSyncItems.Count);
            AllSyncItems.Where(recyclerItem => recyclerItem.Selected).ToList().ForEach(item => AllEpisodesToDownload.Add(item.SyncItem));

            IEpisodeDownloader[] downloadTasks = Converter.ConvertItemsToTasks(AllEpisodesToDownload, DownloadStatusUpdate, DownloadProgressUpdate);
            foreach (var task in downloadTasks)
            {
                Logger.Debug(() => $"DownloadViewModel:Download to: {task.SyncItem.DestinationPath}");
            }

            // run them in a task pool
            TaskPool.RunAllTasks(ControlFile.GetMaximumNumberOfConcurrentDownloads(), downloadTasks);
        }

        public void DownloadComplete()
        {
            Observables.EndDownloading?.Invoke(this, ResourceProvider.GetString(Resource.String.download_activity_complete));
        }

        void DownloadProgressUpdate(object sender, ProgressEventArgs e)
        {
            lock (MessageSyncLock)
            {
                ISyncItem syncItem = e.UserState as ISyncItem;
                if (e.ProgressPercentage % 10 == 0)
                {
                    // only do every 10%
                    var line = string.Format("{0} ({1} of {2}) {3}%", syncItem.EpisodeTitle,
                                                    DisplayFormatter.RenderFileSize(e.ItemsProcessed),
                                                    DisplayFormatter.RenderFileSize(e.TotalItemsToProcess),
                                                    e.ProgressPercentage);
                    Logger.Debug(() => line);
                    Observables.UpdateItemProgress?.Invoke(this, Tuple.Create(syncItem, e.ProgressPercentage));
                }
                if (IsDestinationDriveFull(ControlFile.GetSourceRoot(), ControlFile.GetFreeSpaceToLeaveOnDownload()))
                {
                    TaskPool?.CancelAllTasks();
                }
            }
        }

        private bool IsDestinationDriveFull(string root, long freeSpaceToLeaveInMb)
        {
            var freeMb = ByteConverter.BytesToMegabytes(FileSystemHelper.GetAvailableFileSystemSizeInBytes(root));
            if (freeMb < freeSpaceToLeaveInMb)
            {
                var message = string.Format("Destination drive is full leaving {0:#,0.##} MB free", freeMb);
                Observables.DisplayMessage?.Invoke(this, message);
                Logger.Debug(() => message);
                return true;
            }
            return false;
        }

        void DownloadStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            bool _verbose = false;
            if (e.MessageLevel == StatusUpdateLevel.Verbose && !_verbose)
            {
                return;
            }

            lock (MessageSyncLock)
            {
                // keep all the message together
                if (e.Exception != null)
                {
                    Logger.LogException(() => $"MainActivity:StatusUpdate -> ", e.Exception);
                }
                else
                {
                    Logger.Debug(() => $"MainActivity:StatusUpdate {e.Message}");
                }
            }
        }
    }
}
using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PodcastUtilities.AndroidLogic.ViewModel.Download
{
    public class DownloadViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> Title;
            public EventHandler<int> StartProgress;
            public EventHandler<int> UpdateProgress;
            public EventHandler EndProgress;
            public EventHandler<List<DownloadRecyclerItem>> SetSyncItems;
            public EventHandler<Tuple<ISyncItem, int>> UpdateItemProgress;
            public EventHandler<string> DisplayMessage;
            public EventHandler StartDownloading;
            public EventHandler<string> EndDownloading;
            public EventHandler Exit;

        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IApplicationControlFileProvider ApplicationControlFileProvider;
        private IFileSystemHelper FileSystemHelper;
        private IByteConverter ByteConverter;
        private IEpisodeFinder PodcastEpisodeFinder;
        private ISyncItemToEpisodeDownloaderTaskConverter Converter;
        private ITaskPool TaskPool;


        private List<DownloadRecyclerItem> AllItems = new List<DownloadRecyclerItem>(20);
        private bool StartedFindingPodcasts = false;
        private bool CompletedFindingPodcasts = false;
        private int FeedCount = 0;

        // do not make this anything other than private
        private object SyncLock = new object();
        // do not make this anything other than private
        private object MessageSyncLock = new object();

        public DownloadViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            IFileSystemHelper fileSystemHelper,
            IByteConverter byteConverter,
            IEpisodeFinder podcastEpisodeFinder,
            ISyncItemToEpisodeDownloaderTaskConverter converter,
            ITaskPool taskPool
            ) : base(app)
        {
            ApplicationContext = app;
            Logger = logger;
            ResourceProvider = resProvider;
            ApplicationControlFileProvider = appControlFileProvider;
            FileSystemHelper = fileSystemHelper;
            ByteConverter = byteConverter;
            PodcastEpisodeFinder = podcastEpisodeFinder;
            Converter = converter;
            TaskPool = taskPool;
        }

        public void Initialise()
        {
            Logger.Debug(() => $"DownloadViewModel:Initialise");
            Observables.Title?.Invoke(this, ResourceProvider.GetString(Resource.String.download_activity_title));
        }

        public void FindEpisodesToDownload()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            Logger.Debug(() => $"DownloadViewModel:FindEpisodesToDownload");
            if (controlFile == null)
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
                        Observables.SetSyncItems?.Invoke(this, AllItems);
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

            foreach (var item in controlFile.GetPodcasts())
            {
                FeedCount++;
            }

            Observables.StartProgress?.Invoke(this, FeedCount);

            // find the episodes to download
            AllItems.Clear();
            int count = 0;
            foreach (var podcastInfo in controlFile.GetPodcasts())
            {
                var episodesInThisFeed = PodcastEpisodeFinder.FindEpisodesToDownload(
                    controlFile.GetSourceRoot(),
                    controlFile.GetRetryWaitInSeconds(),
                    podcastInfo,
                    controlFile.GetDiagnosticRetainTemporaryFiles());
                foreach (var episode in episodesInThisFeed)
                {
                    Logger.Debug(() => $"DownloadViewModel:FindEpisodesToDownload {episode.Id}, {episode.EpisodeTitle}");
                    var item = new DownloadRecyclerItem()
                    {
                        SyncItem = episode,
                        ProgressPercentage = 0,
                        Podcast = podcastInfo,
                        Selected = true
                    };
                    AllItems.Add(item);
                }
                count++;
                Observables.UpdateProgress?.Invoke(this, count);
            }
            CompletedFindingPodcasts = true;
            Observables.EndProgress?.Invoke(this, null);
            Observables.SetSyncItems?.Invoke(this, AllItems);
            SetTitle();
        }

        private int GetItemsSelectedCount()
        {
            if (AllItems.Count < 1)
            {
                return 0;
            }
            return AllItems.Where(recyclerItem => recyclerItem.Selected).Count();
        }

        private void SetTitle()
        {
            var title = ResourceProvider.GetQuantityString(Resource.Plurals.download_activity_title_after_load, GetItemsSelectedCount());
            Logger.Debug(() => $"DownloadViewModel:SetTitle - {title}");
            Observables.Title?.Invoke(this, title);
        }

        internal void SelectionChanged(int position)
        {
            SetTitle();
        }

        public void DownloadAllPodcasts()
        {
            Logger.Debug(() => $"DownloadViewModel:DownloadAllPodcasts");

            if (AllItems.Count < 1)
            {
                Observables.DisplayMessage?.Invoke(this, "Nothing to download");
                return;
            }

            Observables.StartDownloading?.Invoke(this, null);

            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            int numberOfConnections = controlFile.GetMaximumNumberOfConcurrentDownloads();
            System.Net.ServicePointManager.DefaultConnectionLimit = numberOfConnections;

            List<ISyncItem> AllEpisodesToDownload = new List<ISyncItem>(AllItems.Count);
            AllItems.Where(recyclerItem => recyclerItem.Selected).ToList().ForEach(item => AllEpisodesToDownload.Add(item.SyncItem));

            IEpisodeDownloader[] downloadTasks = Converter.ConvertItemsToTasks(AllEpisodesToDownload, DownloadStatusUpdate, DownloadProgressUpdate);
            foreach (var task in downloadTasks)
            {
                Logger.Debug(() => $"DownloadViewModel:Download to: {task.SyncItem.DestinationPath}");
            }

            // run them in a task pool
            TaskPool.RunAllTasks(numberOfConnections, downloadTasks);
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
                var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
                if (IsDestinationDriveFull(controlFile.GetSourceRoot(), controlFile.GetFreeSpaceToLeaveOnDownload()))
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
                    Logger.LogException(() => $"DownloadViewModel:StatusUpdate -> ", e.Exception);
                }
                else
                {
                    Logger.Debug(() => $"DownloadViewModel:StatusUpdate {e.Message}");
                }
            }
        }

    }
}
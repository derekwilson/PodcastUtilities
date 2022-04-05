using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PodcastUtilities.AndroidLogic.ViewModel.Purge
{
    public class PurgeViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> Title;
            public EventHandler<int> StartProgress;
            public EventHandler<int> UpdateProgress;
            public EventHandler EndProgress;
            public EventHandler<List<PurgeRecyclerItem>> SetPurgeItems;
            public EventHandler StartDeleting;
            public EventHandler<string> EndDeleting;
            public EventHandler<string> DisplayMessage;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IApplicationControlFileProvider ApplicationControlFileProvider;
        private IEpisodePurger EpisodePurger;
        private IFileUtilities FileUtilities;
        private ICrashReporter CrashReporter;
        private IAnalyticsEngine AnalyticsEngine;

        private List<PurgeRecyclerItem> AllItems = new List<PurgeRecyclerItem>(20);
        private bool StartedFindingItems = false;
        private bool CompletedFindingItems = false;
        private int FeedCount = 0;

        // do not make this anything other than private
        private object SyncLock = new object();

        public PurgeViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resourceProvider,
            IApplicationControlFileProvider applicationControlFileProvider,
            IEpisodePurger episodePurger,
            IFileUtilities fileUtilities, 
            ICrashReporter crashReporter, 
            IAnalyticsEngine analyticsEngine) : base(app)
        {
            Logger = logger;
            ResourceProvider = resourceProvider;
            ApplicationControlFileProvider = applicationControlFileProvider;
            EpisodePurger = episodePurger;
            FileUtilities = fileUtilities;
            CrashReporter = crashReporter;
            AnalyticsEngine = analyticsEngine;
        }

        public void Initialise()
        {
            Logger.Debug(() => $"PurgeViewModel:Initialise");
            Observables.Title?.Invoke(this, ResourceProvider.GetString(Resource.String.purge_activity_title));
        }

        internal void SelectionChanged(int position)
        {
            SetTitle();
        }

        public void FindItemsToDelete()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            Logger.Debug(() => $"PurgeViewModel:FindItemsToDelete");
            if (controlFile == null)
            {
                Logger.Warning(() => $"PurgeViewModel:FindItemsToDelete - no control file");
                return;
            }

            lock (SyncLock)
            {
                if (StartedFindingItems)
                {
                    Logger.Warning(() => $"PurgeViewModel:FindItemsToDelete - ignoring, already initialised");
                    if (CompletedFindingItems)
                    {
                        Observables.SetPurgeItems?.Invoke(this, AllItems);
                        SetTitle();
                    }
                    else
                    {
                        // we scan each feed twice
                        Observables.StartProgress?.Invoke(this, FeedCount *2);
                    }
                    return;
                }
                StartedFindingItems = true;
            }

            foreach (var item in controlFile.GetPodcasts())
            {
                FeedCount++;
            }

            Observables.StartProgress?.Invoke(this, FeedCount * 2);

            // find the items to delete
            AllItems.Clear();
            int count = 0;

            // find the episodes to delete
            List<IFileInfo> allFilesToDelete = new List<IFileInfo>(20);
            foreach (var podcastInfo in controlFile.GetPodcasts())
            {
                IList<IFileInfo> filesToDeleteFromThisFeed = EpisodePurger.FindEpisodesToPurge(controlFile.GetSourceRoot(), podcastInfo);
                List<IFileInfo> sortedFileList = filesToDeleteFromThisFeed.OrderBy(item => item.Name).ToList();
                foreach (var file in sortedFileList)
                {
                    var line = $"File: {file.FullName}";
                    Logger.Debug(() => $"PurgeViewModel:FindItemsToDelete {line}");
                    var item = new PurgeRecyclerItem()
                    {
                        FileOrDirectoryItem = file,
                        Selected = true
                    };
                    // this is for the recycler adapter
                    AllItems.Add(item);
                }
                // and this is so we can work out which folder will be empty
                allFilesToDelete.AddRange(filesToDeleteFromThisFeed);
                count++;
                Observables.UpdateProgress?.Invoke(this, count);
            }

            // find folders that can now be deleted
            foreach (var podcastInfo in controlFile.GetPodcasts())
            {
                IList<IDirectoryInfo> foldersToDeleteInThisFeed = EpisodePurger.FindEmptyFoldersToDelete(controlFile.GetSourceRoot(), podcastInfo, allFilesToDelete);
                List<IDirectoryInfo> sortedDirList = foldersToDeleteInThisFeed.OrderBy(item => item.FullName).ToList();
                foreach (var dir in sortedDirList)
                {
                    var line = $"Dir: {dir.FullName}";
                    Logger.Debug(() => $"PurgeViewModel:FindItemsToDelete {line}");
                    var item = new PurgeRecyclerItem()
                    {
                        FileOrDirectoryItem = dir,
                        Selected = true
                    };
                    AllItems.Add(item);
                }
                count++;
                Observables.UpdateProgress?.Invoke(this, count);
            }

            CompletedFindingItems = true;
            Observables.EndProgress?.Invoke(this, null);
            Observables.SetPurgeItems?.Invoke(this, AllItems);
            SetTitle();
            AnalyticsEngine.PurgeScanEvent(GetItemsSelectedCount());
        }

        public void PurgeAllItems()
        {
            AnalyticsEngine.PurgeDeleteEvent(GetItemsSelectedCount());
            foreach (PurgeRecyclerItem item in AllItems)
            {
                if (item.Selected)
                {
                    try
                    {
                        DeleteItem(item.FileOrDirectoryItem);
                    } catch (Exception ex)
                    {
                        var name = GetDisplayName(item.FileOrDirectoryItem);
                        Logger.LogException(() => $"Delete item: {name}", ex);
                        CrashReporter.LogNonFatalException(ex);
                        string fmt = ResourceProvider.GetString(Resource.String.error_delete_item);
                        Observables.DisplayMessage?.Invoke(this, string.Format(fmt, name));
                    }
                }
            }
        }

        private string GetDisplayName(IFileInfo fileInfo) => fileInfo.Name;
        private string GetDisplayName(IDirectoryInfo dirInfo) => dirInfo.FullName;
        private string GetDisplayName<T>(T field) => "Unknown type: " + field.GetType();

        private void DeleteItem(IFileInfo fileInfo) => FileUtilities.FileDelete(fileInfo.FullName);
        private void DeleteItem(IDirectoryInfo dirInfo) => EpisodePurger.PurgeFolder(dirInfo);
        private void DeleteItem<T>(T field) => Logger.Debug(() => $"Ignoring unknown type: {field.GetType()}");

        public void PurgeComplete()
        {
            Observables.EndDeleting?.Invoke(this, ResourceProvider.GetString(Resource.String.purge_activity_complete));
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
            var title = ResourceProvider.GetQuantityString(Resource.Plurals.purge_activity_title_after_load, GetItemsSelectedCount());
            Logger.Debug(() => $"PurgeViewModel:SetTitle - {title}");
            Observables.Title?.Invoke(this, title);
        }

    }
}
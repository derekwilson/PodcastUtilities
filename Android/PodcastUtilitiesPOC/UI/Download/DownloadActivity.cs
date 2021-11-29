using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Feeds;
using PodcastUtilitiesPOC.CustomViews;
using PodcastUtilitiesPOC.UI.Main;
using PodcastUtilitiesPOC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastUtilitiesPOC.UI.Download
{
    [Activity(Label = "Download Podcasts", ParentActivity = typeof(MainActivity))]
    public class DownloadActivity : AppCompatActivity
    {
        private AndroidApplication AndroidApplication;
        private EmptyRecyclerView RvDownloads;
        private ProgressSpinnerView ProgressSpinner;
        private LinearLayout NoDataView;
        private SyncItemRecyclerAdapter Adapter;
        private List<RecyclerSyncItem> AllSyncItems = new List<RecyclerSyncItem>(20);
        private ITaskPool TaskPool;
        static object SyncLock = new object();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"DownloadActivity:OnCreate");

            base.OnCreate(savedInstanceState);

            // Set our view from the layout resource
            SetContentView(Resource.Layout.activity_download);

            RvDownloads = FindViewById<EmptyRecyclerView>(Resource.Id.rvDownloads);
            ProgressSpinner = FindViewById<ProgressSpinnerView>(Resource.Id.progressBar);
            NoDataView = FindViewById<LinearLayout>(Resource.Id.layNoData);

            RvDownloads.SetLayoutManager(new LinearLayoutManager(this));
            RvDownloads.AddItemDecoration(new DividerItemDecoration(this, DividerItemDecoration.Vertical));
            RvDownloads.SetEmptyView(NoDataView);
            Adapter = new SyncItemRecyclerAdapter(this);
            RvDownloads.SetAdapter(Adapter);

            Task.Run(() => FindEpisodesToDownload());
            AndroidApplication.Logger.Debug(() => $"DownloadActivity:OnCreate - end");
        }

        private void FindEpisodesToDownload()
        {
            AndroidApplication.Logger.Debug(() => $"DownloadActivity:FindEpisodesToDownload");
            if (AndroidApplication.ControlFile == null)
            {
                AndroidApplication.Logger.Warning(() => $"DownloadActivity:FindEpisodesToDownload - no control file");
                return;
            }

            int feedCount = 0;
            foreach (var item in AndroidApplication.ControlFile.GetPodcasts())
            {
                feedCount++;
            }

            StartProgress(feedCount);
            IEpisodeFinder podcastEpisodeFinder = null;
            podcastEpisodeFinder = AndroidApplication.IocContainer.Resolve<IEpisodeFinder>();

            // find the episodes to download
            AllSyncItems.Clear();
            int count = 0;
            foreach (var podcastInfo in AndroidApplication.ControlFile.GetPodcasts())
            {
                var episodesInThisFeed = podcastEpisodeFinder.FindEpisodesToDownload(
                    AndroidApplication.ControlFile.GetSourceRoot(),
                    AndroidApplication.ControlFile.GetRetryWaitInSeconds(),
                    podcastInfo,
                    AndroidApplication.ControlFile.GetDiagnosticRetainTemporaryFiles());
                foreach (var episode in episodesInThisFeed)
                {
                    AndroidApplication.Logger.Debug(() => $"DownloadActivity:FindEpisodesToDownload {episode.Id}, {episode.EpisodeTitle}");
                    var item = new RecyclerSyncItem()
                    {
                        SyncItem = episode,
                        ProgressPercentage = 0,
                        Podcast = podcastInfo
                    };
                    AllSyncItems.Add(item);
                }
                count++;
                UpdateProgress(count);
            }
            EndProgress();
            RunOnUiThread(() =>
            {
                Adapter.SetItems(AllSyncItems);
                Adapter.NotifyDataSetChanged();
            });
        }

        private void DownloadAllPodcasts()
        {
            AndroidApplication.Logger.Debug(() => $"DownloadActivity:DownloadAllPodcasts");

            List<ISyncItem> AllEpisodes = new List<ISyncItem>(Adapter.ItemCount);
            AllSyncItems.ForEach(item => AllEpisodes.Add(item.SyncItem));

            var converter = AndroidApplication.IocContainer.Resolve<ISyncItemToEpisodeDownloaderTaskConverter>();
            IEpisodeDownloader[] downloadTasks = converter.ConvertItemsToTasks(AllEpisodes, StatusUpdate, ProgressUpdate);

            foreach (var task in downloadTasks)
            {
                AndroidApplication.Logger.Debug(() => $"DownloadActivity:Download to: {task.SyncItem.DestinationPath}");
            }

            // run them in a task pool
            TaskPool = AndroidApplication.IocContainer.Resolve<ITaskPool>();
            TaskPool.RunAllTasks(AndroidApplication.ControlFile.GetMaximumNumberOfConcurrentDownloads(), downloadTasks);

            ToastMessage("Done");
        }

        void ProgressUpdate(object sender, ProgressEventArgs e)
        {
            lock (SyncLock)
            {
                ISyncItem syncItem = e.UserState as ISyncItem;
                if (e.ProgressPercentage % 10 == 0)
                {
                    // only do every 10%
                    var line = string.Format("{0} ({1} of {2}) {3}%", syncItem.EpisodeTitle,
                                                    DisplayFormatter.RenderFileSize(e.ItemsProcessed),
                                                    DisplayFormatter.RenderFileSize(e.TotalItemsToProcess),
                                                    e.ProgressPercentage);
                    AndroidApplication.Logger.Debug(() => line);
                    RunOnUiThread(() =>
                    {
                        var position = Adapter.SetItemProgress(syncItem.Id, e.ProgressPercentage);
                        Adapter.NotifyItemChanged(position);
                    });
                }
            }
        }

        void StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            bool _verbose = false;
            if (e.MessageLevel == StatusUpdateLevel.Verbose && !_verbose)
            {
                return;
            }

            lock (SyncLock)
            {
                // keep all the message together
                if (e.Exception != null)
                {
                    AndroidApplication.Logger.LogException(() => $"MainActivity:StatusUpdate -> ", e.Exception);
                }
                else
                {
                    AndroidApplication.Logger.Debug(() => $"MainActivity:StatusUpdate {e.Message}");
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //change main_compat_menu
            MenuInflater.Inflate(Resource.Menu.menu_download, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            AndroidApplication.Logger.Debug(() => $"DownloadActivity:OnOptionsItemSelected {item.ItemId}");
            switch (item.ItemId)
            {
                case Resource.Id.home:
                    OnBackPressed();
                    return true;
                case Resource.Id.action_download_all_podcasts:
                    if (Adapter.ItemCount < 1)
                    {
                        ToastMessage("Nothing to download");
                        return true;
                    }
                    Task.Run(() => DownloadAllPodcasts());
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void ToastMessage(string message)
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
            });
        }

        private void StartProgress(int max)
        {
            RunOnUiThread(() =>
            {
                ProgressViewHelper.StartProgress(ProgressSpinner, Window, max);
            });
        }

        private void UpdateProgress(int position)
        {
            RunOnUiThread(() =>
            {
                ProgressSpinner.Progress = position;
            });
        }

        private void EndProgress()
        {
            RunOnUiThread(() =>
            {
                ProgressViewHelper.CompleteProgress(ProgressSpinner, Window);
            });
        }
    }
}
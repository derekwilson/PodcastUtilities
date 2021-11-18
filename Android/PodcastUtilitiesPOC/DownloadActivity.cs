using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.Common.Feeds;
using PodcastUtilitiesPOC.CustomViews;
using PodcastUtilitiesPOC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastUtilitiesPOC
{
    [Activity(Label = "Download Podcasts", ParentActivity = typeof(MainActivity))]
    public class DownloadActivity : AppCompatActivity
    {
        private AndroidApplication AndroidApplication;
        private EmptyRecyclerView RvDownloads;
        private ProgressSpinnerView ProgressSpinner;
        private LinearLayout NoDataView;
        private SyncItemRecyclerAdapter Adapter;
        List<ISyncItem> AllSyncItems = new List<ISyncItem>(20);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = (Application as AndroidApplication);
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
                AllSyncItems.AddRange(episodesInThisFeed);
                foreach (var episode in episodesInThisFeed)
                {
                    AndroidApplication.Logger.Debug(() => $"DownloadActivity:FindEpisodesToDownload {episode.Id}, {episode.EpisodeTitle}");
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
            }
            return base.OnOptionsItemSelected(item);
        }


        private void StartProgress(int max)
        {
            RunOnUiThread(() =>
            {
                ProgressViewHelper.StartProgress(ProgressSpinner, this.Window, max);
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
                ProgressViewHelper.CompleteProgress(ProgressSpinner, this.Window);
            });
        }
    }
}
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Feeds;
using PodcastUtilitiesPOC.AndroidLogic.ViewModel;
using PodcastUtilitiesPOC.AndroidLogic.ViewModel.Download;
using PodcastUtilitiesPOC.CustomViews;
using PodcastUtilitiesPOC.UI.Main;
using PodcastUtilitiesPOC.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastUtilitiesPOC.UI.Download
{
    [Activity(Label = "Download Podcasts", ParentActivity = typeof(MainActivity))]
    public class DownloadActivity : AppCompatActivity
    {
        private DownloadViewModel ViewModel;

        private AndroidApplication AndroidApplication;
        private EmptyRecyclerView RvDownloads;
        private ProgressSpinnerView ProgressSpinner;
        private LinearLayout NoDataView;
        private SyncItemRecyclerAdapter Adapter;
        // do not make this anything other than private
        private object SyncLock = new object();

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

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(DownloadViewModel))) as DownloadViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise(AndroidApplication.ControlFile);
            Task.Run(() => ViewModel.FindEpisodesToDownload());

            AndroidApplication.Logger.Debug(() => $"DownloadActivity:OnCreate - end");
        }

        protected override void OnStop()
        {
            base.OnStop();
            KillViewModelObservers();
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.StartProgress += StartProgress;
            ViewModel.Observables.UpdateProgress += UpdateProgress;
            ViewModel.Observables.EndPorgress += EndProgress;
            ViewModel.Observables.SetSynItems += SetSynItems;
            ViewModel.Observables.DownloadProgressUpdate += DownloadProgressUpdate;
            ViewModel.Observables.DownloadStatusUpdate += DownloadStatusUpdate;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.Title -= SetTitle;
            ViewModel.Observables.StartProgress -= StartProgress;
            ViewModel.Observables.UpdateProgress -= UpdateProgress;
            ViewModel.Observables.EndPorgress -= EndProgress;
            ViewModel.Observables.SetSynItems -= SetSynItems;
            ViewModel.Observables.DownloadProgressUpdate -= DownloadProgressUpdate;
            ViewModel.Observables.DownloadStatusUpdate -= DownloadStatusUpdate;
        }

        private void SetTitle(object sender, string title)
        {
            RunOnUiThread(() =>
            {
                Title = title;
            });
        }

        private void SetSynItems(object sender, List<RecyclerSyncItem> items)
        {
            RunOnUiThread(() =>
            {
                Adapter.SetItems(items);
                Adapter.NotifyDataSetChanged();
            });
        }

        private void EndProgress(object sender, System.EventArgs e)
        {
            RunOnUiThread(() =>
            {
                ProgressViewHelper.CompleteProgress(ProgressSpinner, Window);
            });
        }

        private void UpdateProgress(object sender, int position)
        {
            RunOnUiThread(() =>
            {
                ProgressSpinner.Progress = position;
            });
        }

        private void StartProgress(object sender, int max)
        {
            RunOnUiThread(() =>
            {
                ProgressViewHelper.StartProgress(ProgressSpinner, Window, max);
            });
        }

        void DownloadProgressUpdate(object sender, ProgressEventArgs e)
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

        void DownloadStatusUpdate(object sender, StatusUpdateEventArgs e)
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
                    Task.Run(() => ViewModel.DownloadAllPodcasts());
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
    }
}
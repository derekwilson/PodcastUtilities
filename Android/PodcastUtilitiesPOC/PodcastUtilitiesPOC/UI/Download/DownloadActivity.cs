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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastUtilitiesPOC.UI.Download
{
    // Title is set dynamically
    [Activity(ParentActivity = typeof(MainActivity))]
    public class DownloadActivity : AppCompatActivity
    {
        private DownloadViewModel ViewModel;

        private AndroidApplication AndroidApplication;
        private EmptyRecyclerView RvDownloads;
        private ProgressSpinnerView ProgressSpinner;
        private LinearLayout NoDataView;
        private SyncItemRecyclerAdapter Adapter;

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
            ViewModel.Observables.SetSyncItems += SetSyncItems;
            ViewModel.Observables.UpdateItemProgress += UpdateItemProgress;
            ViewModel.Observables.DisplayMessage += ToastMessage;
            ViewModel.Observables.StartDownloading += StartDownloading;
            ViewModel.Observables.EndDownloading += EndDownloading;
            ViewModel.Observables.Exit += Exit;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.Title -= SetTitle;
            ViewModel.Observables.StartProgress -= StartProgress;
            ViewModel.Observables.UpdateProgress -= UpdateProgress;
            ViewModel.Observables.EndPorgress -= EndProgress;
            ViewModel.Observables.SetSyncItems -= SetSyncItems;
            ViewModel.Observables.UpdateItemProgress -= UpdateItemProgress;
            ViewModel.Observables.DisplayMessage -= ToastMessage;
            ViewModel.Observables.StartDownloading -= StartDownloading;
            ViewModel.Observables.EndDownloading -= EndDownloading;
            ViewModel.Observables.Exit -= Exit;
        }

        private void ToastMessage(object sender, string message)
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
            });
        }

        private void SetTitle(object sender, string title)
        {
            RunOnUiThread(() =>
            {
                Title = title;
            });
        }

        private void SetSyncItems(object sender, List<RecyclerSyncItem> items)
        {
            RunOnUiThread(() =>
            {
                Adapter.SetItems(items);
                Adapter.NotifyDataSetChanged();
            });
        }

        private void StartProgress(object sender, int max)
        {
            RunOnUiThread(() =>
            {
                ProgressViewHelper.StartProgress(ProgressSpinner, Window, max);
            });
        }

        private void UpdateProgress(object sender, int position)
        {
            RunOnUiThread(() =>
            {
                ProgressSpinner.Progress = position;
            });
        }

        private void EndProgress(object sender, System.EventArgs e)
        {
            RunOnUiThread(() =>
            {
                ProgressViewHelper.CompleteProgress(ProgressSpinner, Window);
            });
        }

        private void UpdateItemProgress(object sender, Tuple<ISyncItem, int> updateItem)
        {
            RunOnUiThread(() =>
            {
                (ISyncItem item, int progress) = updateItem;
                var position = Adapter.SetItemProgress(item.Id, progress);
                Adapter.NotifyItemChanged(position);
            });
        }

        private void StartDownloading(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                Adapter.SetReadOnly(true);
            });
        }

        private void EndDownloading(object sender, string message)
        {
            RunOnUiThread(() =>
            {
                Adapter.SetReadOnly(false);
                Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
            });
        }

        private void Exit(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                Finish();
            });
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
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
                    Task.Run(() => ViewModel.DownloadAllPodcasts())
                        .ContinueWith(t => ViewModel.DownloadComplete());
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}
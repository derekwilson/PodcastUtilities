using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.AndroidLogic.Adapters;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Download;
using PodcastUtilities.Common.Feeds;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PodcastUtilities.UI.Download
{
    // Title is set dynamically
    [Activity(ParentActivity = typeof(MainActivity))]
    public class DownloadActivity : AppCompatActivity
    {
        private DownloadViewModel ViewModel;

        private AndroidApplication AndroidApplication;
        private EmptyRecyclerView RvDownloads;
        private LinearLayout NoDataView;
        private ProgressSpinnerView ProgressSpinner;
        private DownloadRecyclerItemAdapter Adapter;
        private Button DownloadButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"DownloadActivity:OnCreate");

            base.OnCreate(savedInstanceState);

            // Set our view from the layout resource
            SetContentView(Resource.Layout.activity_download);

            RvDownloads = FindViewById<EmptyRecyclerView>(Resource.Id.rvDownloads);
            NoDataView = FindViewById<LinearLayout>(Resource.Id.layNoData);
            ProgressSpinner = FindViewById<ProgressSpinnerView>(Resource.Id.progressBar);
            DownloadButton = FindViewById<Button>(Resource.Id.btnDownload);

            RvDownloads.SetLayoutManager(new LinearLayoutManager(this));
            RvDownloads.AddItemDecoration(new DividerItemDecoration(this, DividerItemDecoration.Vertical));
            RvDownloads.SetEmptyView(NoDataView);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(DownloadViewModel))) as DownloadViewModel;

            Adapter = new DownloadRecyclerItemAdapter(this, ViewModel);
            RvDownloads.SetAdapter(Adapter);

            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();
            Task.Run(() => ViewModel.FindEpisodesToDownload());

            DownloadButton.Click += (sender, e) => 
            Task.Run(() => ViewModel.DownloadAllPodcasts())
                    .ContinueWith(t => ViewModel.DownloadComplete());

            AndroidApplication.Logger.Debug(() => $"DownloadActivity:OnCreate - end");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            KillViewModelObservers();
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.StartProgress += StartProgress;
            ViewModel.Observables.UpdateProgress += UpdateProgress;
            ViewModel.Observables.EndProgress += EndProgress;
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
            ViewModel.Observables.EndProgress -= EndProgress;
            ViewModel.Observables.SetSyncItems -= SetSyncItems;
            ViewModel.Observables.UpdateItemProgress -= UpdateItemProgress;
            ViewModel.Observables.DisplayMessage -= ToastMessage;
            ViewModel.Observables.StartDownloading -= StartDownloading;
            ViewModel.Observables.EndDownloading -= EndDownloading;
            ViewModel.Observables.Exit -= Exit;
        }

        private void SetSyncItems(object sender, List<DownloadRecyclerItem> items)
        {
            RunOnUiThread(() =>
            {
                Adapter.SetItems(items);
                Adapter.NotifyDataSetChanged();
            });
        }

        private void EndProgress(object sender, EventArgs e)
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

        private void SetTitle(object sender, string title)
        {
            RunOnUiThread(() =>
            {
                Title = title;
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

        private void ToastMessage(object sender, string e)
        {
            throw new NotImplementedException();
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
    }
}
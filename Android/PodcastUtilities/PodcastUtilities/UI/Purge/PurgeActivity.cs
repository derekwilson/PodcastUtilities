using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.AndroidLogic.Adapters;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Purge;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PodcastUtilities.UI.Purge
{
    // Title is set dynamically
    [Activity(ParentActivity = typeof(MainActivity))]
    public class PurgeActivity : AppCompatActivity
    {
        private PurgeViewModel ViewModel;

        private AndroidApplication AndroidApplication;

        private EmptyRecyclerView RvPurgeItems;
        private PurgeRecyclerItemAdapter Adapter;
        private LinearLayout NoDataView;
        private ProgressSpinnerView ProgressSpinner;
        private Button DeleteButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"PurgeActivity:OnCreate");

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            base.OnCreate(savedInstanceState);

            // Set our view from the layout resource
            SetContentView(Resource.Layout.activity_purge);

            RvPurgeItems = FindViewById<EmptyRecyclerView>(Resource.Id.rvPurge);
            NoDataView = FindViewById<LinearLayout>(Resource.Id.layNoDataPurge);
            ProgressSpinner = FindViewById<ProgressSpinnerView>(Resource.Id.progressBarPurge);
            DeleteButton = FindViewById<Button>(Resource.Id.btnPurge);

            RvPurgeItems.SetLayoutManager(new LinearLayoutManager(this));
            RvPurgeItems.AddItemDecoration(new DividerItemDecoration(this, DividerItemDecoration.Vertical));
            RvPurgeItems.SetEmptyView(NoDataView);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(PurgeViewModel))) as PurgeViewModel;

            Adapter = new PurgeRecyclerItemAdapter(this, ViewModel);
            RvPurgeItems.SetAdapter(Adapter);

            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();
            Task.Run(() => ViewModel.FindItemsToDelete());

            AndroidApplication.Logger.Debug(() => $"PurgeActivity:OnCreate - end");
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"PurgeActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"PurgeActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.StartProgress += StartProgress;
            ViewModel.Observables.UpdateProgress += UpdateProgress;
            ViewModel.Observables.EndProgress += EndProgress;
            ViewModel.Observables.SetPurgeItems += SetPurgeItems;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.Title -= SetTitle;
            ViewModel.Observables.StartProgress -= StartProgress;
            ViewModel.Observables.UpdateProgress -= UpdateProgress;
            ViewModel.Observables.EndProgress -= EndProgress;
            ViewModel.Observables.SetPurgeItems -= SetPurgeItems;
        }

        private void SetTitle(object sender, string title)
        {
            RunOnUiThread(() =>
            {
                Title = title;
            });
        }

        private void SetPurgeItems(object sender, List<PurgeRecyclerItem> items)
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
                DeleteButton.Enabled = true;
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
                DeleteButton.Enabled = false;
            });
        }

    }
}
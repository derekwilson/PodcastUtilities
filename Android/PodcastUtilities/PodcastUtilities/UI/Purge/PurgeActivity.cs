using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Purge;

namespace PodcastUtilities.UI.Purge
{
    // Title is set dynamically
    [Activity(ParentActivity = typeof(MainActivity))]
    public class PurgeActivity : AppCompatActivity
    {
        private PurgeViewModel ViewModel;

        private AndroidApplication AndroidApplication;

        private EmptyRecyclerView RvDownloads;
        private LinearLayout NoDataView;
        private ProgressSpinnerView ProgressSpinner;
        private Button DownloadButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"PurgeActivity:OnCreate");

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            base.OnCreate(savedInstanceState);

            // Set our view from the layout resource
            SetContentView(Resource.Layout.activity_purge);

            RvDownloads = FindViewById<EmptyRecyclerView>(Resource.Id.rvPurge);
            NoDataView = FindViewById<LinearLayout>(Resource.Id.layNoDataPurge);
            ProgressSpinner = FindViewById<ProgressSpinnerView>(Resource.Id.progressBarPurge);
            DownloadButton = FindViewById<Button>(Resource.Id.btnPurge);

            RvDownloads.SetLayoutManager(new LinearLayoutManager(this));
            RvDownloads.AddItemDecoration(new DividerItemDecoration(this, DividerItemDecoration.Vertical));
            RvDownloads.SetEmptyView(NoDataView);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(PurgeViewModel))) as PurgeViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();

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
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.Title -= SetTitle;
        }

        private void SetTitle(object sender, string title)
        {
            RunOnUiThread(() =>
            {
                Title = title;
            });
        }
    }
}
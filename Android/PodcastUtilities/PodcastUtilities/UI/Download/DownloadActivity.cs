using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Download;
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

            RvDownloads.SetLayoutManager(new LinearLayoutManager(this));
            RvDownloads.AddItemDecoration(new DividerItemDecoration(this, DividerItemDecoration.Vertical));
            RvDownloads.SetEmptyView(NoDataView);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(DownloadViewModel))) as DownloadViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();
            //Task.Run(() => ViewModel.FindEpisodesToDownload());

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
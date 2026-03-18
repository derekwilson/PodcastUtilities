using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Share;

namespace PodcastUtilities.UI.Share
{
    [Activity(Label = "@string/share_episode_activity_title", ParentActivity = typeof(MainActivity))]
    internal class ShareEpisodeActivity : AppCompatActivity
    {
        private const string ACTIVITY_PARAM_FOLDER = "ShareEpisodeActivity:Param:Folder";

        public static Intent CreateIntent(Context context, string? folder)
        {
            Intent intent = new Intent(context, typeof(ShareEpisodeActivity));
            if (!string.IsNullOrEmpty(folder))
            {
                intent.PutExtra(ACTIVITY_PARAM_FOLDER, folder);
            }
            return intent;
        }

        private AndroidApplication AndroidApplication = null!;
        private ShareEpisodeViewModel ViewModel = null!;

        private EmptyRecyclerView RvEpisodes = null!;
        private LinearLayout NoDataView = null!;
        private ProgressSpinnerView ProgressSpinner = null!;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            AndroidApplication = (AndroidApplication)Application!;
            AndroidApplication.Logger.Debug(() => $"ShareEpisodeActivity:OnCreate");

            base.OnCreate(savedInstanceState);

            // Set our view from the layout resource
            SetContentView(Resource.Layout.activity_share_episode);

            RvEpisodes = FindViewById<EmptyRecyclerView>(Resource.Id.rvEpisodes)!;
            NoDataView = FindViewById<LinearLayout>(Resource.Id.layNoData)!;
            ProgressSpinner = FindViewById<ProgressSpinnerView>(Resource.Id.progressBar)!;

            RvEpisodes.SetLayoutManager(new LinearLayoutManager(this));
            RvEpisodes.AddItemDecoration(new DividerItemDecoration(this, DividerItemDecoration.Vertical));
            RvEpisodes.SetEmptyView(NoDataView);

            var factory = AndroidApplication.IocContainer?.Resolve<ViewModelFactory>() ?? throw new MissingMemberException("ViewModelFactory");
            ViewModel = (ShareEpisodeViewModel)new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(ShareEpisodeViewModel)));

            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            var folder = Intent?.GetStringExtra(ACTIVITY_PARAM_FOLDER);

            ViewModel.Initialise(folder);
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"ShareEpisodeActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"ShareEpisodeActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            if (OperatingSystem.IsAndroidVersionAtLeast(23))
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        public override bool DispatchKeyEvent(KeyEvent? e)
        {
            if (BackKeyMapper.HandleKeyEvent(this, e))
            {
                AndroidApplication.Logger.Debug(() => $"ShareEpisodeActivity:DispatchKeyEvent - handled");
                return true;
            }
            return base.DispatchKeyEvent(e);
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.DisplayMessage += ToastMessage;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.Title -= SetTitle;
            ViewModel.Observables.DisplayMessage -= ToastMessage;
        }

        private void SetTitle(object? sender, string title)
        {
            RunOnUiThread(() =>
            {
                Title = title;
            });
        }

        private void ToastMessage(object? sender, string message)
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(Application.Context, message, ToastLength.Short)?.Show();
            });
        }

    }
}

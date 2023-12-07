using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Widget;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Configure;

namespace PodcastUtilities.UI.Configure
{
    [Activity(Label = "@string/global_values_activity_title", ParentActivity = typeof(EditConfigActivity))]
    internal class GlobalValuesActivity : AppCompatActivity
    {
        private AndroidApplication AndroidApplication;
        private GlobalValuesViewModel ViewModel;

        private NestedScrollView Container = null;
        private LinearLayoutCompat DownloadFreeSpaceRowContainer = null;
        private TextView DownloadFreeSpaceRowSubLabel = null;
        private LinearLayoutCompat PlaylistFileRowContainer = null;
        private TextView PlaylistFileRowSubLabel = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity:OnCreate");

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_global_values);

            Container = FindViewById<NestedScrollView>(Resource.Id.feed_defaults_container);
            DownloadFreeSpaceRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.download_free_space_row_label_container);
            DownloadFreeSpaceRowSubLabel = FindViewById<TextView>(Resource.Id.download_free_space_row_sub_label);
            PlaylistFileRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.playlist_file_row_label_container);
            PlaylistFileRowSubLabel = FindViewById<TextView>(Resource.Id.playlist_file_row_sub_label);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(GlobalValuesViewModel))) as GlobalValuesViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();

            DownloadFreeSpaceRowContainer.Click += (sender, e) => DoDownloadFreeSpaceOptions();
            PlaylistFileRowContainer.Click += (sender, e) => DoPlaylistFileOptions();

            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity:OnCreate - end");
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void DoDownloadFreeSpaceOptions()
        {
            Toast.MakeText(Application.Context, "Not implemented", ToastLength.Short).Show();
        }

        private void DoPlaylistFileOptions()
        {
            Toast.MakeText(Application.Context, "Not implemented", ToastLength.Short).Show();
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.DisplayMessage += DisplayMessage;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.DisplayMessage -= DisplayMessage;
        }

        private void DisplayMessage(object sender, string message)
        {
            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity: DisplayMessage {message}");
            RunOnUiThread(() =>
            {
                Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
            });
        }
    }

}
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
using System;

namespace PodcastUtilities.UI.Configure
{
    [Activity(Label = "@string/feed_defaults_activity_title", ParentActivity = typeof(EditConfigActivity))]
    internal class FeedDefaultsActivity : AppCompatActivity
    {
        private AndroidApplication AndroidApplication;
        private FeedDefaultsViewModel ViewModel;

        private NestedScrollView Container = null;
        private LinearLayoutCompat MaxDaysOldRowContainer = null;
        private TextView MaxDaysOldRowSubLabel = null;
        private LinearLayoutCompat DownloadStrategyRowContainer = null;
        private TextView DownloadStrategyRowSubLabel = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"FeedDefaultsActivity:OnCreate");

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_feed_defaults);

            Container = FindViewById<NestedScrollView>(Resource.Id.feed_defaults_container);
            MaxDaysOldRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.max_days_old_row_label_container);
            MaxDaysOldRowSubLabel = FindViewById<TextView>(Resource.Id.max_days_old_row_sub_label);
            DownloadStrategyRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.download_strategy_row_label_container);
            DownloadStrategyRowSubLabel = FindViewById<TextView>(Resource.Id.download_strategy_row_sub_label);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(FeedDefaultsViewModel))) as FeedDefaultsViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();

            MaxDaysOldRowContainer.Click += (sender, e) => DoMaxDaysOldOptions();
            DownloadStrategyRowContainer.Click += (sender, e) => DoDownloadStrategyOptions();

            AndroidApplication.Logger.Debug(() => $"FeedDefaultsActivity:OnCreate - end");
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"FeedDefaultsActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"FeedDefaultsActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void DoDownloadStrategyOptions()
        {
            Toast.MakeText(Application.Context, "Not implemented", ToastLength.Short).Show();
        }

        private void DoMaxDaysOldOptions()
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
            AndroidApplication.Logger.Debug(() => $"EditConfigActivity: DisplayMessage {message}");
            RunOnUiThread(() =>
            {
                Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
            });
        }
    }
}
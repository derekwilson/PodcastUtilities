using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Main;
using System;

namespace PodcastUtilities
{
#if DEBUG
    [Activity(Label = "@string/app_name_debug", Theme = "@style/AppTheme", MainLauncher = true)]
#else
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
#endif
    public class MainActivity : AppCompatActivity
    {
        private AndroidApplication AndroidApplication;
        private MainViewModel ViewModel;

        LinearLayout DriveInfoContainerView = null;
        TextView NoDriveDataView = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnCreate");

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            DriveInfoContainerView = FindViewById<LinearLayout>(Resource.Id.drive_info_container);
            NoDriveDataView = FindViewById<TextView>(Resource.Id.txtNoData);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(MainViewModel))) as MainViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();

            if (PermissionChecker.HasManageStoragePermission(this))
            {
                ViewModel?.RefreshFileSystemInfo();
            }
            else
            {
                AndroidApplication.Logger.Debug(() => $"MainActivity:OnCreate - permission not granted - requesting");
                PermissionRequester.RequestManageStoragePermission(this, PermissionRequester.REQUEST_CODE_WRITE_EXTERNAL_STORAGE_PERMISSION, AndroidApplication.PackageName);
            }

            AndroidApplication.Logger.Debug(() => $"MainActivity:OnCreate - end");
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            switch (requestCode)
            {
                // for manage storage on SDK30+ it will go to activity result - thanks google
                // also we get CANCELLED as the result code so its difficult to know if it worked
                case PermissionRequester.REQUEST_CODE_WRITE_EXTERNAL_STORAGE_PERMISSION:
                    if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                    {
                        ViewModel?.RefreshFileSystemInfo();
                    }
                    else
                    {
                        ToastMessage("Permission Denied");
                    }
                    break;
                default:
                    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                    break;
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnActivityResult {requestCode}, {resultCode}");
            base.OnActivityResult(requestCode, resultCode, data);

            AndroidApplication.Logger.Debug(() => $"MainActivity:OnActivityResult {data.Data.ToString()}");
            switch (requestCode)
            {
                // we asked for manage storage access in SDK30+
                case PermissionRequester.REQUEST_CODE_WRITE_EXTERNAL_STORAGE_PERMISSION:
                    ViewModel?.RefreshFileSystemInfo();
                    break;
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            KillViewModelObservers();
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.AddInfoView += AddInfoView;
            ViewModel.Observables.ShowNoDriveMessage += ShowNoDriveMessage;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.Title -= SetTitle;
            ViewModel.Observables.AddInfoView -= AddInfoView;
            ViewModel.Observables.ShowNoDriveMessage += ShowNoDriveMessage;
        }

        private void SetTitle(object sender, string title)
        {
            RunOnUiThread(() =>
            {
                Title = title;
            });
        }

        private void ShowNoDriveMessage(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                NoDriveDataView.Visibility = ViewStates.Visible;
            });
        }

        private void AddInfoView(object sender, DriveVolumeInfoView view)
        {
            RunOnUiThread(() =>
            {
                DriveInfoContainerView?.AddView(view);
                NoDriveDataView.Visibility = ViewStates.Gone;
            });
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
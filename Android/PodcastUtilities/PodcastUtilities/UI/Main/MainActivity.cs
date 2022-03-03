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
using PodcastUtilities.UI.Settings;
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

            if (!PermissionChecker.HasManageStoragePermission(this))
            {
                AndroidApplication.Logger.Debug(() => $"MainActivity:OnCreate - permission not granted - requesting");
                PermissionRequester.RequestManageStoragePermission(this, PermissionRequester.REQUEST_CODE_WRITE_EXTERNAL_STORAGE_PERMISSION, AndroidApplication.PackageName);
            }

            AndroidApplication.Logger.Debug(() => $"MainActivity:OnCreate - end - observers {GetObserverCount()}");
        }

        private string GetObserverCount()
        {
            if (ViewModel == null)
            {
                return "null viewmodel";
            }
            if (ViewModel.Observables == null)
            {
                return "null viewmodel observables";
            }
            if (ViewModel.Observables.NavigateToSettings == null)
            {
                return "null viewmodel observable navigate";
            }

            var list = ViewModel.Observables.NavigateToSettings.GetInvocationList();
            if (list == null)
            {
                return "null invoke list";
            }
            try
            {
                return list.Length.ToString();
            } 
            catch 
            {
                return "error counting list";
            }
        }

        protected override void OnResume()
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnResume - observers {GetObserverCount()}");
            
            base.OnResume();
            if (PermissionChecker.HasManageStoragePermission(this))
            {
                ViewModel?.RefreshFileSystemInfo();
            }
        }

        protected override void OnPause()
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnPause - observers {GetObserverCount()}");
            base.OnPause();
        }

        protected override void OnStop()
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnStop - observers {GetObserverCount()}");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnDestroy - observers {GetObserverCount()}");
            base.OnDestroy();
            KillViewModelObservers();
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            EnableMenuItemIfAvailable(menu, Resource.Id.action_load_config);
            EnableMenuItemIfAvailable(menu, Resource.Id.action_settings);
            return true;
        }

        private void EnableMenuItemIfAvailable(IMenu menu, int itemId)
        {
            menu.FindItem(itemId)?.SetEnabled(ViewModel.isActionAvailable(itemId));
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (ViewModel.ActionSelected(item.ItemId))
            {
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.AddInfoView += AddInfoView;
            ViewModel.Observables.ShowNoDriveMessage += ShowNoDriveMessage;
            ViewModel.Observables.NavigateToSettings += NavigateToSettings;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.Title -= SetTitle;
            ViewModel.Observables.AddInfoView -= AddInfoView;
            ViewModel.Observables.ShowNoDriveMessage -= ShowNoDriveMessage;
            ViewModel.Observables.NavigateToSettings -= NavigateToSettings;
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
            AndroidApplication.Logger.Debug(() => $"MainActivity: ShowNoDriveMessage");
            RunOnUiThread(() =>
            {
                NoDriveDataView.Visibility = ViewStates.Visible;
                DriveInfoContainerView.Visibility = ViewStates.Gone;
                DriveInfoContainerView.RemoveAllViews();
            });
        }

        private void AddInfoView(object sender, DriveVolumeInfoView view)
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity: AddInfoView");
            RunOnUiThread(() =>
            {
                NoDriveDataView.Visibility = ViewStates.Gone;
                DriveInfoContainerView.Visibility = ViewStates.Visible;
                DriveInfoContainerView?.AddView(view);
            });
        }

        private void ToastMessage(string message)
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity: ToastMessage {message}");
            RunOnUiThread(() =>
            {
                Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
            });
        }

        private void NavigateToSettings(object sender, EventArgs e)
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity: NavigateToSettings");
            RunOnUiThread(() =>
            {
                var intent = new Intent(this, typeof(SettingsActivity));
                StartActivity(intent);
            });
        }
    }
}
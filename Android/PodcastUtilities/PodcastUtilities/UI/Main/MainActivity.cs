using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using AndroidX.Core.Widget;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.FloatingActionButton;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Main;
using PodcastUtilities.UI.Configure;
using PodcastUtilities.UI.Download;
using PodcastUtilities.UI.Purge;
using PodcastUtilities.UI.Settings;
using System;
using System.Collections.Generic;

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

        private IPermissionChecker PermissionChecker;

        private NestedScrollView Container = null;
        private LinearLayout DriveInfoContainerView = null;
        private TextView NoDriveDataView = null;
        private EmptyRecyclerView RvFeeds;
        private LinearLayout NoFeedView;
        private PodcastFeedRecyclerItemAdapter FeedAdapter;
        private TextView CacheRoot = null;
        private TextView FeedsTitle = null;

        private FloatingActionButton PurgeButton;
        private FloatingActionButton DownloadButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnCreate");
            PermissionChecker = new PermissionChecker();

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Container = FindViewById<NestedScrollView>(Resource.Id.container);  
            DriveInfoContainerView = FindViewById<LinearLayout>(Resource.Id.drive_info_container);
            NoDriveDataView = FindViewById<TextView>(Resource.Id.txtNoData);
            RvFeeds = FindViewById<EmptyRecyclerView>(Resource.Id.feed_list);
            NoFeedView = FindViewById<LinearLayout>(Resource.Id.layNoFeed);
            CacheRoot = FindViewById<TextView>(Resource.Id.cache_root_value);
            FeedsTitle = FindViewById<TextView>(Resource.Id.feed_list_label);
            PurgeButton = FindViewById<FloatingActionButton>(Resource.Id.fab_main_purge);
            DownloadButton = FindViewById<FloatingActionButton>(Resource.Id.fab_main_download);

            RvFeeds.SetLayoutManager(new LinearLayoutManager(this));
            RvFeeds.AddItemDecoration(new DividerItemDecoration(this, DividerItemDecoration.Vertical));
            RvFeeds.SetEmptyView(NoFeedView);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(MainViewModel))) as MainViewModel;

            FeedAdapter = new PodcastFeedRecyclerItemAdapter(this, ViewModel);
            RvFeeds.SetAdapter(FeedAdapter);

            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();

            PurgeButton.Click += (sender, e) => DoAction(Resource.Id.action_purge);
            DownloadButton.Click += (sender, e) => DoAction(Resource.Id.action_download);

            if (!PermissionChecker.HasManageStoragePermission(this))
            {
                AndroidApplication.Logger.Debug(() => $"MainActivity:OnCreate - manage storage permission not granted - requesting");
                PermissionRequester.RequestManageStoragePermission(this, PermissionRequester.REQUEST_CODE_WRITE_EXTERNAL_STORAGE_PERMISSION, AndroidApplication.PackageName);
            }
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnCreate - end - observers {GetObserverCount()}");
        }

        protected override void OnResume()
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnResume - observers {GetObserverCount()}");

            base.OnResume();
            if (PermissionChecker.HasManageStoragePermission(this))
            {
                AndroidApplication.Logger.Debug(() => $"MainActivity:OnResume - we have manage storage");
                ViewModel?.RefreshFileSystemInfo();
                AndroidApplication.Logger.Debug(() => $"MainActivity:OnResume - write storage permission = {PermissionChecker.HasWriteStoragePermission(this)}");
                if (!PermissionChecker.HasWriteStoragePermission(this))
                {
                    PermissionRequester.RequestWriteStoragePermission(this, PermissionRequester.REQUEST_CODE_WRITE_EXTERNAL_STORAGE_PERMISSION);
                }
            }
        }

        private void RequestPostNotificationPermission()
        {
            if (!PermissionChecker.HasPostNotifcationPermission(this))
            {
                AndroidApplication.Logger.Debug(() => $"MainActivity:RequestPostNotificationPermission - post notification permission = {PermissionChecker.HasPostNotifcationPermission(this)}");
                PermissionRequester.RequestPostNotificationPermission(this, PermissionRequester.REQUEST_CODE_POST_NOTIFICATION_PERMISSION);
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
                // for manage storage on SDK30+ the result will go to OnActivityResult - thanks google
                // also we get CANCELLED as the result code so its difficult to know if it worked
                case PermissionRequester.REQUEST_CODE_WRITE_EXTERNAL_STORAGE_PERMISSION:
                    if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                    {
                        ViewModel?.RefreshFileSystemInfo();
                        RequestPostNotificationPermission();
                    }
                    else
                    {
                        ToastMessage("Permission Denied");
                    }
                    break;
                case PermissionRequester.REQUEST_CODE_POST_NOTIFICATION_PERMISSION:
                    if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                    {
                        AndroidApplication.Logger.Debug(() => $"MainActivity:OnRequestPermissionsResult post notification OK");
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

            AndroidApplication.Logger.Debug(() => $"MainActivity:OnActivityResult data = {data?.Data?.ToString()}");
            switch (requestCode)
            {
                // we asked for manage storage access in SDK30+
                case PermissionRequester.REQUEST_CODE_WRITE_EXTERNAL_STORAGE_PERMISSION:
                    ViewModel?.RefreshFileSystemInfo();
                    RequestPostNotificationPermission();
                    break;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            // we want a separator on the menu
            MenuCompat.SetGroupDividerEnabled(menu, true);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            EnableMenuItemIfAvailable(menu, Resource.Id.action_edit_config);
            EnableMenuItemIfAvailable(menu, Resource.Id.action_purge);
            EnableMenuItemIfAvailable(menu, Resource.Id.action_download);
            EnableMenuItemIfAvailable(menu, Resource.Id.action_playlist);
            EnableMenuItemIfAvailable(menu, Resource.Id.action_settings);
            EnableFabsIfAvailable();
            return true;
        }

        private void EnableMenuItemIfAvailable(IMenu menu, int itemId)
        {
            menu.FindItem(itemId)?.SetEnabled(ViewModel.IsActionAvailable(itemId));
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (!PermissionChecker.HasManageStoragePermission(this))
            {
                ToastMessage(GetString(Resource.String.manage_external_permissions_rationale));
                return base.OnOptionsItemSelected(item);
            }
            if (ViewModel.ActionSelected(item.ItemId))
            {
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (BackKeyMapper.HandleKeyEvent(this, e))
            {
                AndroidApplication.Logger.Debug(() => $"MainActivity:DispatchKeyEvent - handled");
                return true;
            }
            if (ViewModel.KeyEvent(e))
            {
                AndroidApplication.Logger.Debug(() => $"MainActivity:DispatchKeyEvent - handled by model");
                return true;
            }
            return base.DispatchKeyEvent(e);
        }

        private void DoAction(int action)
        {
            if (!PermissionChecker.HasManageStoragePermission(this))
            {
                ToastMessage(GetString(Resource.String.manage_external_permissions_rationale));
                return;
            }
            ViewModel.ActionSelected(action);
        }

        private void EnableFabsIfAvailable()
        {
            DownloadButton.Enabled = ViewModel.IsActionAvailable(Resource.Id.action_download);
            PurgeButton.Enabled = ViewModel.IsActionAvailable(Resource.Id.action_purge);
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.AddInfoView += AddInfoView;
            ViewModel.Observables.ToastMessage += ToastMessage;
            ViewModel.Observables.ShowNoDriveMessage += ShowNoDriveMessage;
            ViewModel.Observables.NavigateToSettings += NavigateToSettings;
            ViewModel.Observables.SetFeedItems += SetFeedItems;
            ViewModel.Observables.SetCacheRoot += SetCacheRoot;
            ViewModel.Observables.NavigateToDownload += NavigateToDownload;
            ViewModel.Observables.NavigateToPurge += NavigateToPurge;
            ViewModel.Observables.NavigateToEditConfig += NavigateToEditConfig;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.Title -= SetTitle;
            ViewModel.Observables.AddInfoView -= AddInfoView;
            ViewModel.Observables.ToastMessage -= ToastMessage;
            ViewModel.Observables.ShowNoDriveMessage -= ShowNoDriveMessage;
            ViewModel.Observables.NavigateToSettings -= NavigateToSettings;
            ViewModel.Observables.SetFeedItems -= SetFeedItems;
            ViewModel.Observables.SetCacheRoot -= SetCacheRoot;
            ViewModel.Observables.NavigateToDownload -= NavigateToDownload;
            ViewModel.Observables.NavigateToPurge -= NavigateToPurge;
            ViewModel.Observables.NavigateToEditConfig -= NavigateToEditConfig;
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

        private void AddInfoView(object sender, View view)
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity: AddInfoView");
            RunOnUiThread(() =>
            {
                NoDriveDataView.Visibility = ViewStates.Gone;
                DriveInfoContainerView.Visibility = ViewStates.Visible;
                DriveInfoContainerView?.AddView(view);
            });
        }

        private void ToastMessage(object sender, string message)
        {
            ToastMessage(message);
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

        private void NavigateToDownload(object sender, string folder)
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity: NavigateToDownload");
            RunOnUiThread(() =>
            {
                var intent = DownloadActivity.CreateIntent(this, folder);
                StartActivity(intent);
            });
        }

        private void NavigateToPurge(object sender, EventArgs e)
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity: NavigateToPurge");
            RunOnUiThread(() =>
            {
                var intent = new Intent(this, typeof(PurgeActivity));
                StartActivity(intent);
            });
        }

        private void NavigateToEditConfig(object sender, EventArgs e)
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity: NavigateToEditConfig");
            RunOnUiThread(() =>
            {
                var intent = new Intent(this, typeof(EditConfigActivity));
                StartActivity(intent);
            });
        }

        private void SetFeedItems(object sender, Tuple<string, List<PodcastFeedRecyclerItem>> feeditems)
        {
            (string heading, List<PodcastFeedRecyclerItem> items) = feeditems;
            RunOnUiThread(() =>
            {
                FeedsTitle.Text = heading;
                FeedAdapter.SetItems(items);
                FeedAdapter.NotifyDataSetChanged();
                EnableFabsIfAvailable();
                ViewScrollHelper.ScrollToTop(Container);
            });
        }

        private void SetCacheRoot(object sender, string root)
        {
            RunOnUiThread(() =>
            {
                CacheRoot.Text = root;
            });
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
    }
}


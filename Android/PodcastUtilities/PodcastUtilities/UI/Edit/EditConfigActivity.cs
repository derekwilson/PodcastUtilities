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
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Edit;
using System;

namespace PodcastUtilities.UI.Edit
{
    [Activity(Label = "@string/edit_config_activity_title", ParentActivity = typeof(MainActivity))]
    internal class EditConfigActivity : AppCompatActivity
    {
        private AndroidApplication AndroidApplication;
        private EditConfigViewModel ViewModel;

        private NestedScrollView Container = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"EditConfigActivity:OnCreate");

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_edit_config);

            Container = FindViewById<NestedScrollView>(Resource.Id.edit_container);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(EditConfigViewModel))) as EditConfigViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();

            AndroidApplication.Logger.Debug(() => $"EditConfigActivity:OnCreate - end");
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"EditConfigActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"EditConfigActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (BackKeyMapper.HandleKeyEvent(this, e))
            {
                AndroidApplication.Logger.Debug(() => $"EditConfigActivity:DispatchKeyEvent - handled");
                return true;
            }
            if (ViewModel.KeyEvent(e))
            {
                AndroidApplication.Logger.Debug(() => $"EditConfigActivity:DispatchKeyEvent - handled by model");
                return true;
            }
            return base.DispatchKeyEvent(e);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_edit_config, menu);
            // we want a separator on the menu
            MenuCompat.SetGroupDividerEnabled(menu, true);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            EnableMenuItemIfAvailable(menu, Resource.Id.action_edit_share_control);
            EnableMenuItemIfAvailable(menu, Resource.Id.action_edit_reset_control);
            EnableMenuItemIfAvailable(menu, Resource.Id.action_edit_cache_root);
            EnableMenuItemIfAvailable(menu, Resource.Id.action_edit_globals);
            return true;
        }

        private void EnableMenuItemIfAvailable(IMenu menu, int itemId)
        {
            menu.FindItem(itemId)?.SetEnabled(ViewModel.IsActionAvailable(itemId));
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (ViewModel.ActionSelected(item.ItemId))
            {
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.DisplayMessage += DisplayMessage;
            ViewModel.Observables.DisplayChooser += DisplayChooser;
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.DisplayMessage -= DisplayMessage;
            ViewModel.Observables.DisplayChooser -= DisplayChooser;
        }

        private void DisplayChooser(object sender, Tuple<string, Intent> args)
        {
            (string title, Intent intent) = args;
            StartActivity(Intent.CreateChooser(intent, title));
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
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
using AndroidX.DocumentFile.Provider;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Edit;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.UI.Edit
{
    [Activity(Label = "@string/edit_config_activity_title", ParentActivity = typeof(MainActivity))]
    internal class EditConfigActivity : AppCompatActivity, SelectableStringListBottomSheetFragment.IListener
    {
        private const int REQUEST_SELECT_FILE = 3000;
        private const int REQUEST_SELECT_FOLDER = 3001;

        private const string BOTTOMSHEET_CACHE_OPTIONS_TAG = "cache_options_tag";
        private const string RESET_PROMPT_TAG = "reset_prompt_tag";

        private AndroidApplication AndroidApplication;
        private EditConfigViewModel ViewModel;

        private NestedScrollView Container = null;
        private TextView CacheRootSubLabel = null;
        private TextView CacheRootOptions = null;

        private OkCancelDialogFragment ResetPromptDialogFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"EditConfigActivity:OnCreate");

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_edit_config);

            Container = FindViewById<NestedScrollView>(Resource.Id.edit_container);
            CacheRootSubLabel = FindViewById<TextView>(Resource.Id.cache_root_row_sub_label);
            CacheRootOptions = FindViewById<TextView>(Resource.Id.cache_root_row_options);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(EditConfigViewModel))) as EditConfigViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();

            CacheRootOptions.Click += (sender, e) => DoCacheRootOptions();

            ResetPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(RESET_PROMPT_TAG) as OkCancelDialogFragment;
            SetupFragmentObservers(ResetPromptDialogFragment);

            AndroidApplication.Logger.Debug(() => $"EditConfigActivity:OnCreate - end");
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"EditConfigActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
            KillFragmentObservers(ResetPromptDialogFragment);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"EditConfigActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            AndroidApplication.Logger.Debug(() => $"EditConfigActivity:OnActivityResult {requestCode}, {resultCode}");
            base.OnActivityResult(requestCode, resultCode, data);
            if (!resultCode.Equals(Result.Ok))
            {
                return;
            }

            AndroidApplication.Logger.Debug(() => $"EditConfigActivity:OnActivityResult {data.Data.ToString()}");
            switch (requestCode)
            {
                case REQUEST_SELECT_FILE:
                    ViewModel?.LoadContolFile(data.Data);
                    break;
                case REQUEST_SELECT_FOLDER:
                    Android.Net.Uri uri = data.Data;

                    //AndroidApplication.Logger.Debug(() => $"EditConfigActivity:OnActivityResult treeUri {uri}");
                    //GrantUriPermission(AndroidApplication.PackageName, uri, ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
                    //AndroidApplication.ContentResolver.TakePersistableUriPermission(uri, ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);

                    DocumentFile folder = DocumentFile.FromTreeUri(ApplicationContext, uri);
                    AndroidApplication.Logger.Debug(() => $"EditConfigActivity:OnActivityResult folder {folder.Uri.Path}");
                    ViewModel.FolderSelected(folder);
                    break;
            }
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
            EnableMenuItemIfAvailable(menu, Resource.Id.action_edit_load_control);
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

        private void DoCacheRootOptions()
        {
            AndroidApplication.Logger.Debug(() => $"EditConfigActivity:DoCacheRootOptions");
            List<SelectableString> options = ViewModel.GetCacheRootOptions();
            var sheet = SelectableStringListBottomSheetFragment.NewInstance(
                GetString(Resource.String.cache_root_sheet_title), 
                options);
            sheet.Show(this.SupportFragmentManager, BOTTOMSHEET_CACHE_OPTIONS_TAG);
        }

        public void BottomsheetItemSelected(string tag, int position, SelectableString item)
        {
            AndroidApplication.Logger.Debug(() => $"EditConfigActivity:BottomsheetItemSelected {tag}, {position}");
            switch (tag)
            {
                case BOTTOMSHEET_CACHE_OPTIONS_TAG:
                    Toast.MakeText(Application.Context, $"BottomsheetItemSelected Options clicked = {position}", ToastLength.Short).Show();
                    break;
            }
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.DisplayMessage += DisplayMessage;
            ViewModel.Observables.DisplayChooser += DisplayChooser;
            ViewModel.Observables.ResetPrompt += ResetPrompt;
            ViewModel.Observables.SelectFolder += SelectFolder;
            ViewModel.Observables.SelectControlFile += SelectControlFile;
            ViewModel.Observables.SetCacheRoot += SetCacheRoot;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.DisplayMessage -= DisplayMessage;
            ViewModel.Observables.DisplayChooser -= DisplayChooser;
            ViewModel.Observables.ResetPrompt -= ResetPrompt;
            ViewModel.Observables.SelectFolder -= SelectFolder;
            ViewModel.Observables.SelectControlFile -= SelectControlFile;
            ViewModel.Observables.SetCacheRoot -= SetCacheRoot;
        }

        private void SelectControlFile(object sender, EventArgs e)
        {
            AndroidApplication.Logger.Debug(() => $"EditConfigActivity: SelectControlFile");
            // ACTION_OPEN_DOCUMENT is the intent to choose a file via the system's file browser.
            var intent = new Intent(Intent.ActionOpenDocument);

            // Filter to only show results that can be "opened", such as a
            // file (as opposed to a list of contacts or timezones)
            intent.AddCategory(Intent.CategoryOpenable);

            // Filter using the MIME type.
            // If one wanted to search for ogg vorbis files, the type would be "audio/ogg".
            // To search for all documents available via installed storage providers, it would be "*/*".
            // as we know that other apps do not always report GPX MIME type correctly lets try for everything
            intent.SetType("*/*");

            RunOnUiThread(() =>
            {
                StartActivityForResult(intent, REQUEST_SELECT_FILE);
            });
        }

        private void SelectFolder(object sender, EventArgs e)
        {
            AndroidApplication.Logger.Debug(() => $"EditConfigActivity:SelectFolder");
            Intent intent = new Intent(Intent.ActionOpenDocumentTree);
            StartActivityForResult(intent, REQUEST_SELECT_FOLDER);
        }

        private void ResetPrompt(object sender, Tuple<string, string, string, string> parameters)
        {
            RunOnUiThread(() =>
            {
                (string title, string message, string ok, string cancel) = parameters;
                ResetPromptDialogFragment = OkCancelDialogFragment.NewInstance(title, message, ok, cancel, null);
                SetupFragmentObservers(ResetPromptDialogFragment);
                ResetPromptDialogFragment.Show(SupportFragmentManager, RESET_PROMPT_TAG);
            });
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

        private void SetCacheRoot(object sender, string root)
        {
            AndroidApplication.Logger.Debug(() => $"EditConfigActivity: SetCacheRoot {root}");
            RunOnUiThread(() =>
            {
                CacheRootSubLabel.Text = root;
            });
        }

        private void SetupFragmentObservers(OkCancelDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"EditConfigActivity: SetupFragmentObservers - {fragment.Tag}");
                fragment.OkSelected += OkSelected;
                fragment.CancelSelected += CancelSelected;
            }
        }

        private void KillFragmentObservers(OkCancelDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"EditConfigActivity: KillFragmentObservers - {fragment.Tag}");
                fragment.OkSelected -= OkSelected;
                fragment.CancelSelected -= CancelSelected;
            }
        }

        private void CancelSelected(object sender, Tuple<string, string> parameters)
        {
            (string tag, string data) = parameters;
            AndroidApplication.Logger.Debug(() => $"CancelSelected: {tag}");
            switch (tag)
            {
                case RESET_PROMPT_TAG:
                    KillFragmentObservers(ResetPromptDialogFragment);
                    break;
            }
        }

        private void OkSelected(object sender, Tuple<string, string> parameters)
        {
            (string tag, string data) = parameters;
            AndroidApplication.Logger.Debug(() => $"OkSelected: {tag}");
            RunOnUiThread(() =>
            {
                switch (tag)
                {
                    case RESET_PROMPT_TAG:
                        KillFragmentObservers(ResetPromptDialogFragment);
                        ViewModel.ResetConfirmed();
                        break;
                }
            });
        }
    }
}
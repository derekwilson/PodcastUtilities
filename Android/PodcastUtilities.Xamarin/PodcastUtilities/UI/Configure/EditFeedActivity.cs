using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Widget;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.ViewModel.Configure;
using PodcastUtilities.AndroidLogic.ViewModel;
using AndroidX.AppCompat.App;
using Android.Content.PM;
using System.Collections.Generic;
using static PodcastUtilities.AndroidLogic.CustomViews.DefaultableItemValuePromptDialogFragment;
using System;
using Android.Content;
using Android.Graphics;
using Android.Views;
using PodcastUtilities.AndroidLogic.Utilities;
using AndroidX.Core.View;
using PodcastUtilities.UI.Download;
using Google.Android.Material.FloatingActionButton;

namespace PodcastUtilities.UI.Configure
{
    [Activity(Label = "@string/edit_feed_activity_title", ParentActivity = typeof(EditConfigActivity))]
    internal class EditFeedActivity : AppCompatActivity, SelectableStringListBottomSheetFragment.IListener
    {
        private const string ACTIVITY_PARAM_ID_CONFIG = "EditFeedActivity:Param:Id";

        public static Intent CreateIntent(Context context, string id)
        {
            Intent intent = new Intent(context, typeof(EditFeedActivity));
            if (!string.IsNullOrEmpty(id))
            {
                intent.PutExtra(ACTIVITY_PARAM_ID_CONFIG, id);
            }
            return intent;
        }

        private const string BOTTOMSHEET_DOWNLOAD_STRATEGY_CONFIG_TAG = "config_download_strategy_tag";
        private const string BOTTOMSHEET_NAMING_STYLE_CONFIG_TAG = "config_naming_style_tag";

        private const string FOLDER_PROMPT_TAG = "folder_prompt_tag";
        private const string URL_PROMPT_TAG = "url_prompt_tag";
        private const string MAX_DAYS_OLD_PROMPT_CONFIG_TAG = "config_max_days_old_prompt_tag";
        private const string DELETE_DOWNLOAD_DAYS_OLD_PROMPT_CONFIG_TAG = "config_delete_download_prompt_tag";
        private const string MAX_DOWNLOAD_ITEMS_PROMPT_CONFIG_TAG = "config_max_download_items_prompt_tag";
        private const string DELETE_PROMPT_TAG = "delete_prompt_tag";

        private AndroidApplication AndroidApplication;
        private EditFeedViewModel ViewModel;

        private NestedScrollView Container = null;
        private LinearLayoutCompat FolderRowContainer = null;
        private TextView FolderRowSubLabel = null;
        private LinearLayoutCompat UrlRowContainer = null;
        private TextView UrlRowSubLabel = null;
        private LinearLayoutCompat DownloadStrategyRowContainer = null;
        private TextView DownloadStrategyRowSubLabel = null;
        private LinearLayoutCompat NamingStyleRowContainer = null;
        private TextView NamingStyleRowSubLabel = null;
        private LinearLayoutCompat MaxDaysOldRowContainer = null;
        private TextView MaxDaysOldRowSubLabel = null;
        private LinearLayoutCompat DeleteDaysOldRowContainer = null;
        private TextView DeleteDaysOldRowSubLabel = null;
        private LinearLayoutCompat MaxDownloadItemsRowContainer = null;
        private TextView MaxDownloadItemsRowSubLabel = null;

        private FloatingActionButton RemoveButton;
        private FloatingActionButton ShareButton;

        private ValuePromptDialogFragment FolderPromptDialogFragment;
        private ValuePromptDialogFragment UrlPromptDialogFragment;
        private DefaultableItemValuePromptDialogFragment MaxDaysOldPromptDialogFragment;
        private DefaultableItemValuePromptDialogFragment DeleteDaysOldPromptDialogFragment;
        private DefaultableItemValuePromptDialogFragment MaxDownloadItemsPromptDialogFragment;

        private OkCancelDialogFragment DeletePodcastPromptDialogFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"EditFeedActivity:OnCreate");

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_edit_feed);

            var id = Intent?.GetStringExtra(ACTIVITY_PARAM_ID_CONFIG);
            if (id == null)
            {
                AndroidApplication.Logger.Debug(() => $"EditFeedActivity:OnCreate - no id specified");
            }

            Container = FindViewById<NestedScrollView>(Resource.Id.edit_feed_container);
            FolderRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.feed_folder_row_label_container);
            FolderRowSubLabel = FindViewById<TextView>(Resource.Id.feed_folder_row_sub_label);
            UrlRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.feed_url_row_label_container);
            UrlRowSubLabel = FindViewById<TextView>(Resource.Id.feed_url_row_sub_label);
            DownloadStrategyRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.feed_download_strategy_row_label_container);
            DownloadStrategyRowSubLabel = FindViewById<TextView>(Resource.Id.feed_download_strategy_row_sub_label);
            NamingStyleRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.feed_naming_style_row_label_container);
            NamingStyleRowSubLabel = FindViewById<TextView>(Resource.Id.feed_naming_style_row_sub_label);
            MaxDaysOldRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.feed_max_days_old_row_label_container);
            MaxDaysOldRowSubLabel = FindViewById<TextView>(Resource.Id.feed_max_days_old_row_sub_label);
            DeleteDaysOldRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.feed_delete_download_days_old_row_label_container);
            DeleteDaysOldRowSubLabel = FindViewById<TextView>(Resource.Id.feed_delete_download_days_old_row_sub_label);
            MaxDownloadItemsRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.feed_max_download_items_row_label_container);
            MaxDownloadItemsRowSubLabel = FindViewById<TextView>(Resource.Id.feed_max_download_items_row_sub_label);

            RemoveButton = FindViewById<FloatingActionButton>(Resource.Id.fab_config_remove_podcast);
            ShareButton = FindViewById<FloatingActionButton>(Resource.Id.fab_config_share);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(EditFeedViewModel))) as EditFeedViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise(id);

            FolderRowContainer.Click += (sender, e) => DoFolderOptions();
            UrlRowContainer.Click += (sender, e) => DoUrlOptions();
            DownloadStrategyRowContainer.Click += (sender, e) => DoDownloadStrategyOptions();
            NamingStyleRowContainer.Click += (sender, e) => DoNamingStyleOptions();
            MaxDaysOldRowContainer.Click += (sender, e) => DoMaxDaysOldOptions();
            DeleteDaysOldRowContainer.Click += (sender, e) => DoDeleteDownloadDaysOldOptions();
            MaxDownloadItemsRowContainer.Click += (sender, e) => DoMaxDownloadItemsOptions();
            RemoveButton.Click += (sender, e) => ViewModel.RemovePodcastSelected();
            ShareButton.Click += (sender, e) => ViewModel.SharePodcastSelected();

            FolderPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(FOLDER_PROMPT_TAG) as ValuePromptDialogFragment;
            SetupValueFragmentObservers(FolderPromptDialogFragment);
            UrlPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(URL_PROMPT_TAG) as ValuePromptDialogFragment;
            SetupValueFragmentObservers(UrlPromptDialogFragment);
            MaxDaysOldPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(MAX_DAYS_OLD_PROMPT_CONFIG_TAG) as DefaultableItemValuePromptDialogFragment;
            SetupDefaultableItemValueFragmentObservers(MaxDaysOldPromptDialogFragment);
            DeleteDaysOldPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(DELETE_DOWNLOAD_DAYS_OLD_PROMPT_CONFIG_TAG) as DefaultableItemValuePromptDialogFragment;
            SetupDefaultableItemValueFragmentObservers(DeleteDaysOldPromptDialogFragment);
            MaxDownloadItemsPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(MAX_DOWNLOAD_ITEMS_PROMPT_CONFIG_TAG) as DefaultableItemValuePromptDialogFragment;
            SetupDefaultableItemValueFragmentObservers(MaxDownloadItemsPromptDialogFragment);
            DeletePodcastPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(DELETE_PROMPT_TAG) as OkCancelDialogFragment;
            SetupFragmentObservers(DeletePodcastPromptDialogFragment);

            AndroidApplication.Logger.Debug(() => $"EditFeedActivity:OnCreate - end");
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"EditFeedActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
            KillValueFragmentObservers(FolderPromptDialogFragment);
            KillValueFragmentObservers(UrlPromptDialogFragment);
            KillDefaultableItemValueFragmentObservers(MaxDaysOldPromptDialogFragment);
            KillDefaultableItemValueFragmentObservers(DeleteDaysOldPromptDialogFragment);
            KillDefaultableItemValueFragmentObservers(MaxDownloadItemsPromptDialogFragment);
            KillFragmentObservers(DeletePodcastPromptDialogFragment);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"EditFeedActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (BackKeyMapper.HandleKeyEvent(this, e))
            {
                AndroidApplication.Logger.Debug(() => $"EditFeedActivity:DispatchKeyEvent - handled");
                return true;
            }
            return base.DispatchKeyEvent(e);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_edit_feed, menu);
            // we want a separator on the menu
            MenuCompat.SetGroupDividerEnabled(menu, true);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            EnableMenuItemIfAvailable(menu, Resource.Id.action_test_feed);
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

        private void DoUrlOptions()
        {
            ViewModel.UrlOptions();
        }

        private void DoFolderOptions()
        {
            ViewModel.FolderOptions();
        }

        private void DoDownloadStrategyOptions()
        {
            AndroidApplication.Logger.Debug(() => $"EditFeedActivity:DoDownloadStrategyOptions");
            List<SelectableString> options = ViewModel.GetDownloadStrategyOptions();
            var sheet = SelectableStringListBottomSheetFragment.NewInstance(
                true,
                GetString(Resource.String.download_strategy_sheet_title),
                options);
            sheet.Show(SupportFragmentManager, BOTTOMSHEET_DOWNLOAD_STRATEGY_CONFIG_TAG);
        }

        private void DoNamingStyleOptions()
        {
            AndroidApplication.Logger.Debug(() => $"EditFeedActivity:DoNamingStyleOptions");
            List<SelectableString> options = ViewModel.GetNamingStyleOptions();
            var sheet = SelectableStringListBottomSheetFragment.NewInstance(
                true,
                GetString(Resource.String.naming_style_sheet_title),
                options);
            sheet.Show(SupportFragmentManager, BOTTOMSHEET_NAMING_STYLE_CONFIG_TAG);
        }

        public void BottomsheetItemSelected(string tag, int position, SelectableString item)
        {
            AndroidApplication.Logger.Debug(() => $"EditFeedActivity:BottomsheetItemSelected {tag}, {position}");
            switch (tag)
            {
                case BOTTOMSHEET_DOWNLOAD_STRATEGY_CONFIG_TAG:
                    ViewModel.DoDownloadStrategyOption(item);
                    break;
                case BOTTOMSHEET_NAMING_STYLE_CONFIG_TAG:
                    ViewModel.DoNamingStyleOption(item);
                    break;
            }
        }

        private void DoMaxDaysOldOptions()
        {
            ViewModel.MaxDaysOldOptions();
        }

        private void DoMaxDownloadItemsOptions()
        {
            ViewModel.MaxDownloadItemsOptions();
        }

        private void DoDeleteDownloadDaysOldOptions()
        {
            ViewModel.DeleteDownloadDaysOldOptions();
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.NavigateToDownload += NavigateToDownload;
            ViewModel.Observables.Folder += Folder;
            ViewModel.Observables.PromptForFolder += PromptForFolder;
            ViewModel.Observables.Url += Url;
            ViewModel.Observables.PromptForUrl += PromptForUrl;
            ViewModel.Observables.DisplayMessage += DisplayMessage;
            ViewModel.Observables.DownloadStrategy += DownloadStrategy;
            ViewModel.Observables.NamingStyle += NamingStyle;
            ViewModel.Observables.MaxDaysOld += MaxDaysOld;
            ViewModel.Observables.PromptForMaxDaysOld += PromptForMaxDaysOld;
            ViewModel.Observables.DeleteDaysOld += DeleteDaysOld;
            ViewModel.Observables.PromptForDeleteDaysOld += PromptForDeleteDaysOld;
            ViewModel.Observables.MaxDownloadItems += MaxDownloadItems;
            ViewModel.Observables.PromptForMaxDownloadItems += PromptForMaxDownloadItems;
            ViewModel.Observables.DeletePrompt += DeletePrompt;
            ViewModel.Observables.Exit += Exit;
            ViewModel.Observables.DisplayChooser += DisplayChooser;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.Title -= SetTitle;
            ViewModel.Observables.NavigateToDownload -= NavigateToDownload;
            ViewModel.Observables.Folder -= Folder;
            ViewModel.Observables.PromptForFolder -= PromptForFolder;
            ViewModel.Observables.Url -= Url;
            ViewModel.Observables.PromptForUrl -= PromptForUrl;
            ViewModel.Observables.DisplayMessage -= DisplayMessage;
            ViewModel.Observables.DownloadStrategy -= DownloadStrategy;
            ViewModel.Observables.NamingStyle -= NamingStyle;
            ViewModel.Observables.MaxDaysOld -= MaxDaysOld;
            ViewModel.Observables.PromptForMaxDaysOld -= PromptForMaxDaysOld;
            ViewModel.Observables.DeleteDaysOld -= DeleteDaysOld;
            ViewModel.Observables.PromptForDeleteDaysOld -= PromptForDeleteDaysOld;
            ViewModel.Observables.MaxDownloadItems -= MaxDownloadItems;
            ViewModel.Observables.PromptForMaxDownloadItems -= PromptForMaxDownloadItems;
            ViewModel.Observables.DeletePrompt -= DeletePrompt;
            ViewModel.Observables.Exit -= Exit;
            ViewModel.Observables.DisplayChooser -= DisplayChooser;
        }

        private void DisplayChooser(object sender, Tuple<string, Intent> args)
        {
            AndroidApplication.Logger.Debug(() => $"EditFeedActivity: DisplayChooser");
            (string title, Intent intent) = args;
            StartActivity(Intent.CreateChooser(intent, title));
        }

        private void Exit(object sender, EventArgs e)
        {
            AndroidApplication.Logger.Debug(() => $"EditFeedActivity: Exit");
            RunOnUiThread(() =>
            {
                Finish();
            });
        }

        private void NavigateToDownload(object sender, string folder)
        {
            AndroidApplication.Logger.Debug(() => $"EditFeedActivity: NavigateToDownload {folder}");
            RunOnUiThread(() =>
            {
                var intent = DownloadActivity.CreateIntentToTestFeed(this, folder);
                StartActivity(intent);
            });
        }

        private void PromptForUrl(object sender, ValuePromptDialogFragment.ValuePromptDialogFragmentParameters parameters)
        {
            RunOnUiThread(() =>
            {
                UrlPromptDialogFragment = ValuePromptDialogFragment.NewInstance(parameters);
                SetupValueFragmentObservers(UrlPromptDialogFragment);
                UrlPromptDialogFragment.Show(SupportFragmentManager, URL_PROMPT_TAG);
            });
        }

        private void PromptForFolder(object sender, ValuePromptDialogFragment.ValuePromptDialogFragmentParameters parameters)
        {
            RunOnUiThread(() =>
            {
                FolderPromptDialogFragment = ValuePromptDialogFragment.NewInstance(parameters);
                SetupValueFragmentObservers(FolderPromptDialogFragment);
                FolderPromptDialogFragment.Show(SupportFragmentManager, FOLDER_PROMPT_TAG);
            });
        }

        private void SetTitle(object sender, string title)
        {
            RunOnUiThread(() =>
            {
                Title = title;
            });
        }

        private void Folder(object sender, string str)
        {
            RunOnUiThread(() =>
            {
                FolderRowSubLabel.Text = str;
            });
        }

        private void Url(object sender, string str)
        {
            RunOnUiThread(() =>
            {
                UrlRowSubLabel.Text = str;
            });
        }

        private void DisplayMessage(object sender, string message)
        {
            AndroidApplication.Logger.Debug(() => $"EditFeedActivity: DisplayMessage {message}");
            RunOnUiThread(() =>
            {
                Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
            });
        }

        private void SetTextViewToShowOverride(TextView textView, bool isSet)
        {
            // this may not be slick enough but lets see
            if (isSet)
            {
                textView.SetTypeface(textView.Typeface, TypefaceStyle.Bold);
            }
            else
            {
                //textView.SetTypeface(textView.Typeface, TypefaceStyle.Normal);
                textView.SetTypeface(Typeface.Create(textView.Typeface, TypefaceStyle.Normal), TypefaceStyle.Normal);
            }
        }

        private void MaxDownloadItems(object sender, Tuple<bool, string> parameters)
        {
            (bool isSet, string str) = parameters;
            RunOnUiThread(() =>
            {
                MaxDownloadItemsRowSubLabel.Text = str;
                SetTextViewToShowOverride(MaxDownloadItemsRowSubLabel, isSet);
            });
        }

        private void NamingStyle(object sender, Tuple<bool, string> parameters)
        {
            (bool isSet, string str) = parameters;
            RunOnUiThread(() =>
            {
                NamingStyleRowSubLabel.Text = str;
                SetTextViewToShowOverride(NamingStyleRowSubLabel, isSet);
            });
        }

        private void DownloadStrategy(object sender, Tuple<bool, string> parameters)
        {
            (bool isSet, string str) = parameters;
            AndroidApplication.Logger.Debug(() => $"EditFeedActivity:DownloadStrategy {isSet} {str}");
            RunOnUiThread(() =>
            {
                DownloadStrategyRowSubLabel.Text = str;
                SetTextViewToShowOverride(DownloadStrategyRowSubLabel, isSet);
            });
        }

        private void MaxDaysOld(object sender, Tuple<bool, string> parameters)
        {
            (bool isSet, string str) = parameters;
            AndroidApplication.Logger.Debug(() => $"EditFeedActivity:MaxDaysOld {isSet} {str}");
            RunOnUiThread(() =>
            {
                MaxDaysOldRowSubLabel.Text = str;
                SetTextViewToShowOverride(MaxDaysOldRowSubLabel, isSet);
            });
        }
        private void DeleteDaysOld(object sender, Tuple<bool, string> parameters)
        {
            (bool isSet, string str) = parameters;
            RunOnUiThread(() =>
            {
                DeleteDaysOldRowSubLabel.Text = str;
                SetTextViewToShowOverride(DeleteDaysOldRowSubLabel, isSet);
            });
        }

        private void PromptForDeleteDaysOld(object sender, DefaultableItemValuePromptDialogFragmentParameters parameters)
        {
            RunOnUiThread(() =>
            {
                DeleteDaysOldPromptDialogFragment = DefaultableItemValuePromptDialogFragment.NewInstance(parameters);
                SetupDefaultableItemValueFragmentObservers(DeleteDaysOldPromptDialogFragment);
                DeleteDaysOldPromptDialogFragment.Show(SupportFragmentManager, DELETE_DOWNLOAD_DAYS_OLD_PROMPT_CONFIG_TAG);
            });
        }

        private void PromptForMaxDaysOld(object sender, DefaultableItemValuePromptDialogFragmentParameters parameters)
        {
            RunOnUiThread(() =>
            {
                MaxDaysOldPromptDialogFragment = DefaultableItemValuePromptDialogFragment.NewInstance(parameters);
                SetupDefaultableItemValueFragmentObservers(MaxDaysOldPromptDialogFragment);
                MaxDaysOldPromptDialogFragment.Show(SupportFragmentManager, MAX_DAYS_OLD_PROMPT_CONFIG_TAG);
            });
        }

        private void PromptForMaxDownloadItems(object sender, DefaultableItemValuePromptDialogFragmentParameters parameters)
        {
            RunOnUiThread(() =>
            {
                MaxDownloadItemsPromptDialogFragment = DefaultableItemValuePromptDialogFragment.NewInstance(parameters);
                SetupDefaultableItemValueFragmentObservers(MaxDownloadItemsPromptDialogFragment);
                MaxDownloadItemsPromptDialogFragment.Show(SupportFragmentManager, MAX_DOWNLOAD_ITEMS_PROMPT_CONFIG_TAG);
            });
        }

        private void DeletePrompt(object sender, Tuple<string, string, string, string> parameters)
        {
            RunOnUiThread(() =>
            {
                (string title, string message, string ok, string cancel) = parameters;
                DeletePodcastPromptDialogFragment = OkCancelDialogFragment.NewInstance(title, message, ok, cancel, null);
                SetupFragmentObservers(DeletePodcastPromptDialogFragment);
                DeletePodcastPromptDialogFragment.Show(SupportFragmentManager, DELETE_PROMPT_TAG);
            });
        }

        private void SetupFragmentObservers(OkCancelDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"EditFeedActivity: SetupFragmentObservers - {fragment.Tag}");
                fragment.OkSelected += OkSelected;
                fragment.CancelSelected += CancelSelected;
            }
        }

        private void KillFragmentObservers(OkCancelDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"EditFeedActivity: KillFragmentObservers - {fragment.Tag}");
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
                case DELETE_PROMPT_TAG:
                    KillFragmentObservers(DeletePodcastPromptDialogFragment);
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
                    case DELETE_PROMPT_TAG:
                        KillFragmentObservers(DeletePodcastPromptDialogFragment);
                        ViewModel.DeleteConfirmed();
                        break;
                }
            });
        }

        private void SetupValueFragmentObservers(ValuePromptDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"EditFeedActivity: SetupFragmentObservers - {fragment.Tag}");
                fragment.OkSelected += OkSelected;
                fragment.CancelSelected += CancelSelected;
            }
        }

        private void KillValueFragmentObservers(ValuePromptDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"EditFeedActivity: KillFragmentObservers - {fragment.Tag}");
                fragment.OkSelected -= OkSelected;
                fragment.CancelSelected -= CancelSelected;
            }
        }

        private void OkSelected(object sender, Tuple<string, string, string> parameters)
        {
            (string tag, string data, string value) = parameters;
            AndroidApplication.Logger.Debug(() => $"OkSelected: {tag}");
            switch (tag)
            {
                case FOLDER_PROMPT_TAG:
                    KillValueFragmentObservers(FolderPromptDialogFragment);
                    ViewModel.SetFolder(value);
                    break;
                case URL_PROMPT_TAG:
                    KillValueFragmentObservers(UrlPromptDialogFragment);
                    ViewModel.SetUrl(value);
                    break;
            }
        }

        private void CancelSelected(object sender, Tuple<string, string, string> parameters)
        {
            (string tag, string data, string value) = parameters;
            AndroidApplication.Logger.Debug(() => $"CancelSelected: {tag}");
            switch (tag)
            {
                case FOLDER_PROMPT_TAG:
                    KillValueFragmentObservers(FolderPromptDialogFragment);
                    break;
                case URL_PROMPT_TAG:
                    KillValueFragmentObservers(UrlPromptDialogFragment);
                    break;
            }
        }

        private void SetupDefaultableItemValueFragmentObservers(DefaultableItemValuePromptDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"EditFeedActivity: SetupFragmentObservers - {fragment.Tag}");
                fragment.OkSelected += DefaultableItemValueOkSelected;
                fragment.CancelSelected += DefaultableItemValueCancelSelected;
            }
        }

        private void KillDefaultableItemValueFragmentObservers(DefaultableItemValuePromptDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"EditFeedActivity: KillFragmentObservers - {fragment.Tag}");
                fragment.OkSelected -= DefaultableItemValueOkSelected;
                fragment.CancelSelected -= DefaultableItemValueCancelSelected;
            }
        }

        private void DefaultableItemValueOkSelected(object sender, Tuple<string, string, string, ItemValueType> parameters)
        {
            (string tag, string data, string value, ItemValueType valueType) = parameters;
            AndroidApplication.Logger.Debug(() => $"OkSelected: {tag}");
            switch (tag)
            {
                case MAX_DAYS_OLD_PROMPT_CONFIG_TAG:
                    KillDefaultableItemValueFragmentObservers(MaxDaysOldPromptDialogFragment);
                    ViewModel.SetMaxDaysOld(value, valueType);
                    break;
                case DELETE_DOWNLOAD_DAYS_OLD_PROMPT_CONFIG_TAG:
                    KillDefaultableItemValueFragmentObservers(DeleteDaysOldPromptDialogFragment);
                    ViewModel.SetDeleteDaysOld(value, valueType);
                    break;
                case MAX_DOWNLOAD_ITEMS_PROMPT_CONFIG_TAG:
                    KillDefaultableItemValueFragmentObservers(MaxDownloadItemsPromptDialogFragment);
                    ViewModel.SetMaxDownloadItems(value, valueType);
                    break;
            }
        }

        private void DefaultableItemValueCancelSelected(object sender, Tuple<string, string> parameters)
        {
            (string tag, string data) = parameters;
            AndroidApplication.Logger.Debug(() => $"CancelSelected: {tag}");
            switch (tag)
            {
                case MAX_DAYS_OLD_PROMPT_CONFIG_TAG:
                    KillDefaultableItemValueFragmentObservers(MaxDaysOldPromptDialogFragment);
                    break;
                case DELETE_DOWNLOAD_DAYS_OLD_PROMPT_CONFIG_TAG:
                    KillDefaultableItemValueFragmentObservers(DeleteDaysOldPromptDialogFragment);
                    break;
                case MAX_DOWNLOAD_ITEMS_PROMPT_CONFIG_TAG:
                    KillDefaultableItemValueFragmentObservers(MaxDownloadItemsPromptDialogFragment);
                    break;
            }
        }

    }
}
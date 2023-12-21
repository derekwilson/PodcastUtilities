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

namespace PodcastUtilities.UI.Configure
{
    [Activity(Label = "@string/edit_feed_activity_title", ParentActivity = typeof(EditConfigActivity))]
    internal class EditFeedActivity : AppCompatActivity, SelectableStringListBottomSheetFragment.IListener
    {
        private const string ACTIVITY_PARAM_ID_CONFIG = "EditFeedActivity:Param:Id";
        private const string ACTIVITY_PARAM_FOLDER_CONFIG = "EditFeedActivity:Param:Folder";

        public static Intent CreateIntent(Context context, string id, string folder)
        {
            Intent intent = new Intent(context, typeof(EditFeedActivity));
            if (!string.IsNullOrEmpty(id))
            {
                intent.PutExtra(ACTIVITY_PARAM_ID_CONFIG, id);
            }
            if (!string.IsNullOrEmpty(folder))
            {
                intent.PutExtra(ACTIVITY_PARAM_FOLDER_CONFIG, folder);
            }
            return intent;
        }

        private const string BOTTOMSHEET_DOWNLOAD_STRATEGY_CONFIG_TAG = "config_download_strategy_tag";
        private const string BOTTOMSHEET_NAMING_STYLE_CONFIG_TAG = "config_naming_style_tag";

        private const string MAX_DAYS_OLD_PROMPT_CONFIG_TAG = "config_max_days_old_prompt_tag";
        private const string DELETE_DOWNLOAD_DAYS_OLD_PROMPT_CONFIG_TAG = "config_delete_download_prompt_tag";
        private const string MAX_DOWNLOAD_ITEMS_PROMPT_CONFIG_TAG = "config_max_download_items_prompt_tag";

        private AndroidApplication AndroidApplication;
        private EditFeedViewModel ViewModel;

        private NestedScrollView Container = null;
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

        private DefaultableItemValuePromptDialogFragment MaxDaysOldPromptDialogFragment;
        private DefaultableItemValuePromptDialogFragment DeleteDaysOldPromptDialogFragment;
        private DefaultableItemValuePromptDialogFragment MaxDownloadItemsPromptDialogFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"EditFeedActivity:OnCreate");

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_edit_feed);

            var id = Intent?.GetStringExtra(ACTIVITY_PARAM_ID_CONFIG);
            var folder = Intent?.GetStringExtra(ACTIVITY_PARAM_FOLDER_CONFIG);
            if (folder == null || id == null)
            {
                AndroidApplication.Logger.Debug(() => $"EditFeedActivity:OnCreate - no id/folder specified");
            }

            Container = FindViewById<NestedScrollView>(Resource.Id.edit_feed_container);
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

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(EditFeedViewModel))) as EditFeedViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise(id, folder);

            DownloadStrategyRowContainer.Click += (sender, e) => DoDownloadStrategyOptions();
            NamingStyleRowContainer.Click += (sender, e) => DoNamingStyleOptions();
            MaxDaysOldRowContainer.Click += (sender, e) => DoMaxDaysOldOptions();
            DeleteDaysOldRowContainer.Click += (sender, e) => DoDeleteDownloadDaysOldOptions();
            MaxDownloadItemsRowContainer.Click += (sender, e) => DoMaxDownloadItemsOptions();

            MaxDaysOldPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(MAX_DAYS_OLD_PROMPT_CONFIG_TAG) as DefaultableItemValuePromptDialogFragment;
            SetupDefaultableItemValueFragmentObservers(MaxDaysOldPromptDialogFragment);
            DeleteDaysOldPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(DELETE_DOWNLOAD_DAYS_OLD_PROMPT_CONFIG_TAG) as DefaultableItemValuePromptDialogFragment;
            SetupDefaultableItemValueFragmentObservers(DeleteDaysOldPromptDialogFragment);
            MaxDownloadItemsPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(MAX_DOWNLOAD_ITEMS_PROMPT_CONFIG_TAG) as DefaultableItemValuePromptDialogFragment;
            SetupDefaultableItemValueFragmentObservers(MaxDownloadItemsPromptDialogFragment);

            AndroidApplication.Logger.Debug(() => $"EditFeedActivity:OnCreate - end");
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"EditFeedActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
            KillDefaultableItemValueFragmentObservers(MaxDaysOldPromptDialogFragment);
            KillDefaultableItemValueFragmentObservers(DeleteDaysOldPromptDialogFragment);
            KillDefaultableItemValueFragmentObservers(MaxDownloadItemsPromptDialogFragment);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"EditFeedActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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
            DisplayMessage(this, "Not implemented");
        }

        private void DoDeleteDownloadDaysOldOptions()
        {
            DisplayMessage(this, "Not implemented");
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.DisplayMessage += DisplayMessage;
            ViewModel.Observables.DownloadStrategy += DownloadStrategy;
            ViewModel.Observables.NamingStyle += NamingStyle;
            ViewModel.Observables.MaxDaysOld += MaxDaysOld;
            ViewModel.Observables.PromptForMaxDaysOld += PromptForMaxDaysOld;
            ViewModel.Observables.DeleteDaysOld += DeleteDaysOld;
            ViewModel.Observables.PromptForDeleteDaysOld += PromptForDeleteDaysOld;
            ViewModel.Observables.MaxDownloadItems += MaxDownloadItems;
            ViewModel.Observables.PromptForMaxDownloadItems += PromptForMaxDownloadItems;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.Title -= SetTitle;
            ViewModel.Observables.DisplayMessage -= DisplayMessage;
            ViewModel.Observables.DownloadStrategy -= DownloadStrategy;
            ViewModel.Observables.NamingStyle -= NamingStyle;
            ViewModel.Observables.MaxDaysOld -= MaxDaysOld;
            ViewModel.Observables.PromptForMaxDaysOld -= PromptForMaxDaysOld;
            ViewModel.Observables.DeleteDaysOld -= DeleteDaysOld;
            ViewModel.Observables.PromptForDeleteDaysOld -= PromptForDeleteDaysOld;
            ViewModel.Observables.MaxDownloadItems -= MaxDownloadItems;
            ViewModel.Observables.PromptForMaxDownloadItems -= PromptForMaxDownloadItems;
        }

        private void SetTitle(object sender, string title)
        {
            RunOnUiThread(() =>
            {
                Title = title;
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
                textView.SetTypeface(textView.Typeface, TypefaceStyle.Normal);
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
                    //ViewModel.SetDeleteDaysOld(value, valueType);
                    break;
                case MAX_DOWNLOAD_ITEMS_PROMPT_CONFIG_TAG:
                    KillDefaultableItemValueFragmentObservers(MaxDownloadItemsPromptDialogFragment);
                    //ViewModel.SetMaxDownloadItems(value, valueType);
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
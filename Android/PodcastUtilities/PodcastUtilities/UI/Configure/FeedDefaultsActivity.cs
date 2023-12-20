using Android.App;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Widget;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Configure;
using System;
using System.Collections.Generic;
using static Android.Hardware.Camera;
using static PodcastUtilities.AndroidLogic.CustomViews.DefaultableItemValuePromptDialogFragment;

namespace PodcastUtilities.UI.Configure
{
    [Activity(Label = "@string/feed_defaults_activity_title", ParentActivity = typeof(EditConfigActivity))]
    internal class FeedDefaultsActivity : AppCompatActivity, SelectableStringListBottomSheetFragment.IListener
    {
        private const string BOTTOMSHEET_DOWNLOAD_STRATEGY_TAG = "download_strategy_tag";
        private const string BOTTOMSHEET_NAMING_STYLE_TAG = "naming_style_tag";

        private const string MAX_DAYS_OLD_PROMPT_TAG = "max_days_old_prompt_tag";
        private const string DELETE_DOWNLOAD_DAYS_OLD_PROMPT_TAG = "delete_download_prompt_tag";
        private const string MAX_DOWNLOAD_ITEMS_PROMPT_TAG = "max_download_items_prompt_tag";

        private AndroidApplication AndroidApplication;
        private FeedDefaultsViewModel ViewModel;

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
            AndroidApplication.Logger.Debug(() => $"FeedDefaultsActivity:OnCreate");

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_feed_defaults);

            Container = FindViewById<NestedScrollView>(Resource.Id.feed_defaults_container);
            DownloadStrategyRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.download_strategy_row_label_container);
            DownloadStrategyRowSubLabel = FindViewById<TextView>(Resource.Id.download_strategy_row_sub_label);
            NamingStyleRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.naming_style_row_label_container);
            NamingStyleRowSubLabel = FindViewById<TextView>(Resource.Id.naming_style_row_sub_label);
            MaxDaysOldRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.max_days_old_row_label_container);
            MaxDaysOldRowSubLabel = FindViewById<TextView>(Resource.Id.max_days_old_row_sub_label);
            DeleteDaysOldRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.delete_download_days_old_row_label_container);
            DeleteDaysOldRowSubLabel = FindViewById<TextView>(Resource.Id.delete_download_days_old_row_sub_label);
            MaxDownloadItemsRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.max_download_items_row_label_container);
            MaxDownloadItemsRowSubLabel = FindViewById<TextView>(Resource.Id.max_download_items_row_sub_label);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(FeedDefaultsViewModel))) as FeedDefaultsViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();

            DownloadStrategyRowContainer.Click += (sender, e) => DoDownloadStrategyOptions();
            NamingStyleRowContainer.Click += (sender, e) => DoNamingStyleOptions();
            MaxDaysOldRowContainer.Click += (sender, e) => DoMaxDaysOldOptions();
            DeleteDaysOldRowContainer.Click += (sender, e) => DoDeleteDownloadDaysOldOptions();
            MaxDownloadItemsRowContainer.Click += (sender, e) => DoMaxMaxDownloadItemsOptions();

            MaxDaysOldPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(MAX_DAYS_OLD_PROMPT_TAG) as DefaultableItemValuePromptDialogFragment;
            SetupDefaultableItemValueFragmentObservers(MaxDaysOldPromptDialogFragment);
            DeleteDaysOldPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(DELETE_DOWNLOAD_DAYS_OLD_PROMPT_TAG) as DefaultableItemValuePromptDialogFragment;
            SetupDefaultableItemValueFragmentObservers(DeleteDaysOldPromptDialogFragment);
            MaxDownloadItemsPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(MAX_DOWNLOAD_ITEMS_PROMPT_TAG) as DefaultableItemValuePromptDialogFragment;
            SetupDefaultableItemValueFragmentObservers(MaxDownloadItemsPromptDialogFragment);

            AndroidApplication.Logger.Debug(() => $"FeedDefaultsActivity:OnCreate - end");
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"FeedDefaultsActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
            KillDefaultableItemValueFragmentObservers(MaxDaysOldPromptDialogFragment);
            KillDefaultableItemValueFragmentObservers(DeleteDaysOldPromptDialogFragment);
            KillDefaultableItemValueFragmentObservers(MaxDownloadItemsPromptDialogFragment);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"FeedDefaultsActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void DoDownloadStrategyOptions()
        {
            AndroidApplication.Logger.Debug(() => $"FeedDefaultsActivity:DoDownloadStrategyOptions");
            List<SelectableString> options = ViewModel.GetDownloadStrategyOptions();
            var sheet = SelectableStringListBottomSheetFragment.NewInstance(
                true,
                GetString(Resource.String.download_strategy_sheet_title),
                options);
            sheet.Show(SupportFragmentManager, BOTTOMSHEET_DOWNLOAD_STRATEGY_TAG);
        }

        private void DoNamingStyleOptions()
        {
            AndroidApplication.Logger.Debug(() => $"FeedDefaultsActivity:DoNamingStyleOptions");
            List<SelectableString> options = ViewModel.GetNamingStyleOptions();
            var sheet = SelectableStringListBottomSheetFragment.NewInstance(
                true,
                GetString(Resource.String.naming_style_sheet_title),
                options);
            sheet.Show(SupportFragmentManager, BOTTOMSHEET_NAMING_STYLE_TAG);
        }

        public void BottomsheetItemSelected(string tag, int position, SelectableString item)
        {
            AndroidApplication.Logger.Debug(() => $"FeedDefaultsActivity:BottomsheetItemSelected {tag}, {position}");
            switch (tag)
            {
                case BOTTOMSHEET_DOWNLOAD_STRATEGY_TAG:
                    ViewModel.DoDownloadStrategyOption(item);
                    break;
                case BOTTOMSHEET_NAMING_STYLE_TAG:
                    ViewModel.DoNamingStyleOption(item);
                    break;
            }
        }

        private void DoMaxDaysOldOptions()
        {
            ViewModel.MaxDaysOldOptions();
        }

        private void DoMaxMaxDownloadItemsOptions()
        {
            ViewModel.MaxMaxDownloadItemsOptions();
        }

        private void DoDeleteDownloadDaysOldOptions()
        {
            ViewModel.DeleteDownloadDaysOldOptions();
        }

        private void SetupViewModelObservers()
        {
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

        private void MaxDownloadItems(object sender, string str)
        {
            RunOnUiThread(() =>
            {
                MaxDownloadItemsRowSubLabel.Text = str;
            });
        }

        private void PromptForMaxDownloadItems(object sender, DefaultableItemValuePromptDialogFragmentParameters parameters)
        {
            RunOnUiThread(() =>
            {
                MaxDownloadItemsPromptDialogFragment = DefaultableItemValuePromptDialogFragment.NewInstance(parameters);
                SetupDefaultableItemValueFragmentObservers(MaxDownloadItemsPromptDialogFragment);
                MaxDownloadItemsPromptDialogFragment.Show(SupportFragmentManager, MAX_DOWNLOAD_ITEMS_PROMPT_TAG);
            });
        }

        private void DeleteDaysOld(object sender, string str)
        {
            RunOnUiThread(() =>
            {
                DeleteDaysOldRowSubLabel.Text = str;
            });
        }

        private void PromptForDeleteDaysOld(object sender, DefaultableItemValuePromptDialogFragmentParameters parameters)
        {
            RunOnUiThread(() =>
            {
                DeleteDaysOldPromptDialogFragment = DefaultableItemValuePromptDialogFragment.NewInstance(parameters);
                SetupDefaultableItemValueFragmentObservers(DeleteDaysOldPromptDialogFragment);
                DeleteDaysOldPromptDialogFragment.Show(SupportFragmentManager, DELETE_DOWNLOAD_DAYS_OLD_PROMPT_TAG);
            });
        }

        private void PromptForMaxDaysOld(object sender, DefaultableItemValuePromptDialogFragmentParameters parameters)
        {
            RunOnUiThread(() =>
            {
                MaxDaysOldPromptDialogFragment = DefaultableItemValuePromptDialogFragment.NewInstance(parameters);
                SetupDefaultableItemValueFragmentObservers(MaxDaysOldPromptDialogFragment);
                MaxDaysOldPromptDialogFragment.Show(SupportFragmentManager, MAX_DAYS_OLD_PROMPT_TAG);
            });
        }

        private void NamingStyle(object sender, string str)
        {
            RunOnUiThread(() =>
            {
                NamingStyleRowSubLabel.Text = str;
            });
        }

        private void DownloadStrategy(object sender, string str)
        {
            RunOnUiThread(() =>
            {
                DownloadStrategyRowSubLabel.Text = str;
            });
        }

        private void MaxDaysOld(object sender, string str)
        {
            RunOnUiThread(() =>
            {
                MaxDaysOldRowSubLabel.Text = str;
            });
        }

        private void DisplayMessage(object sender, string message)
        {
            AndroidApplication.Logger.Debug(() => $"FeedDefaultsActivity: DisplayMessage {message}");
            RunOnUiThread(() =>
            {
                Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
            });
        }

        private void SetupDefaultableItemValueFragmentObservers(DefaultableItemValuePromptDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"FeedDefaultsActivity: SetupFragmentObservers - {fragment.Tag}");
                fragment.OkSelected += DefaultableItemValueOkSelected;
                fragment.CancelSelected += DefaultableItemValueCancelSelected;
            }
        }

        private void KillDefaultableItemValueFragmentObservers(DefaultableItemValuePromptDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"FeedDefaultsActivity: KillFragmentObservers - {fragment.Tag}");
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
                case MAX_DAYS_OLD_PROMPT_TAG:
                    KillDefaultableItemValueFragmentObservers(MaxDaysOldPromptDialogFragment);
                    ViewModel.SetMaxDaysOld(value, valueType);
                    break;
                case DELETE_DOWNLOAD_DAYS_OLD_PROMPT_TAG:
                    KillDefaultableItemValueFragmentObservers(DeleteDaysOldPromptDialogFragment);
                    ViewModel.SetDeleteDaysOld(value, valueType);
                    break;
                case MAX_DOWNLOAD_ITEMS_PROMPT_TAG:
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
                case MAX_DAYS_OLD_PROMPT_TAG:
                    KillDefaultableItemValueFragmentObservers(MaxDaysOldPromptDialogFragment);
                    break;
                case DELETE_DOWNLOAD_DAYS_OLD_PROMPT_TAG:
                    KillDefaultableItemValueFragmentObservers(DeleteDaysOldPromptDialogFragment);
                    break;
                case MAX_DOWNLOAD_ITEMS_PROMPT_TAG:
                    KillDefaultableItemValueFragmentObservers(MaxDownloadItemsPromptDialogFragment);
                    break;
            }
        }
    }
}
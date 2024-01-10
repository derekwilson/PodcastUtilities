using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Widget;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Configure;
using System;
using System.Collections.Generic;
using static PodcastUtilities.AndroidLogic.CustomViews.DefaultableItemValuePromptDialogFragment;

namespace PodcastUtilities.UI.Configure
{
    [Activity(Label = "@string/global_values_activity_title", ParentActivity = typeof(EditConfigActivity))]
    internal class GlobalValuesActivity : AppCompatActivity, SelectableStringListBottomSheetFragment.IListener
    {
        private const string BOTTOMSHEET_PLAYLIST_FORMAT_TAG = "playlist_format_tag";
        private const string BOTTOMSHEET_DIAG_OUTPUT_TAG = "diag_output_tag";
        private const string FREESPACE_PROMPT_TAG = "freespace_prompt_tag";
        private const string PLAYLIST_FILENAME_PROMPT_TAG = "playlist_filename_prompt_tag";
        private const string PLAYLIST_SEPERATOR_PROMPT_TAG = "playlist_seperator_prompt_tag";

        private AndroidApplication AndroidApplication;
        private GlobalValuesViewModel ViewModel;

        private NestedScrollView Container = null;
        private LinearLayoutCompat DownloadFreeSpaceRowContainer = null;
        private TextView DownloadFreeSpaceRowSubLabel = null;
        private LinearLayoutCompat PlaylistFileRowContainer = null;
        private TextView PlaylistFileRowSubLabel = null;
        private LinearLayoutCompat PlaylistSeperatorRowContainer = null;
        private TextView PlaylistSeperatorRowSubLabel = null;
        private LinearLayoutCompat PlaylistFormatRowContainer = null;
        private TextView PlaylistFormatRowSubLabel = null;
        private LinearLayoutCompat DiagOutputRowContainer = null;
        private TextView DiagOutputRowSubLabel = null;
        private ConstraintLayout DiagRetainTempRowContainer = null;
        private TextView DiagRetainTempRowSubLabel = null;
        private AppCompatCheckBox DiagRetainTempRowCheck = null;

        private ValuePromptDialogFragment PlaylistFilenamePromptDialogFragment;
        private ValuePromptDialogFragment PlaylistSeperatorPromptDialogFragment;
        private DefaultableItemValuePromptDialogFragment FreespacePromptDialogFragment;

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
            PlaylistSeperatorRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.playlist_seperator_row_label_container);
            PlaylistSeperatorRowSubLabel = FindViewById<TextView>(Resource.Id.playlist_seperator_row_sub_label);
            PlaylistFormatRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.playlist_format_row_label_container);
            PlaylistFormatRowSubLabel = FindViewById<TextView>(Resource.Id.playlist_format_row_sub_label);
            DiagOutputRowContainer = FindViewById<LinearLayoutCompat>(Resource.Id.diag_output_row_label_container);
            DiagOutputRowSubLabel = FindViewById<TextView>(Resource.Id.diag_output_row_sub_label);
            DiagRetainTempRowContainer = FindViewById<ConstraintLayout>(Resource.Id.diag_retain_temp_row_container);
            DiagRetainTempRowSubLabel = FindViewById<TextView>(Resource.Id.diag_retain_temp_row_sub_label);
            DiagRetainTempRowCheck = FindViewById<AppCompatCheckBox>(Resource.Id.diag_retain_temp_row_check);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(GlobalValuesViewModel))) as GlobalValuesViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();

            DownloadFreeSpaceRowContainer.Click += (sender, e) => DoDownloadFreeSpaceOptions();
            PlaylistFileRowContainer.Click += (sender, e) => DoPlaylistFileOptions();
            PlaylistSeperatorRowContainer.Click += (sender, e) => DoPlaylistSeperatorOptions();
            PlaylistFormatRowContainer.Click += (sender, e) => DoPlaylistFormatOptions();
            DiagOutputRowContainer.Click += (sender, e) => DoDiagOutputOptions();
            DiagRetainTempRowContainer.Click += (sender, e) => DoDiagRetainTempOptions();

            PlaylistFilenamePromptDialogFragment = SupportFragmentManager.FindFragmentByTag(PLAYLIST_FILENAME_PROMPT_TAG) as ValuePromptDialogFragment;
            SetupValueFragmentObservers(PlaylistFilenamePromptDialogFragment);
            PlaylistSeperatorPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(PLAYLIST_SEPERATOR_PROMPT_TAG) as ValuePromptDialogFragment;
            SetupValueFragmentObservers(PlaylistSeperatorPromptDialogFragment);
            FreespacePromptDialogFragment = SupportFragmentManager.FindFragmentByTag(FREESPACE_PROMPT_TAG) as DefaultableItemValuePromptDialogFragment;
            SetupDefaultableItemValueFragmentObservers(FreespacePromptDialogFragment);

            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity:OnCreate - end");
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
            KillValueFragmentObservers(PlaylistFilenamePromptDialogFragment);
            KillValueFragmentObservers(PlaylistSeperatorPromptDialogFragment);
            KillDefaultableItemValueFragmentObservers(FreespacePromptDialogFragment);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void DoDownloadFreeSpaceOptions()
        {
            ViewModel.DownloadFreespaceOptions();
        }

        private void DoPlaylistFileOptions()
        {
            ViewModel.PlaylistFileOptions();
        }

        private void DoPlaylistSeperatorOptions()
        {
            ViewModel.PlaylistSeperatorOptions();
        }

        private void DoPlaylistFormatOptions()
        {
            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity:DoPlaylistFormatOptions");
            List<SelectableString> options = ViewModel.GetPlaylistFormatOptions();
            var sheet = SelectableStringListBottomSheetFragment.NewInstance(
                true,
                GetString(Resource.String.playlist_format_sheet_title),
                options);
            sheet.Show(SupportFragmentManager, BOTTOMSHEET_PLAYLIST_FORMAT_TAG);
        }

        private void DoDiagOutputOptions()
        {
            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity:DoDiagOutputOptions");
            List<SelectableString> options = ViewModel.GetDiagOutputOptions();
            var sheet = SelectableStringListBottomSheetFragment.NewInstance(
                true,
                GetString(Resource.String.diag_output_sheet_title),
                options);
            sheet.Show(SupportFragmentManager, BOTTOMSHEET_DIAG_OUTPUT_TAG);
        }

        private void DoDiagRetainTempOptions()
        {
            ViewModel.DiagRetainTempOptions();
        }

        public void BottomsheetItemSelected(string tag, int position, SelectableString item)
        {
            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity:BottomsheetItemSelected {tag}, {position}");
            switch (tag)
            {
                case BOTTOMSHEET_PLAYLIST_FORMAT_TAG:
                    ViewModel.DoPlaylistFormatOption(item);
                    break;
                case BOTTOMSHEET_DIAG_OUTPUT_TAG:
                    ViewModel.DoDiagOutputOption(item);
                    break;
            }
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.DisplayMessage += DisplayMessage;
            ViewModel.Observables.PlaylistFile += PlaylistFile;
            ViewModel.Observables.DownloadFreeSpace += DownloadFreeSpace;
            ViewModel.Observables.PromptForPlaylistFile += PromptForPlaylistFile;
            ViewModel.Observables.PromptForDownloadFreespace += PromptForDownloadFreespace;
            ViewModel.Observables.PlaylistFormat += PlaylistFormat;
            ViewModel.Observables.PlaylistSeperator += PlaylistSeperator;
            ViewModel.Observables.PromptForPlaylistSeperator += PromptForPlaylistSeperator;
            ViewModel.Observables.DiagOutput += DiagOutput;
            ViewModel.Observables.DiagRetainTemp += DiagRetainTemp;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.DisplayMessage -= DisplayMessage;
            ViewModel.Observables.PlaylistFile -= PlaylistFile;
            ViewModel.Observables.DownloadFreeSpace -= DownloadFreeSpace;
            ViewModel.Observables.PromptForPlaylistFile -= PromptForPlaylistFile;
            ViewModel.Observables.PromptForDownloadFreespace -= PromptForDownloadFreespace;
            ViewModel.Observables.PlaylistFormat -= PlaylistFormat;
            ViewModel.Observables.PlaylistSeperator -= PlaylistSeperator;
            ViewModel.Observables.PromptForPlaylistSeperator -= PromptForPlaylistSeperator;
            ViewModel.Observables.DiagOutput -= DiagOutput;
            ViewModel.Observables.DiagRetainTemp -= DiagRetainTemp;
        }

        private void DiagRetainTemp(object sender, Tuple<bool, string> parameters)
        {
            (bool isSet, string str) = parameters;
            RunOnUiThread(() =>
            {
                DiagRetainTempRowCheck.Checked = isSet;
                DiagRetainTempRowSubLabel.Text = str;
            });
        }

        private void DiagOutput(object sender, string str)
        {
            RunOnUiThread(() =>
            {
                DiagOutputRowSubLabel.Text = str;
            });
        }

        private void DownloadFreeSpace(object sender, string str)
        {
            RunOnUiThread(() =>
            {
                DownloadFreeSpaceRowSubLabel.Text = str;
            });
        }

        private void PlaylistFile(object sender, string str)
        {
            RunOnUiThread(() =>
            {
                PlaylistFileRowSubLabel.Text = str;
            });
        }

        private void PlaylistFormat(object sender, string str)
        {
            RunOnUiThread(() =>
            {
                PlaylistFormatRowSubLabel.Text = str;
            });
        }

        private void PlaylistSeperator(object sender, string str)
        {
            RunOnUiThread(() =>
            {
                PlaylistSeperatorRowSubLabel.Text = str;
            });
        }

        private void PromptForPlaylistFile(object sender, ValuePromptDialogFragment.ValuePromptDialogFragmentParameters parameters)
        {
            RunOnUiThread(() =>
            {
                PlaylistFilenamePromptDialogFragment = ValuePromptDialogFragment.NewInstance(parameters);
                SetupValueFragmentObservers(PlaylistFilenamePromptDialogFragment);
                PlaylistFilenamePromptDialogFragment.Show(SupportFragmentManager, PLAYLIST_FILENAME_PROMPT_TAG);
            });
        }

        private void PromptForPlaylistSeperator(object sender, ValuePromptDialogFragment.ValuePromptDialogFragmentParameters parameters)
        {
            RunOnUiThread(() =>
            {
                PlaylistSeperatorPromptDialogFragment = ValuePromptDialogFragment.NewInstance(parameters);
                SetupValueFragmentObservers(PlaylistSeperatorPromptDialogFragment);
                PlaylistSeperatorPromptDialogFragment.Show(SupportFragmentManager, PLAYLIST_SEPERATOR_PROMPT_TAG);
            });
        }

        private void PromptForDownloadFreespace(object sender, DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters parameters)
        {
            RunOnUiThread(() =>
            {
                FreespacePromptDialogFragment = DefaultableItemValuePromptDialogFragment.NewInstance(parameters);
                SetupDefaultableItemValueFragmentObservers(FreespacePromptDialogFragment);
                FreespacePromptDialogFragment.Show(SupportFragmentManager, FREESPACE_PROMPT_TAG);
            });
        }

        private void DisplayMessage(object sender, string message)
        {
            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity: DisplayMessage {message}");
            RunOnUiThread(() =>
            {
                Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
            });
        }

        private void SetupValueFragmentObservers(ValuePromptDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity: SetupFragmentObservers - {fragment.Tag}");
                fragment.OkSelected += OkSelected;
                fragment.CancelSelected += CancelSelected;
            }
        }

        private void KillValueFragmentObservers(ValuePromptDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity: KillFragmentObservers - {fragment.Tag}");
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
                case PLAYLIST_FILENAME_PROMPT_TAG:
                    KillValueFragmentObservers(PlaylistFilenamePromptDialogFragment);
                    ViewModel.SetPlaylistFilename(value);
                    break;
                case PLAYLIST_SEPERATOR_PROMPT_TAG:
                    KillValueFragmentObservers(PlaylistSeperatorPromptDialogFragment);
                    ViewModel.SetPlaylistSeperator(value);
                    break;
            }
        }

        private void CancelSelected(object sender, Tuple<string, string, string> parameters)
        {
            (string tag, string data, string value) = parameters;
            AndroidApplication.Logger.Debug(() => $"CancelSelected: {tag}");
            switch (tag)
            {
                case PLAYLIST_FILENAME_PROMPT_TAG:
                    KillValueFragmentObservers(PlaylistFilenamePromptDialogFragment);
                    break;
                case PLAYLIST_SEPERATOR_PROMPT_TAG:
                    KillValueFragmentObservers(PlaylistSeperatorPromptDialogFragment);
                    break;
            }
        }

        private void SetupDefaultableItemValueFragmentObservers(DefaultableItemValuePromptDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity: SetupFragmentObservers - {fragment.Tag}");
                fragment.OkSelected += DefaultableItemValueOkSelected;
                fragment.CancelSelected += DefaultableItemValueCancelSelected;
            }
        }

        private void KillDefaultableItemValueFragmentObservers(DefaultableItemValuePromptDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity: KillFragmentObservers - {fragment.Tag}");
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
                case FREESPACE_PROMPT_TAG:
                    KillDefaultableItemValueFragmentObservers(FreespacePromptDialogFragment);
                    ViewModel.SetFreespaceOnDownload(value, valueType);
                    break;
            }
        }

        private void DefaultableItemValueCancelSelected(object sender, Tuple<string, string> parameters)
        {
            (string tag, string data) = parameters;
            AndroidApplication.Logger.Debug(() => $"CancelSelected: {tag}");
            switch (tag)
            {
                case FREESPACE_PROMPT_TAG:
                    KillDefaultableItemValueFragmentObservers(FreespacePromptDialogFragment);
                    break;
            }
        }

    }

}
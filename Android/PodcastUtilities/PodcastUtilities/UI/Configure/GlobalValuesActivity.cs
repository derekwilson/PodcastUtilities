using Android.App;
using Android.Content.PM;
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
using static Android.Hardware.Camera;

namespace PodcastUtilities.UI.Configure
{
    [Activity(Label = "@string/global_values_activity_title", ParentActivity = typeof(EditConfigActivity))]
    internal class GlobalValuesActivity : AppCompatActivity
    {
        private const string FREESPACE_PROMPT_TAG = "freespace_prompt_tag";
        private const string PLAYLIST_FILENAME_PROMPT_TAG = "playlist_filename_prompt_tag";

        private AndroidApplication AndroidApplication;
        private GlobalValuesViewModel ViewModel;

        private NestedScrollView Container = null;
        private LinearLayoutCompat DownloadFreeSpaceRowContainer = null;
        private TextView DownloadFreeSpaceRowSubLabel = null;
        private LinearLayoutCompat PlaylistFileRowContainer = null;
        private TextView PlaylistFileRowSubLabel = null;

        private ValuePromptDialogFragment PlaylistFilenamePromptDialogFragment;
        private NumericPromptDialogFragment FreespacePromptDialogFragment;

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

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(GlobalValuesViewModel))) as GlobalValuesViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();

            DownloadFreeSpaceRowContainer.Click += (sender, e) => DoDownloadFreeSpaceOptions();
            PlaylistFileRowContainer.Click += (sender, e) => DoPlaylistFileOptions();

            PlaylistFilenamePromptDialogFragment = SupportFragmentManager.FindFragmentByTag(PLAYLIST_FILENAME_PROMPT_TAG) as ValuePromptDialogFragment;
            SetupValueFragmentObservers(PlaylistFilenamePromptDialogFragment);
            FreespacePromptDialogFragment = SupportFragmentManager.FindFragmentByTag(FREESPACE_PROMPT_TAG) as NumericPromptDialogFragment;
            SetupNumericFragmentObservers(FreespacePromptDialogFragment);

            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity:OnCreate - end");
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
            KillValueFragmentObservers(PlaylistFilenamePromptDialogFragment);
            KillNumericFragmentObservers(FreespacePromptDialogFragment);
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

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.DisplayMessage += DisplayMessage;
            ViewModel.Observables.PlaylistFile += PlaylistFile;
            ViewModel.Observables.DownloadFreeSpace += DownloadFreeSpace;
            ViewModel.Observables.PromptForPlaylistFile += PromptForPlaylistFile;
            ViewModel.Observables.PromptForDownloadFreespace += PromptForDownloadFreespace;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.DisplayMessage -= DisplayMessage;
            ViewModel.Observables.PlaylistFile -= PlaylistFile;
            ViewModel.Observables.DownloadFreeSpace -= DownloadFreeSpace;
            ViewModel.Observables.PromptForPlaylistFile -= PromptForPlaylistFile;
            ViewModel.Observables.PromptForDownloadFreespace += PromptForDownloadFreespace;
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

        private void PromptForPlaylistFile(object sender, ValuePromptDialogFragment.ValuePromptDialogFragmentParameters parameters)
        {
            RunOnUiThread(() =>
            {
                PlaylistFilenamePromptDialogFragment = ValuePromptDialogFragment.NewInstance(parameters);
                SetupValueFragmentObservers(PlaylistFilenamePromptDialogFragment);
                PlaylistFilenamePromptDialogFragment.Show(SupportFragmentManager, PLAYLIST_FILENAME_PROMPT_TAG);
            });
        }

        private void PromptForDownloadFreespace(object sender, NumericPromptDialogFragment.NumericPromptDialogFragmentParameters parameters)
        {
            RunOnUiThread(() =>
            {
                FreespacePromptDialogFragment = NumericPromptDialogFragment.NewInstance(parameters);
                SetupNumericFragmentObservers(FreespacePromptDialogFragment);
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
            }
        }

        private void SetupNumericFragmentObservers(NumericPromptDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity: SetupFragmentObservers - {fragment.Tag}");
                fragment.OkSelected += NumericOkSelected;
                fragment.CancelSelected += NumericCancelSelected;
            }
        }

        private void KillNumericFragmentObservers(NumericPromptDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity: KillFragmentObservers - {fragment.Tag}");
                fragment.OkSelected -= NumericOkSelected;
                fragment.CancelSelected -= NumericCancelSelected;
            }
        }

        private void NumericOkSelected(object sender, Tuple<string, string, long> parameters)
        {
            (string tag, string data, long value) = parameters;
            AndroidApplication.Logger.Debug(() => $"OkSelected: {tag}");
            switch (tag)
            {
                case FREESPACE_PROMPT_TAG:
                    KillNumericFragmentObservers(FreespacePromptDialogFragment);
                    ViewModel.SetFreespaceOnDownload(value);
                    break;
            }
        }

        private void NumericCancelSelected(object sender, Tuple<string, string, long> parameters)
        {
            (string tag, string data, long value) = parameters;
            AndroidApplication.Logger.Debug(() => $"CancelSelected: {tag}");
            switch (tag)
            {
                case FREESPACE_PROMPT_TAG:
                    KillNumericFragmentObservers(FreespacePromptDialogFragment);
                    break;
            }
        }

    }

}
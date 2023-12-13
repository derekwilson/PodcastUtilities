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

namespace PodcastUtilities.UI.Configure
{
    [Activity(Label = "@string/global_values_activity_title", ParentActivity = typeof(EditConfigActivity))]
    internal class GlobalValuesActivity : AppCompatActivity
    {
        private const string PLAYLIST_FILENAME_PROMPT_TAG = "playlist_filename_prompt_tag";

        private AndroidApplication AndroidApplication;
        private GlobalValuesViewModel ViewModel;

        private NestedScrollView Container = null;
        private LinearLayoutCompat DownloadFreeSpaceRowContainer = null;
        private TextView DownloadFreeSpaceRowSubLabel = null;
        private LinearLayoutCompat PlaylistFileRowContainer = null;
        private TextView PlaylistFileRowSubLabel = null;

        private ValuePromptDialogFragment PlaylistFilenamePromptDialogFragment;

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
            SetupFragmentObservers(PlaylistFilenamePromptDialogFragment);

            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity:OnCreate - end");
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
            KillFragmentObservers(PlaylistFilenamePromptDialogFragment);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void DoDownloadFreeSpaceOptions()
        {
            Toast.MakeText(Application.Context, "Not implemented", ToastLength.Short).Show();
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
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.DisplayMessage -= DisplayMessage;
            ViewModel.Observables.PlaylistFile -= PlaylistFile;
            ViewModel.Observables.DownloadFreeSpace -= DownloadFreeSpace;
            ViewModel.Observables.PromptForPlaylistFile -= PromptForPlaylistFile;
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
                SetupFragmentObservers(PlaylistFilenamePromptDialogFragment);
                PlaylistFilenamePromptDialogFragment.Show(SupportFragmentManager, PLAYLIST_FILENAME_PROMPT_TAG);
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

        private void SetupFragmentObservers(ValuePromptDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"GlobalValuesActivity: SetupFragmentObservers - {fragment.Tag}");
                fragment.OkSelected += OkSelected;
                fragment.CancelSelected += CancelSelected;
            }
        }

        private void KillFragmentObservers(ValuePromptDialogFragment fragment)
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
                    KillFragmentObservers(PlaylistFilenamePromptDialogFragment);
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
                    KillFragmentObservers(PlaylistFilenamePromptDialogFragment);
                    break;
            }
        }
    }

}
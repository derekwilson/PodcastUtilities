using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using Google.Android.Material.FloatingActionButton;
using PodcastUtilities.AndroidLogic.ViewModel.Configure;
using PodcastUtilities.AndroidLogic.ViewModel;
using System;
using Android.Content.PM;
using Google.Android.Material.Button;
using Android.Views;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.CustomViews;

namespace PodcastUtilities.UI.Configure
{
    [Activity(Label = "@string/add_feed_activity_title", WindowSoftInputMode = SoftInput.StateVisible, ParentActivity = typeof(EditConfigActivity))]
    internal class AddFeedActivity : AppCompatActivity
    {
        private AndroidApplication AndroidApplication;
        private AddFeedViewModel ViewModel;

        private ProgressSpinnerView ProgressSpinner = null;
        private EditText FolderText = null;
        private ImageView ClearFolderButton = null;
        private EditText FeedUrlText = null;
        private ImageView ClearFeedButton = null;
        private MaterialButton TestButton = null;
        private TextView TestFeedErrorMessage = null;
        private FloatingActionButton AddButton = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"AddFeedActivity:OnCreate");

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_add_feed);

            ProgressSpinner = FindViewById<ProgressSpinnerView>(Resource.Id.add_feed_progress_bar);
            FolderText = FindViewById<EditText>(Resource.Id.folder_prompt_value);
            ClearFolderButton = FindViewById<ImageView>(Resource.Id.folder_prompt_txt_clear);
            FeedUrlText = FindViewById<EditText>(Resource.Id.url_prompt_value);
            ClearFeedButton = FindViewById<ImageView>(Resource.Id.url_prompt_txt_clear);
            TestButton = FindViewById<MaterialButton>(Resource.Id.test_feed_button);
            TestFeedErrorMessage = FindViewById<TextView>(Resource.Id.test_feed_error_message);
            AddButton = FindViewById<FloatingActionButton>(Resource.Id.fab_add_add);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(AddFeedViewModel))) as AddFeedViewModel;

            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();

            ClearFolderButton.Click += (sender, e) => FolderText.Text = "";
            ClearFeedButton.Click += (sender, e) => FeedUrlText.Text = "";
            AddButton.Click += (sender, e) => ViewModel.AddFeed(FolderText.Text, FeedUrlText.Text);
            TestButton.Click += (sender, e) => ViewModel.TestFeed(FolderText.Text, FeedUrlText.Text);

            AndroidApplication.Logger.Debug(() => $"AddFeedActivity:OnCreate - end");
        }

        protected override void OnStart()
        {
            AndroidApplication.Logger.Debug(() => $"AddFeedActivity:OnStart");
            base.OnStart();
            DialogHelper.ShowSoftKeyboard(FolderText);
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            ViewModel?.CheckClipboardForUrl(FeedUrlText.Text);
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"AddFeedActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"AddFeedActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.Folder += Folder;
            ViewModel.Observables.Url += Url;
            ViewModel.Observables.DisplayMessage += DisplayMessage;
            ViewModel.Observables.Exit += Exit;
            ViewModel.Observables.DisplayErrorMessage += DisplayErrorMessage;
            ViewModel.Observables.HideErrorMessage += HideErrorMessage;
            ViewModel.Observables.StartDownloading += StartDownloading;
            ViewModel.Observables.EndDownloading += EndDownloading;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.Folder -= Folder;
            ViewModel.Observables.Url -= Url;
            ViewModel.Observables.DisplayMessage -= DisplayMessage;
            ViewModel.Observables.Exit -= Exit;
            ViewModel.Observables.DisplayErrorMessage -= DisplayErrorMessage;
            ViewModel.Observables.HideErrorMessage -= HideErrorMessage;
            ViewModel.Observables.StartDownloading -= StartDownloading;
            ViewModel.Observables.EndDownloading -= EndDownloading;
        }

        private void EndDownloading(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                ProgressViewHelper.CompleteProgress(ProgressSpinner, Window);
                TestButton.Enabled = true;
            });
        }

        private void StartDownloading(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                ProgressViewHelper.StartProgress(ProgressSpinner, Window);
                TestButton.Enabled = false;
            });
        }

        private void HideErrorMessage(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                TestFeedErrorMessage.Visibility = ViewStates.Gone;
            });
        }

        private void DisplayErrorMessage(object sender, string message)
        {
            RunOnUiThread(() =>
            {
                if (!String.IsNullOrEmpty(message))
                {
                    TestFeedErrorMessage.Text = message;
                }
                TestFeedErrorMessage.Visibility = ViewStates.Visible;
            });
        }

        private void DisplayMessage(object sender, string message)
        {
            AndroidApplication.Logger.Debug(() => $"AddFeedActivity: DisplayMessage {message}");
            RunOnUiThread(() =>
            {
                Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
            });
        }

        private void Exit(object sender, EventArgs e)
        {
            AndroidApplication.Logger.Debug(() => $"AddFeedActivity: Exit");
            RunOnUiThread(() =>
            {
                Finish();
            });
        }

        private void Folder(object sender, string str)
        {
            RunOnUiThread(() =>
            {
                FolderText.Text = "";
                FolderText.Append(str);     // because it sets the cursor to the end
            });
        }

        private void Url(object sender, string str)
        {
            RunOnUiThread(() =>
            {
                FeedUrlText.Text = "";
                FeedUrlText.Append(str);    // because it sets the cursor to the end
            });
        }
    }
}
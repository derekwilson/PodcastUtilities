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

namespace PodcastUtilities.UI.Configure
{
    [Activity(Label = "@string/add_feed_activity_title", ParentActivity = typeof(EditConfigActivity))]
    internal class AddFeedActivity : AppCompatActivity
    {
        private AndroidApplication AndroidApplication;
        private AddFeedViewModel ViewModel;

        private EditText FolderText = null;
        private EditText FeedUrlText = null;
        private MaterialButton TestButton;
        private FloatingActionButton AddButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"AddFeedActivity:OnCreate");

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_add_feed);

            FolderText = FindViewById<EditText>(Resource.Id.folder_prompt_value);
            FeedUrlText = FindViewById<EditText>(Resource.Id.url_prompt_value);
            TestButton = FindViewById<MaterialButton>(Resource.Id.test_feed_button);
            AddButton = FindViewById<FloatingActionButton>(Resource.Id.fab_add_add);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(AddFeedViewModel))) as AddFeedViewModel;

            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();

            AddButton.Click += (sender, e) => ViewModel.AddFeed(FolderText.Text, FeedUrlText.Text);
            TestButton.Click += (sender, e) => ViewModel.TestFeed(FeedUrlText.Text);

            AndroidApplication.Logger.Debug(() => $"AddFeedActivity:OnCreate - end");
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
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.Folder -= Folder;
            ViewModel.Observables.Url -= Url;
            ViewModel.Observables.DisplayMessage -= DisplayMessage;
            ViewModel.Observables.Exit -= Exit;
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
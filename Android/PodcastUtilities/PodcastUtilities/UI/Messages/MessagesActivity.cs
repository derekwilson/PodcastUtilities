using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Messages;
using PodcastUtilities.UI.Download;
using System;

namespace PodcastUtilities.UI.Messages
{
    [Activity(Label = "@string/messages_activity", Theme = "@style/AppTheme", ParentActivity = typeof(DownloadActivity))]
    public class MessagesActivity : AppCompatActivity
    {
        private AndroidApplication AndroidApplication;
        private MessagesViewModel ViewModel;

        ScrollView MessagesTextScroller = null;
        TextView MessagesText = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"MessagesActivity:OnCreate");

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_messages);

            MessagesTextScroller = FindViewById<ScrollView>(Resource.Id.messages_scroller);
            MessagesText = FindViewById<TextView>(Resource.Id.messages_text);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(MessagesViewModel))) as MessagesViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();

            AndroidApplication.Logger.Debug(() => $"MessagesActivity:OnCreate - end");
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"MessagesActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"MessagesActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.AddText += AddText;
            ViewModel.Observables.ScrollToTop += ScrollToTop;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.AddText -= AddText;
            ViewModel.Observables.ScrollToTop -= ScrollToTop;
        }

        private void ScrollToTop(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                // Appending to the textview auto scrolls the text to the bottom - force it back to the top
                MessagesTextScroller.FullScroll(FocusSearchDirection.Up);
            });
        }

        private void AddText(object sender, string textBlock)
        {
            AndroidApplication.Logger.Debug(() => $"MessagesActivity:AddTextBlock");
            RunOnUiThread(() =>
            {
                MessagesText.Append(textBlock);
            });
        }
    }
}
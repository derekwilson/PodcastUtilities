using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Utilities;
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

        private ProgressSpinnerView ProgressSpinner = null;
        private ScrollView MessagesTextScroller = null;
        private TextView MessagesText = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"MessagesActivity:OnCreate");

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_messages);

            ProgressSpinner = FindViewById<ProgressSpinnerView>(Resource.Id.progressBar);
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

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (BackKeyMapper.HandleKeyEvent(this, e))
            {
                AndroidApplication.Logger.Debug(() => $"MessagesActivity:DispatchKeyEvent - handled");
                return true;
            }
            return base.DispatchKeyEvent(e);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"MessagesActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                AndroidApplication.Logger.Debug(() => $"MessagesActivity:toolbar back button - exit");
                Finish();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.AddText += AddText;
            ViewModel.Observables.ScrollToTop += ScrollToTop;
            ViewModel.Observables.ResetText += ResetText;
            ViewModel.Observables.StartLoading += StartLoading;
            ViewModel.Observables.EndLoading += EndLoading;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.AddText -= AddText;
            ViewModel.Observables.ScrollToTop -= ScrollToTop;
            ViewModel.Observables.ResetText -= ResetText;
            ViewModel.Observables.StartLoading -= StartLoading;
            ViewModel.Observables.EndLoading -= EndLoading;
        }

        private void EndLoading(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                ProgressViewHelper.CompleteProgress(ProgressSpinner, Window);
            });
        }

        private void StartLoading(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                ProgressViewHelper.StartProgress(ProgressSpinner, Window);
            });
        }

        private void ResetText(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                MessagesText.Text = "";
            });
        }

        private void ScrollToTop(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                ScrollToTopOfText();
            });
        }

        private void ScrollToTopOfText()
        {
            // Appending to the textview auto scrolls the text to the bottom - force it back to the top
            MessagesTextScroller.FullScroll(FocusSearchDirection.Up);
            // Appending to the textview auto scrolls the text to the bottom - force it back to the top for old versions
            if (Build.VERSION.SdkInt <= BuildVersionCodes.P)
            {
                // scroll to the top of the page
                MessagesTextScroller.Parent.RequestChildFocus(MessagesTextScroller, MessagesTextScroller);
            }
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
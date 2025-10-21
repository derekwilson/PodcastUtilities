using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Text;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Configure;
using PodcastUtilities.AndroidLogic.ViewModel.Help;
using PodcastUtilities.UI.Settings;
using System;

namespace PodcastUtilities.UI.Help
{
    [Activity(Label = "@string/help_activity", ParentActivity = typeof(SettingsActivity))]
    public class HelpActivity : AppCompatActivity
    {
        private AndroidApplication AndroidApplication = null!;
        private HelpViewModel ViewModel = null!;

        ScrollView HelpTextScroller = null!;
        TextView HelpText = null!;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            AndroidApplication = (AndroidApplication)Application!;
            AndroidApplication.Logger.Debug(() => $"HelpActivity:OnCreate");

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_help);

            HelpTextScroller = FindViewById<ScrollView>(Resource.Id.help_scroller)!;
            HelpText = FindViewById<TextView>(Resource.Id.help_text)!;
            // make links clickable
            HelpText.MovementMethod = LinkMovementMethod.Instance;

            var factory = AndroidApplication.IocContainer?.Resolve<ViewModelFactory>() ?? throw new MissingMemberException("ViewModelFactory");
            ViewModel = (HelpViewModel)new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(HelpViewModel)));
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();

            AndroidApplication.Logger.Debug(() => $"HelpActivity:OnCreate - end");
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"HelpActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
        }

        public override bool DispatchKeyEvent(KeyEvent? e)
        {
            if (BackKeyMapper.HandleKeyEvent(this, e))
            {
                AndroidApplication.Logger.Debug(() => $"HelpActivity:DispatchKeyEvent - handled");
                return true;
            }
            return base.DispatchKeyEvent(e);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"HelpActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            if (OperatingSystem.IsAndroidVersionAtLeast(23))
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.SetText += SetText;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.SetText -= SetText;
        }

        private void SetText(object? sender, Tuple<string, Html.IImageGetter> parameters)
        {
            (string textBlock, Html.IImageGetter imageGetter) = parameters;
            AndroidApplication.Logger.Debug(() => $"HelpActivity:SetText");
            RunOnUiThread(() =>
            {
                HelpText.TextFormatted = HtmlCompat.FromHtml(textBlock, HtmlCompat.FromHtmlModeLegacy, imageGetter, null);
            });
        }
    }

}
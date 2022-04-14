using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Settings;
using System;

namespace PodcastUtilities.UI.Settings
{
    [Activity(Label = "@string/osl_title", ParentActivity = typeof(SettingsActivity))]

    public class OpenSourceLicensesActivity : AppCompatActivity
    {
        private AndroidApplication AndroidApplication;
        private OpenSourceLicensesViewModel ViewModel;

        ScrollView LicenseTextScroller = null;
        TextView LicenseText = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"OpenSourceLicensesActivity:OnCreate");

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_opensourcelicenses);

            LicenseTextScroller = FindViewById<ScrollView>(Resource.Id.license_scroller);
            LicenseText = FindViewById<TextView>(Resource.Id.license_text);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(OpenSourceLicensesViewModel))) as OpenSourceLicensesViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();

            AndroidApplication.Logger.Debug(() => $"OpenSourceLicensesActivity:OnCreate - end");
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"OpenSourceLicensesActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"OpenSourceLicensesActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.AddText += AddText;
            ViewModel.Observables.ScrollToTop += ScrollToTop;
            ViewModel.Observables.ResetText += ResetText;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.AddText -= AddText;
            ViewModel.Observables.ScrollToTop -= ScrollToTop;
            ViewModel.Observables.ResetText -= ResetText;
        }

        private void ResetText(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                LicenseText.Text = "";
            });
        }

        private void ScrollToTop(object sender, EventArgs e)
        {
            AndroidApplication.Logger.Debug(() => $"OpenSourceLicensesActivity:ScrollToTop Length = {LicenseText.Text.Length}");
            RunOnUiThread(() =>
            {
                // Appending to the textview auto scrolls the text to the bottom - force it back to the top
                LicenseTextScroller.FullScroll(FocusSearchDirection.Up);
            });
        }

        private void AddText(object sender, Tuple<string, string> textBlock)
        {
            RunOnUiThread(() =>
            {
                (string title, string text) = textBlock;
                AddTextToView(LicenseText, title, text);
            });
        }

        private void AddTextToView(TextView textView, string title, string text)
        {
            AndroidApplication.Logger.Debug(() => $"OpenSourceLicensesActivity:AddTextBlock {title}");

            var textTitle = new SpannableString(title);
            var titleLength = textTitle.Length();
            textTitle.SetSpan(new UnderlineSpan(), 0, titleLength, 0);
            textTitle.SetSpan(new StyleSpan(TypefaceStyle.Bold), 0, titleLength, 0);

            textView.Append(textTitle);
            textView.Append("\n\n");

            textView.Append(text);

            textView.Append("\n\n");
        }
    }
}
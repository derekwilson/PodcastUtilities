using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.Lifecycle;
using AndroidX.Preference;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Settings;
using PodcastUtilities.UI.Help;
using System;

namespace PodcastUtilities.UI.Settings
{
    public class SettingsFragment : PreferenceFragmentCompat, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        private AndroidApplication AndroidApplication;
        private SettingsViewModel ViewModel;

        public override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Activity.Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"SettingsFragment:OnCreate");

            base.OnCreate(savedInstanceState);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(SettingsViewModel))) as SettingsViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            SetPreferencesFromResource(Resource.Xml.settings, rootKey);
            UpdateAllPreferenceSummaries();
            UpdateVersion();
            UpdateHelp();
            UpdateOsl();
            UpdatePrivacy();
            UpdateShare();
        }

        public override void OnPause()
        {
            AndroidApplication.Logger.Debug(() => $"SettingsFragment:OnPause");
            base.OnPause();
            PreferenceManager.GetDefaultSharedPreferences(Activity).UnregisterOnSharedPreferenceChangeListener(this);
            ViewModel.Pause();
        }

        public override void OnResume()
        {
            AndroidApplication.Logger.Debug(() => $"SettingsFragment:OnResume");
            base.OnResume();
            PreferenceManager.GetDefaultSharedPreferences(Activity).RegisterOnSharedPreferenceChangeListener(this);
        }

        public override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"SettingsFragment:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
        }

        private void UpdateShare()
        {
            var preference = FindPreference(GetString(Resource.String.settings_config_share_key));
            if (preference != null)
            {
                preference.PreferenceClick += (sender, e) => ShareClick();
            }
        }

        private void ShareClick()
        {
            ViewModel.ShareConfig();
        }

        private void UpdatePrivacy()
        {
            // we cannot do this in the XML file as our package name changes between flavours
            var privacyPreference = FindPreference(GetString(Resource.String.settings_privacy_key));
            if (privacyPreference != null)
            {
                privacyPreference.PreferenceClick += (sender, e) => PrivacyClick();
            }
        }

        private void PrivacyClick()
        {
            var browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(GetString(Resource.String.settings_privacy_url)));
            StartActivity(browserIntent);
        }

        private void UpdateOsl()
        {
            // we cannot do this in the XML file as our package name changes between flavours
            var oslPreference = FindPreference(GetString(Resource.String.settings_osl_key));
            if (oslPreference != null)
            {
                oslPreference.PreferenceClick += (sender, e) => OslClick();
            }
        }
        private void OslClick()
        {
            var intent = new Intent(Activity, typeof(OpenSourceLicensesActivity));
            StartActivity(intent);
        }

        private void UpdateHelp()
        {
            // we cannot do this in the XML file as our package name changes between flavours
            var helpPreference = FindPreference(GetString(Resource.String.settings_help_key));
            if (helpPreference != null)
            {
                helpPreference.PreferenceClick += (sender, e) => HelpClick();
            }
        }

        private void HelpClick()
        {
            var intent = new Intent(Activity, typeof(HelpActivity));
            StartActivity(intent);
        }

        private void UpdateVersion()
        {
            // we cannot do this in the XML file as our package name changes between flavours
            var versionPreference = FindPreference(GetString(Resource.String.settings_version_key));
            if (versionPreference != null)
            {
                versionPreference.PreferenceClick += (sender, e) => VersionClick();
            }
        }

        private void VersionClick()
        {
#if DEBUG
            ViewModel.TestCrashReporting();
#endif
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            UpdatePreferenceSummaryByKey(key);
        }

        private void UpdateAllPreferenceSummaries()
        {
            foreach (var item in PreferenceManager.GetDefaultSharedPreferences(Activity).All.Keys)
            {
                UpdatePreferenceSummaryByKey(item);
            }
        }

        private void UpdatePreferenceSummaryByKey(string key)
        {
            if (key == null || FindPreference(key) == null)
            {
                return;
            }
            UpdatePreferenceSummary(FindPreference(key));
        }

        private void UpdatePreferenceSummary(Preference preference)
        {
            if (preference is ListPreference)
            {
                UpdateListPreferenceSummary(preference as ListPreference);
            }
        }

        private void UpdateListPreferenceSummary(ListPreference listPreference)
        {
            listPreference.Summary = listPreference.Entry;
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.Version += SetVersion;
            ViewModel.Observables.DisplayMessage += DisplayMessage;
            ViewModel.Observables.DisplayChooser += DisplayChooser;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.Version -= SetVersion;
            ViewModel.Observables.DisplayMessage -= DisplayMessage;
            ViewModel.Observables.DisplayChooser -= DisplayChooser;
        }

        private void SetVersion(object sender, string version)
        {
            var versionPreference = FindPreference(GetString(Resource.String.settings_version_key));
            versionPreference.Summary = version;
        }

        private void DisplayMessage(object sender, string message)
        {
            AndroidApplication.Logger.Debug(() => $"SettingsFragment: Message {message}");
            Activity?.RunOnUiThread(() =>
            {
                Toast.MakeText(Activity, message, ToastLength.Short).Show();
            });
        }

        private void DisplayChooser(object sender, Tuple<string, Intent> args)
        {
            (string title, Intent intent) = args;
            StartActivity(Intent.CreateChooser(intent, title));
        }

    }
}
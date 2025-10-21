using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.Lifecycle;
using AndroidX.Preference;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Configure;
using PodcastUtilities.AndroidLogic.ViewModel.Settings;
using PodcastUtilities.UI.Help;

namespace PodcastUtilities.UI.Settings
{
    public class SettingsFragment : PreferenceFragmentCompat, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        private AndroidApplication AndroidApplication = null!;
        private SettingsViewModel ViewModel = null!;

        public override void OnCreate(Bundle? savedInstanceState)
        {
            AndroidApplication = (AndroidApplication)Activity?.Application!;
            AndroidApplication.Logger.Debug(() => $"SettingsFragment:OnCreate");

            base.OnCreate(savedInstanceState);

            var factory = AndroidApplication.IocContainer?.Resolve<ViewModelFactory>() ?? throw new MissingMemberException("ViewModelFactory");
            ViewModel = (SettingsViewModel)new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(SettingsViewModel)));
            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            ViewModel.Initialise();
        }

        public override void OnCreatePreferences(Bundle? savedInstanceState, string? rootKey)
        {
            SetPreferencesFromResource(Resource.Xml.settings, rootKey);
            UpdateAllPreferenceSummaries();
            UpdateVersion();
            UpdateHelp();
            UpdateOsl();
            UpdatePrivacy();
        }

        public override void OnPause()
        {
            AndroidApplication.Logger.Debug(() => $"SettingsFragment:OnPause");
            base.OnPause();
            if (Activity != null)
            {
                PreferenceManager.GetDefaultSharedPreferences(Activity)?.UnregisterOnSharedPreferenceChangeListener(this);
            }
            ViewModel.Pause();
        }

        public override void OnResume()
        {
            AndroidApplication.Logger.Debug(() => $"SettingsFragment:OnResume");
            base.OnResume();
            if (Activity != null)
            {
                PreferenceManager.GetDefaultSharedPreferences(Activity)?.RegisterOnSharedPreferenceChangeListener(this);
            }
        }

        public override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"SettingsFragment:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
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
            ViewModel.PrivacySelected();
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
            if (Activity != null)
            {
                var intent = new Intent(Activity, typeof(OpenSourceLicensesActivity));
                StartActivity(intent);
            }
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
            if (Activity != null)
            {
                var intent = new Intent(Activity, typeof(HelpActivity));
                StartActivity(intent);
            }
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

        public void OnSharedPreferenceChanged(ISharedPreferences? sharedPreferences, string? key)
        {
            if (key !=  null)
            {
                UpdatePreferenceSummaryByKey(key);
            }
        }

        private void UpdateAllPreferenceSummaries()
        {
            if (Activity != null)
            {
                var keys = PreferenceManager.GetDefaultSharedPreferences(Activity)?.All?.Keys;
                if (keys != null)
                {
                    foreach (var item in keys)
                    {
                        UpdatePreferenceSummaryByKey(item);
                    }
                }
            }
        }

        private void UpdatePreferenceSummaryByKey(string key)
        {
            Preference? pref = FindPreference(key);
            if (pref != null)
            {
                UpdatePreferenceSummary(pref);
            }
        }

        private void UpdatePreferenceSummary(Preference preference)
        {
            if (preference is ListPreference)
            {
                UpdateListPreferenceSummary(preference as ListPreference);
            }
        }

        private void UpdateListPreferenceSummary(ListPreference? listPreference)
        {
            if (listPreference != null)
            {
                listPreference.Summary = listPreference.Entry;
            }
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.Version += SetVersion;
            ViewModel.Observables.DisplayMessage += DisplayMessage;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.Version -= SetVersion;
            ViewModel.Observables.DisplayMessage -= DisplayMessage;
        }

        private void SetVersion(object? sender, string version)
        {
            var versionPreference = FindPreference(GetString(Resource.String.settings_version_key));
            if (versionPreference != null)
            {
                versionPreference.Summary = version;
            }
        }

        private void DisplayMessage(object? sender, string message)
        {
            AndroidApplication.Logger.Debug(() => $"SettingsFragment: Message {message}");
            Activity?.RunOnUiThread(() =>
            {
                Toast.MakeText(Activity, message, ToastLength.Short)?.Show();
            });
        }
    }
}
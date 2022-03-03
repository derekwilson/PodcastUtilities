using Android.Content;
using Android.OS;
using AndroidX.Preference;

namespace PodcastUtilities.UI.Settings
{
    public class SettingsFragment : PreferenceFragmentCompat, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // attach ViewModel
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            SetPreferencesFromResource(Resource.Xml.settings, rootKey);
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
        }
    }
}
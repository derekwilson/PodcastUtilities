using Android.App;
using Android.Content;
using AndroidX.Preference;
using System;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IPreferencesProvider
    {
        string GetPreferenceString(string key, string defaultValue);
        void SetPreferenceString(string key, string value);
    }

    public class AndroidDefaultSharedPreferencesProvider : IPreferencesProvider
    {
        private Context ApplicationContext;

        public AndroidDefaultSharedPreferencesProvider(Context applicationContext)
        {
            ApplicationContext = applicationContext;
        }

        private ISharedPreferences? GetSharedPreferences()
        {
            return PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
        }

        public string GetPreferenceString(string key, string defaultValue)
        {
            return GetSharedPreferences()?.GetString(key, defaultValue) ?? defaultValue;
        }

        public void SetPreferenceString(string key, string value)
        {
            GetSharedPreferences()?.Edit()
                    ?.PutString(key, value)
                    ?.Apply();
        }
    }
}
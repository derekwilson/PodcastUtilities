using Android.Content;
using Android.Preferences;
using System;

namespace PodcastUtilitiesPOC.Utilities
{
    interface IPreferencesProvider
    {
        String GetPreferenceString(String key, String defaultValue);
        void SetPreferenceString(String key, String value);
        float GetPreferenceFloat(String key, float defaultValue);
        void SetPreferenceFloat(String key, float value);
    }

    class AndroidApplicationSharedPreferencesProvider : IPreferencesProvider
    {
        Context context;

        /// <summary>
        /// interact with the app shared prefs
        /// </summary>
        /// <param name="context">application context</param>
        public AndroidApplicationSharedPreferencesProvider(Context applicationContext)
        {
            this.context = applicationContext;
        }

        public float GetPreferenceFloat(string key, float defaultValue)
        {
            return GetPreferences().GetFloat(key, defaultValue);
        }

        public string GetPreferenceString(string key, string defaultValue)
        {
            return GetPreferences().GetString(key, defaultValue) ?? defaultValue;
        }

        public void SetPreferenceFloat(string key, float value)
        {
            GetPreferences()
                .Edit()
                .PutFloat(key, value)
                .Apply();
        }

        public void SetPreferenceString(string key, string value)
        {
            GetPreferences()
                .Edit()
                .PutString(key, value)
                .Apply();
        }

        private ISharedPreferences GetPreferences() 
        {
            return context.GetSharedPreferences(context.GetString(Resource.String.prefs_file_key), FileCreationMode.Private);
            //return PreferenceManager.GetDefaultSharedPreferences(context);
        }
    }
}
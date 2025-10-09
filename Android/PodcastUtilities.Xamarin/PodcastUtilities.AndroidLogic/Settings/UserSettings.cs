using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using System;

namespace PodcastUtilities.AndroidLogic.Settings
{
    public interface IUserSettings
    {
        enum DownloadNetworkType
        {
            WIFI,
            WIFIANDCELLULAR
        }

        DownloadNetworkType DownloadNetworkNeeded { get; }

        void ResetCache();
    }

    public class UserSettings : IUserSettings
    {
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IPreferencesProvider PreferencesProvider;

        public UserSettings(
            ILogger logger,
            IResourceProvider resourceProvider,
            IPreferencesProvider preferencesProvider)
        {
            Logger = logger;
            ResourceProvider = resourceProvider;
            PreferencesProvider = preferencesProvider;
        }

        bool _settingsLoadad = false;
        IUserSettings.DownloadNetworkType _downloadNetworkNeeded;

        // do not make this anything other than private
        private object SyncLock = new object();

        public IUserSettings.DownloadNetworkType DownloadNetworkNeeded
        {
            get
            {
                LoadSettingsIfNeeded();
                return _downloadNetworkNeeded;
            }
        }

        private void LoadSettingsIfNeeded()
        {
            lock (SyncLock)
            {
                if (_settingsLoadad)
                {
                    return;
                }
            }
            var preferenceValue = PreferencesProvider.GetPreferenceString(
                ResourceProvider.GetString(Resource.String.settings_download_network_key),
                ResourceProvider.GetString(Resource.String.settings_default_download_network)
                );
            Logger.Debug(() => $"UserSettings:LoadSettingsIfNeeded preferences value = {preferenceValue}");
            Enum.TryParse(preferenceValue, out IUserSettings.DownloadNetworkType _networkType);
            _downloadNetworkNeeded = _networkType;
            _settingsLoadad = true;
        }

        public void ResetCache()
        {
            Logger.Debug(() => $"UserSettings:ResetCache");
            _settingsLoadad = false;
        }
    }
}
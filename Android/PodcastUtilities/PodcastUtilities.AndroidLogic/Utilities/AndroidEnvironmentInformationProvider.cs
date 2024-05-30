using Android.Content;
using PodcastUtilities.AndroidLogic.Logging;
using System;
using System.Net;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IAndroidEnvironmentInformationProvider
    {
        bool IsKindleFire();
        bool IsWsa();
        string DeviceId { get; }
        float FontScaling { get; }
        string UiMode { get; }
        string Manufacturer { get; }
        string Brand { get; }
        string Model { get; }
        string OsVersion { get; }
    }

    public class AndroidEnvironmentInformationProvider : IAndroidEnvironmentInformationProvider
    {
        private Context applicationContext;
        private IResourceProvider resourceProvider;
        private ILogger Logger;

        public AndroidEnvironmentInformationProvider(
            Context applicationContext,
            IResourceProvider resourceProvider,
            ILogger logger)
        {
            this.applicationContext = applicationContext;
            this.resourceProvider = resourceProvider;
            Logger = logger;
        }

        string IAndroidEnvironmentInformationProvider.DeviceId
        {
            get
            {
                string id = "UNKNOWN";
                try
                {
                    id = Android.Provider.Settings.Secure.GetString(applicationContext.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                }
                catch (Exception ex)
                {
                    Logger.LogException(() => "AndroidEnvironmentInformationProvider: error getting DeviceId", ex);
                }
                return id;

            }
        }

        float IAndroidEnvironmentInformationProvider.FontScaling
        {
            get
            {
                float scaling = 0.0F;
                try
                {
                    scaling = applicationContext.Resources.Configuration.FontScale;
                }
                catch (Exception ex)
                {
                    Logger.LogException(() => "AndroidEnvironmentInformationProvider: error getting scaling", ex);
                }
                return scaling;
            }
        }

        public bool IsKindleFire()
        {
            return Android.OS.Build.Manufacturer.Equals("Amazon", StringComparison.InvariantCultureIgnoreCase)
                    && (Android.OS.Build.Model.Equals("Kindle Fire", StringComparison.InvariantCultureIgnoreCase)
                        || Android.OS.Build.Model.StartsWith("KF", StringComparison.InvariantCultureIgnoreCase));
        }

        public bool IsWsa()
        {
            return Android.OS.Build.Manufacturer.Equals("Microsoft Corporation", StringComparison.InvariantCultureIgnoreCase)
                    && (Android.OS.Build.Model.Equals("Subsystem for Android(TM)", StringComparison.InvariantCultureIgnoreCase));
        }

        string IAndroidEnvironmentInformationProvider.Manufacturer
        {
            get
            {
                return Android.OS.Build.Manufacturer;
            }
        }
        string IAndroidEnvironmentInformationProvider.Brand
        {
            get
            {
                return Android.OS.Build.Brand;
            }
        }
        string IAndroidEnvironmentInformationProvider.Model
        {
            get
            {
                return Android.OS.Build.Model;
            }
        }

        string IAndroidEnvironmentInformationProvider.OsVersion
        {
            get
            {
                return Android.OS.Build.VERSION.Release;
            }
        }

        string IAndroidEnvironmentInformationProvider.UiMode
        {
            get
            {
                // override in the "night" folder
                return resourceProvider.GetString(Resource.String.ui_mode);
            }
        }
    }
}
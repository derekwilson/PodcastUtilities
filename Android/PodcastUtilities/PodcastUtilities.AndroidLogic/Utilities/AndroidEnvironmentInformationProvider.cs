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
                    id = Android.Provider.Settings.Secure.GetString(applicationContext.ContentResolver, Android.Provider.Settings.Secure.AndroidId)
                        ?? "UNKNOWN";
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
                    scaling = applicationContext.Resources?.Configuration?.FontScale ?? 0.0F;
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
            var manufacturer = Android.OS.Build.Manufacturer;
            bool isAmazon = manufacturer?.Equals("Amazon", StringComparison.InvariantCultureIgnoreCase) ?? false;
            var model = Android.OS.Build.Model;
            bool isKindle = model?.Equals("Kindle Fire", StringComparison.InvariantCultureIgnoreCase) ?? false;
            bool isKf = model?.StartsWith("KF", StringComparison.InvariantCultureIgnoreCase) ?? false;
            return isAmazon && (isKindle || isKf);
        }

        public bool IsWsa()
        {
            var manufacturer = Android.OS.Build.Manufacturer;
            bool isMicrosoft = manufacturer?.Equals("Microsoft Corporation", StringComparison.InvariantCultureIgnoreCase) ?? false;
            var model = Android.OS.Build.Model;
            bool isWsa = model?.Equals("Subsystem for Android(TM)", StringComparison.InvariantCultureIgnoreCase) ?? false;
            return isMicrosoft && isWsa;
        }

        string IAndroidEnvironmentInformationProvider.Manufacturer
        {
            get
            {
                return Android.OS.Build.Manufacturer ?? "UNKNOWN";
            }
        }
        string IAndroidEnvironmentInformationProvider.Brand
        {
            get
            {
                return Android.OS.Build.Brand ?? "UNKNOWN";
            }
        }
        string IAndroidEnvironmentInformationProvider.Model
        {
            get
            {
                return Android.OS.Build.Model ?? "UNKNOWN";
            }
        }

        string IAndroidEnvironmentInformationProvider.OsVersion
        {
            get
            {
                return Android.OS.Build.VERSION.Release ?? "UNKNOWN";
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
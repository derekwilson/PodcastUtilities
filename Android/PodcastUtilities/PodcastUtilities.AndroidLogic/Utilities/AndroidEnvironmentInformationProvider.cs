using System;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IAndroidEnvironmentInformationProvider
    {
        bool IsKindleFire();
        bool IsWsa();
    }

    public class AndroidEnvironmentInformationProvider : IAndroidEnvironmentInformationProvider
    {
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
    }
}
using Android;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace PodcastUtilitiesPOC.Utilities
{
    class PermissionChecker
    {
        public static bool HasReadStoragePermission(Context context)
        {
            return HasPermissionBeenGranted(context, Manifest.Permission.ReadExternalStorage);
        }

        public static bool HasWriteStoragePermission(Context context)
        {
            return HasPermissionBeenGranted(context, Manifest.Permission.WriteExternalStorage);
        }

        public static bool HasManageStoragePermission(Context context)
        {
            // storage manager only happened in SDK30 / R
            // before that you only need write storage
            if (Build.VERSION.SdkInt < BuildVersionCodes.R)
            {
                return HasPermissionBeenGranted(context, Manifest.Permission.WriteExternalStorage);
            }
            return Android.OS.Environment.IsExternalStorageManager;
        }

        private static bool HasPermissionBeenGranted(Context context, string permission)
        {
            // dynamic permission requests were added in API 23 (Marshmellow)
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
            {
                return true;
            }
            return context.CheckSelfPermission(permission) == Permission.Granted;
        }
    }
}
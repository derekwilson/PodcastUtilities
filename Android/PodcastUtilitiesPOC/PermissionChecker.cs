using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilitiesPOC
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

        private static bool HasPermissionBeenGranted(Context context, string permission)
        {
            // dynamic permission requests were added in API 23 (Marshmellow)
            if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.M)
            {
                return true;
            }
            return context.CheckSelfPermission(permission) == Permission.Granted;
        } 
    }
}
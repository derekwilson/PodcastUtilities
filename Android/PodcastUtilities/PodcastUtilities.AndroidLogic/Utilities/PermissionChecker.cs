﻿using Android;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IPermissionChecker
    {
        public bool HasReadStoragePermission(Context context);

        public bool HasWriteStoragePermission(Context context);

        public bool HasPostNotifcationPermission(Context context);

        public bool HasManageStoragePermission(Context context);
    }

    public class PermissionChecker : IPermissionChecker
    {
        public bool HasReadStoragePermission(Context context)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                // if we are on Android 13+ we do not need any additional permissions to do our work (we will already have Manage File System)
                // anyway they will not be granted at Android 13 and above
                // so lets just say we have all the permissions we are going to need
                return true;
            }
            return HasPermissionBeenGranted(context, Manifest.Permission.ReadExternalStorage);
        }

        public bool HasWriteStoragePermission(Context context)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                // if we are on Android 13+ we do not need any additional permissions to do our work (we will already have Manage File System)
                // anyway they will not be granted at Android 13 and above
                // so lets just say we have all the permissions we are going to need
                return true;
            }
            return HasPermissionBeenGranted(context, Manifest.Permission.WriteExternalStorage);
        }

        public bool HasPostNotifcationPermission(Context context)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                return HasPermissionBeenGranted(context, Manifest.Permission.PostNotifications);
            }
            
            // earlier versions didnt need to ask
            return true;
        }

        public bool HasManageStoragePermission(Context context)
        {
            // storage manager only happened in SDK30 / R
            // before that you only need write storage
            if (Build.VERSION.SdkInt < BuildVersionCodes.R)
            {
                return HasWriteStoragePermission(context);
            }

            return Android.OS.Environment.IsExternalStorageManager;
        }

        private bool HasPermissionBeenGranted(Context context, string permission)
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
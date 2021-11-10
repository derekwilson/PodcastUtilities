using Android;
using Android.App;
using Android.Content;
using Android.Views;
using Google.Android.Material.Snackbar;
using System;

namespace PodcastUtilitiesPOC.Utilities
{
    class PermissionRequester
    {
        public const int RC_WRITE_EXTERNAL_STORAGE_PERMISSION_FOR_DOWNLOAD = 1000;
        public const int RC_WRITE_EXTERNAL_STORAGE_PERMISSION_FOR_SYNC = 1001;
        public const int RC_WRITE_EXTERNAL_STORAGE_PERMISSION_FOR_PLAYLIST = 1002;
        public const int RC_WRITE_EXTERNAL_STORAGE_PERMISSION_FOR_PURGE = 1003;
        public const int RC_WRITE_EXTERNAL_STORAGE_PERMISSION = 1004;
        public const int RC_READ_EXTERNAL_STORAGE_PERMISSION = 1100;
        static readonly string READ_PERMISSIONS_TO_REQUEST = Manifest.Permission.ReadExternalStorage;
        static readonly string WRITE_PERMISSIONS_TO_REQUEST = Manifest.Permission.WriteExternalStorage;
        static readonly string MANAGE_PERMISSIONS_TO_REQUEST = Manifest.Permission.ManageExternalStorage;

        public static void RequestReadStoragePermission(Activity activity, View view)
        {
            RequestPermission(activity, view, Resource.String.read_external_permissions_rationale, READ_PERMISSIONS_TO_REQUEST, RC_READ_EXTERNAL_STORAGE_PERMISSION);
        }

        public static void RequestWriteStoragePermission(Activity activity, View view, int code)
        {
            RequestPermission(activity, view, Resource.String.write_external_permissions_rationale, WRITE_PERMISSIONS_TO_REQUEST, code);
        }

        public static void RequestManageStoragePermission(Activity activity, View view, int code)
        {
            RequestPermission(activity, view, Resource.String.manage_external_permissions_rationale, MANAGE_PERMISSIONS_TO_REQUEST, code);
        }

        private static void RequestPermission(Activity activity, View view, int rationaleId, string permission, int code)
        {
            if (activity.ShouldShowRequestPermissionRationale(permission))
            {
                // Show an explanation to the user *asynchronously* -- don't block
                // this thread waiting for the user's response! After the user
                // sees the explanation, try again to request the permission.
                new AlertDialog.Builder(activity)
                        .SetTitle(Resource.String.permissions_title)
                        .SetMessage(rationaleId)
                        .SetPositiveButton(Resource.String.ok, delegate
                        {
                            // they clicked OK - ask for the permission
                            RequestAPermission(activity, permission, code);
                        })
                        .Create()
                        .Show();
                /*
                Snackbar.Make(view, rationaleId, BaseTransientBottomBar.LengthIndefinite)
                        .SetAction(Resource.String.ok, delegate { RequestAPermission(activity, permission, code); });
                */
            }
            else
            {
                RequestAPermission(activity, permission, code);
            }
        }

        private static void RequestAPermission(Activity activity, string permission, int code)
        {
            string[] permissionArray = { permission };
            activity.RequestPermissions(permissionArray, code);
        }
    }
}
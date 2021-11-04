using Android;
using Android.App;
using Android.Views;
using Google.Android.Material.Snackbar;

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

        public static void RequestReadStoragePermission(Activity activity, View view)
        {
            RequestPermission(activity, view, Resource.String.read_external_permissions_rationale, READ_PERMISSIONS_TO_REQUEST, RC_READ_EXTERNAL_STORAGE_PERMISSION);
        }

        public static void RequestWriteStoragePermission(Activity activity, View view, int code)
        {
            RequestPermission(activity, view, Resource.String.write_external_permissions_rationale, WRITE_PERMISSIONS_TO_REQUEST, code);
        }

        private static void RequestPermission(Activity activity, View view, int rationaleId, string permission, int code)
        {
            if (activity.ShouldShowRequestPermissionRationale(permission))
            {
                Snackbar.Make(view, rationaleId, BaseTransientBottomBar.LengthIndefinite)
                        .SetAction(Resource.String.ok, delegate { RequestAPermission(activity, permission, code); });

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
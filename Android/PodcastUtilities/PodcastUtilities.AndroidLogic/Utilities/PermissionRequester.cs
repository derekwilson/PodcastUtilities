using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Google.Android.Material.Dialog;
using System;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public class PermissionRequester
    {
        public const int REQUEST_CODE_WRITE_EXTERNAL_STORAGE_PERMISSION_FOR_DOWNLOAD = 1000;
        public const int REQUEST_CODE_WRITE_EXTERNAL_STORAGE_PERMISSION_FOR_SYNC = 1001;
        public const int REQUEST_CODE_WRITE_EXTERNAL_STORAGE_PERMISSION_FOR_PLAYLIST = 1002;
        public const int REQUEST_CODE_WRITE_EXTERNAL_STORAGE_PERMISSION_FOR_PURGE = 1003;
        public const int REQUEST_CODE_WRITE_EXTERNAL_STORAGE_PERMISSION = 1004;
        public const int REQUEST_CODE_READ_EXTERNAL_STORAGE_PERMISSION = 1100;

        static readonly string READ_PERMISSIONS_TO_REQUEST = Manifest.Permission.ReadExternalStorage;
        static readonly string WRITE_PERMISSIONS_TO_REQUEST = Manifest.Permission.WriteExternalStorage;

        public static void RequestReadStoragePermission(Activity activity)
        {
            RequestPermission(activity, Resource.String.read_external_permissions_rationale, READ_PERMISSIONS_TO_REQUEST, REQUEST_CODE_READ_EXTERNAL_STORAGE_PERMISSION);
        }

        public static void RequestWriteStoragePermission(Activity activity, int code)
        {
            RequestPermission(activity, Resource.String.write_external_permissions_rationale, WRITE_PERMISSIONS_TO_REQUEST, code);
        }

        public static void RequestManageStoragePermission(Activity activity, int code, string packageName)
        {
            // manage_storage only happened in SDK30 / R
            // before that you only need write storage
            if (Build.VERSION.SdkInt < BuildVersionCodes.R)
            {
                RequestPermission(activity, Resource.String.write_external_permissions_rationale, WRITE_PERMISSIONS_TO_REQUEST, code);
                return;
            }
            // we cannot use the existing permissions request mechanism - 'cuz that would just be too simple
            RequestManageStorage(activity, Resource.String.manage_external_permissions_rationale, code, packageName);
        }

        private static void RequestPermission(Activity activity, int rationaleId, string permission, int code)
        {
            if (activity.ShouldShowRequestPermissionRationale(permission))
            {
                // Show an explanation to the user *asynchronously* -- don't block
                // this thread waiting for the user's response! After the user
                // sees the explanation, try again to request the permission.
                new MaterialAlertDialogBuilder(activity)
                        .SetTitle(Resource.String.permissions_title)
                        .SetMessage(rationaleId)
                        .SetPositiveButton(Resource.String.ok, delegate
                        {
                            // they clicked OK - ask for the permission
                            RequestAPermission(activity, permission, code);
                        })
                        .Create()
                        .Show();
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

        private static void RequestManageStorage(Activity activity, int rationaleId, int code, string packageName)
        {
            // Show an explanation to the user *asynchronously* -- don't block
            // this thread waiting for the user's response! After the user
            // sees the explanation, try again to request the permission.
            new MaterialAlertDialogBuilder(activity)
                    .SetTitle(Resource.String.permissions_title)
                    .SetMessage(rationaleId)
                    .SetPositiveButton(Resource.String.ok, delegate
                    {
                        // they clicked OK - ask for the permission
                        try
                        {
                            Intent intent = new Intent(Android.Provider.Settings.ActionManageAllFilesAccessPermission);
                            intent.AddCategory("android.intent.category.DEFAULT");
                            intent.SetData(Android.Net.Uri.Parse($"package:{packageName}"));
                            activity.StartActivityForResult(intent, code);
                        }
                        catch (Exception)
                        {
                            Intent intent = new Intent();
                            intent.SetAction(Android.Provider.Settings.ActionManageAllFilesAccessPermission);
                            activity.StartActivityForResult(intent, code);
                        }
                    })
                    .Create()
                    .Show();
        }
    }
}
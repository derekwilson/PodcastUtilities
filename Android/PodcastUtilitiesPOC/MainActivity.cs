using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.App;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using PodcastUtilities.Common.Platform;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PodcastUtilitiesPOC
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const int REQUEST_SELECT_FILE = 3000;
        private AndroidApplication AndroidApplication;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = (Application as AndroidApplication);
            AndroidApplication.Logger.Debug(() => "MainActivity:OnCreate");
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // Get our UI controls from the loaded layout
            List<string> envirnment = WindowsEnvironmentInformationProvider.GetEnvironmentRuntimeDisplayInformation();
            StringBuilder builder = new StringBuilder();
            foreach (string line in envirnment)
            {
                builder.AppendLine(line);
            }
            SetTextViewText(Resource.Id.txtVersions, builder.ToString());
            SetTextViewText(Resource.Id.txtAppStorage, System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));

            Button btnLoad = FindViewById<Button>(Resource.Id.btnLoad);
            btnLoad.Click += (sender, e) => LoadConfig();

            bool isReadonly = Android.OS.Environment.MediaMountedReadOnly.Equals(Android.OS.Environment.ExternalStorageState);
            bool isWriteable = Android.OS.Environment.MediaMounted.Equals(Android.OS.Environment.ExternalStorageState);

            IEpisodeFinder podcastEpisodeFinder = null;
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnCreate {podcastEpisodeFinder != null}");
            podcastEpisodeFinder = (Application as AndroidApplication).IocContainer.Resolve<IEpisodeFinder>();
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnCreate {podcastEpisodeFinder != null}");
        }

        private void LoadConfig()
        {
            if (PermissionChecker.HasReadStoragePermission(this))
            {
                SelectFile();
            } else
            {
                PermissionRequester.RequestReadStoragePermission(this, FindViewById(Android.Resource.Id.Content));
            }
        }

        private void SelectFile()
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity:SelectFile");
            // ACTION_OPEN_DOCUMENT is the intent to choose a file via the system's file browser.
            var intent = new Intent(Intent.ActionOpenDocument);

            // Filter to only show results that can be "opened", such as a
            // file (as opposed to a list of contacts or timezones)
            intent.AddCategory(Intent.CategoryOpenable);

            // Filter using the MIME type.
            // If one wanted to search for ogg vorbis files, the type would be "audio/ogg".
            // To search for all documents available via installed storage providers, it would be "*/*".
            // as we know that other apps do not always report GPX MIME type correctly lets try for everything
            intent.SetType("*/*");

            StartActivityForResult(intent, REQUEST_SELECT_FILE);
        }

        private void OpenConfigFile(Android.Net.Uri uri)
        {
            ContentResolver resolver = Application.ContentResolver;
            var stream = resolver.OpenInputStream(uri);
            var xml = new XmlDocument();
            xml.Load(stream);
            var control = new ReadOnlyControlFile(xml);
            var podcasts = control.GetPodcasts();
            int count = 0;
            foreach (var item in podcasts)
            {
                count++;
            }

            AndroidApplication.Logger.Debug(() => $"MainActivity:Control Podcasts {control.GetSourceRoot()}");
            AndroidApplication.Logger.Debug(() => $"MainActivity:Control Podcasts {count}");

            SetTextViewText(Resource.Id.txtConfigFilePath, $"{uri.ToString()}");
            SetTextViewText(Resource.Id.txtOutput, $"{count}, {control.GetSourceRoot()}");
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnActivityResult {requestCode}, {resultCode}");
            if (requestCode == REQUEST_SELECT_FILE)
            {
                if (resultCode.Equals(Result.Ok))
                {
                    Toast.MakeText(Application.Context, "OK ", ToastLength.Short).Show();
                    AndroidApplication.Logger.Debug(() => $"MainActivity:OnActivityResult {data.Data.ToString()}");
                    OpenConfigFile(data.Data);
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            switch (requestCode)
            {
                case PermissionRequester.RC_READ_EXTERNAL_STORAGE_PERMISSION:
                    if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                    {
                        SelectFile();
                    } else
                    {
                        Toast.MakeText(Application.Context, "Denied ", ToastLength.Short).Show();
                    }
                    break;
                default:
                    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                    break;
            }
        }

        private void SetTextViewText(int id, string txt)
        {
            TextView txtView = FindViewById<TextView>(id);
            txtView.Text = txt;
        }
    }
}
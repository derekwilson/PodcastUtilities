using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.App;
using PodcastUtilities.Common.Feeds;
using PodcastUtilities.Common.Platform;
using System.Collections.Generic;
using System.Text;

namespace PodcastUtilitiesPOC
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(AndroidApplication.APP_NAME, "MainActivity:OnCreate");
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // Get our UI controls from the loaded layout
            TextView txtView = FindViewById<TextView>(Resource.Id.textView1);
            List<string> envirnment = WindowsEnvironmentInformationProvider.GetEnvironmentRuntimeDisplayInformation();
            StringBuilder builder = new StringBuilder();
            foreach (string line in envirnment)
            {
                builder.AppendLine(line);
            }
            txtView.Text = builder.ToString();

            IEpisodeFinder podcastEpisodeFinder = null;
            Log.Debug(AndroidApplication.APP_NAME, $"MainActivity:OnCreate {podcastEpisodeFinder != null}");
            podcastEpisodeFinder = (Application as AndroidApplication).IocContainer.Resolve<IEpisodeFinder>();
            Log.Debug(AndroidApplication.APP_NAME, $"MainActivity:OnCreate {podcastEpisodeFinder != null}");
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
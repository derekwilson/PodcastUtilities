﻿using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;

namespace PodcastUtilities
{
#if DEBUG
    [Activity(Label = "@string/app_name_debug", Theme = "@style/AppTheme", MainLauncher = true)]
#else
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
#endif
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
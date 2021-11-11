using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilitiesPOC
{
    [Activity(Label = "DownloadActivity", ParentActivity = typeof(MainActivity))]
    public class DownloadActivity : AppCompatActivity
    {
        private AndroidApplication AndroidApplication;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = (Application as AndroidApplication);
            AndroidApplication.Logger.Debug(() => $"DownloadActivity:OnCreate");

            base.OnCreate(savedInstanceState);

            // Set our view from the layout resource
            SetContentView(Resource.Layout.activity_download);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            AndroidApplication.Logger.Debug(() => $"DownloadActivity:OnOptionsItemSelected {item.ItemId}");
            switch (item.ItemId)
            {
                case Resource.Id.home:
                    OnBackPressed();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

    }
}
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilitiesPOC.AndroidLogic.ViewModel.Download
{
    public class RecyclerSyncItem
    {
        public ISyncItem SyncItem { get; set; }
        public int ProgressPercentage { get; set; }
        public PodcastInfo Podcast { get; set; }
        public bool Selected { get; set; }
    }

}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PodcastUtilities.AndroidLogic.ViewModel.Download;

namespace PodcastUtilities.AndroidLogic.Services.Download
{
	public interface IDownloadService
	{
        bool IsDownloading { get; }

        void StartDownload(List<DownloadRecyclerItem> allItems);
    }
}
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Lifecycle;
using PodcastUtilitiesPOC.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilitiesPOC.UI.Download
{
    class DownloadViewModel : AndroidViewModel, ILifecycleObserver
    {
        private ILogger Logger;

        public DownloadViewModel(
            Application app,
            ILogger logger
            ) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"DownloadViewModel:ctor");
        }

        public void Initialise()
        {
            Logger.Debug(() => $"DownloadViewModel:Initialise");
        }
    }
}
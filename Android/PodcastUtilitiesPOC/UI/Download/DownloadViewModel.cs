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
        public class ObservableGroup
        {
            public MutableLiveData Title = new MutableLiveData();
        }
        public ObservableGroup Observables = new ObservableGroup();

        private ILogger Logger;

        public DownloadViewModel(
            Application app,
            ILogger logger
            ) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"DownloadViewModel:ctor");
            Observables.Title.SetValue("Observed LiveData Title");
        }

        public void Initialise()
        {
            Logger.Debug(() => $"DownloadViewModel:Initialise");
        }

        [Lifecycle.Event.OnCreate]
        [Java.Interop.Export]
        public void OnCreate()
        {
            Logger.Debug(() => $"DownloadViewModel:OnCreate");
        }

        [Lifecycle.Event.OnStart]
        [Java.Interop.Export]
        public void OnStart()
        {
            Logger.Debug(() => $"DownloadViewModel:OnStart");
        }

        [Lifecycle.Event.OnResume]
        [Java.Interop.Export]
        public void OnResume()
        {
            Logger.Debug(() => $"DownloadViewModel:OnResume");
        }

        [Lifecycle.Event.OnPause]
        [Java.Interop.Export]
        public void OnPause()
        {
            Logger.Debug(() => $"DownloadViewModel:OnPause");
        }

        [Lifecycle.Event.OnStop]
        [Java.Interop.Export]
        public void OnStop()
        {
            Logger.Debug(() => $"DownloadViewModel:OnStop");
        }

        [Lifecycle.Event.OnDestroy]
        [Java.Interop.Export]
        public void OnDestroy()
        {
            Logger.Debug(() => $"DownloadViewModel:OnDestroy");
        }

    }
}
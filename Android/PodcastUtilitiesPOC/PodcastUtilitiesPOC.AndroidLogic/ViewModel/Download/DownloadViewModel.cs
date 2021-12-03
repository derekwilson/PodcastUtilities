using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Lifecycle;
using PodcastUtilitiesPOC.AndroidLogic.Logging;
using PodcastUtilitiesPOC.AndroidLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilitiesPOC.AndroidLogic.ViewModel.Download
{
    public class DownloadViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class LiveDataObservableGroup
        {
            public MutableLiveData Title = new MutableLiveData();
        }
        public LiveDataObservableGroup LiveDataObservables = new LiveDataObservableGroup();

        public class ObservableGroup
        {
            public EventHandler<string> Title;

        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application App;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;

        public DownloadViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider
            ) : base(app)
        {
            App = app;
            Logger = logger;
            ResourceProvider = resProvider;
            Logger.Debug(() => $"DownloadViewModel:ctor");
        }

        public void Initialise()
        {
            Logger.Debug(() => $"DownloadViewModel:Initialise - tested");
            var title = ResourceProvider.GetString(Resource.String.download_activity_title);
            //LiveDataObservables.Title.SetValue("Observed LiveData Title");
            Observables.Title?.Invoke(this, title);
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
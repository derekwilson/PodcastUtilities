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

namespace PodcastUtilitiesPOC.AndroidLogic.ViewModel.Example
{
    public class ExampleViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class LiveDataObservableGroup
        {
            public LiveDataObservableGroup(ILiveDataFactory livedateFactory)
            {
                Title = livedateFactory.CreateMutableLiveData();
            }

            public MutableLiveData Title { get; private set; }
        }
        public LiveDataObservableGroup LiveDataObservables;

        public class ObservableGroup
        {
            public EventHandler<string> Title;
            public EventHandler<string> Body;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private ILogger Logger;
        private IResourceProvider ResourceProvider;

        public ExampleViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resourceProvider,
            ILiveDataFactory livedateFactory
            ) : base(app)
        {
            Logger = logger;
            ResourceProvider = resourceProvider;

            Logger.Debug(() => $"ExampleViewModel:ctor");

            LiveDataObservables = new LiveDataObservableGroup(livedateFactory);
        }

        public void Initialise()
        {
            Logger.Debug(() => $"ExampleViewModel:Initialise");
            LiveDataObservables.Title.PostValue("Observed LiveData Title");
            Observables.Title?.Invoke(this, "Observed Title");
            Observables.Body?.Invoke(this, ResourceProvider.GetString(Resource.String.example_activity_body_text));
        }

        [Lifecycle.Event.OnCreate]
        [Java.Interop.Export]
        public void OnCreate()
        {
            Logger.Debug(() => $"ExampleViewModel:OnCreate");
        }

        [Lifecycle.Event.OnStart]
        [Java.Interop.Export]
        public void OnStart()
        {
            Logger.Debug(() => $"ExampleViewModel:OnStart");
        }

        [Lifecycle.Event.OnResume]
        [Java.Interop.Export]
        public void OnResume()
        {
            Logger.Debug(() => $"ExampleViewModel:OnResume");
        }

        [Lifecycle.Event.OnPause]
        [Java.Interop.Export]
        public void OnPause()
        {
            Logger.Debug(() => $"ExampleViewModel:OnPause");
        }

        [Lifecycle.Event.OnStop]
        [Java.Interop.Export]
        public void OnStop()
        {
            Logger.Debug(() => $"ExampleViewModel:OnStop");
        }

        [Lifecycle.Event.OnDestroy]
        [Java.Interop.Export]
        public void OnDestroy()
        {
            Logger.Debug(() => $"ExampleViewModel:OnDestroy");
        }

    }
}
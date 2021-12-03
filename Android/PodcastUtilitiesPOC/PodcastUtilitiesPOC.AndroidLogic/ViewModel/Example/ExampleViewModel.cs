using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Lifecycle;
using PodcastUtilitiesPOC.AndroidLogic.Logging;
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
            public MutableLiveData Title = new MutableLiveData();
        }
        public LiveDataObservableGroup LiveDataObservables = new LiveDataObservableGroup();

        public class ObservableGroup
        {
            public EventHandler<string> Title;

        }
        public ObservableGroup Observables = new ObservableGroup();

        private ILogger Logger;

        public ExampleViewModel(
            Application app,
            ILogger logger
            ) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"ExampleViewModel:ctor");
        }

        public void Initialise()
        {
            Logger.Debug(() => $"ExampleViewModel:Initialise");
            LiveDataObservables.Title.SetValue("Example Observed LiveData Title");
            Observables.Title?.Invoke(this, "Example Observed Title");
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
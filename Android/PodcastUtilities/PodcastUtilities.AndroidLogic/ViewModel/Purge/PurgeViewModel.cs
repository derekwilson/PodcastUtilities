using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using System;

namespace PodcastUtilities.AndroidLogic.ViewModel.Purge
{
    public class PurgeViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> Title;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IApplicationControlFileProvider ApplicationControlFileProvider;

        public PurgeViewModel(
            Application app,
            ILogger logger, 
            IResourceProvider resourceProvider, 
            IApplicationControlFileProvider applicationControlFileProvider) : base(app)
        {
            Logger = logger;
            ResourceProvider = resourceProvider;
            ApplicationControlFileProvider = applicationControlFileProvider;
        }

        public void Initialise()
        {
            Logger.Debug(() => $"PurgeViewModel:Initialise");
            Observables.Title?.Invoke(this, ResourceProvider.GetString(Resource.String.purge_activity_title));
        }

    }
}
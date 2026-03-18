using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;

namespace PodcastUtilities.AndroidLogic.ViewModel.Share
{
    public class ShareEpisodeViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string>? Title;
            public EventHandler<string>? DisplayMessage;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IApplicationControlFileProvider ApplicationControlFileProvider;
        private ICrashReporter CrashReporter;
        private IAnalyticsEngine AnalyticsEngine;

        // do not make this anything other than private
        private object SyncLock = new object();

        private string? FolderSelected = null;

        public ShareEpisodeViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resourceProvider,
            IApplicationControlFileProvider applicationControlFileProvider,
            ICrashReporter crashReporter, 
            IAnalyticsEngine analyticsEngine) : base(app)
        {
            Logger = logger;
            ResourceProvider = resourceProvider;
            ApplicationControlFileProvider = applicationControlFileProvider;
            CrashReporter = crashReporter;
            AnalyticsEngine = analyticsEngine;
        }

        public void Initialise(string? folder)
        {
            Logger.Debug(() => $"ShareEpisodeViewModel:Initialise - {folder}");
            FolderSelected = folder;
            Observables.Title?.Invoke(this, ResourceProvider.GetString(Resource.String.share_episode_activity_title));
        }
    }
}

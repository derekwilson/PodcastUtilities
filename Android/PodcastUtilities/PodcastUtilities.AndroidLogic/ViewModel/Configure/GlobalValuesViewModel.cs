using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using System;

namespace PodcastUtilities.AndroidLogic.ViewModel.Configure
{
    public class GlobalValuesViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> DisplayMessage;
            public EventHandler<string> DownloadFreeSpace;
            public EventHandler<string> PlaylistFile;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IApplicationControlFileProvider ApplicationControlFileProvider;
        private ICrashReporter CrashReporter;
        private IAnalyticsEngine AnalyticsEngine;
        private IFileSystemHelper FileSystemHelper;
        private IApplicationControlFileFactory ApplicationControlFileFactory;

        public GlobalValuesViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            ICrashReporter crashReporter,
            IAnalyticsEngine analyticsEngine,
            IFileSystemHelper fileSystemHelper,
            IApplicationControlFileFactory applicationControlFileFactory) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"GlobalValuesViewModel:ctor");

            ApplicationContext = app;
            ResourceProvider = resProvider;
            ApplicationControlFileProvider = appControlFileProvider;
            ApplicationControlFileProvider.ConfigurationUpdated += ConfigurationUpdated;
            CrashReporter = crashReporter;
            AnalyticsEngine = analyticsEngine;
            FileSystemHelper = fileSystemHelper;
            ApplicationControlFileFactory = applicationControlFileFactory;
        }

        private void ConfigurationUpdated(object sender, EventArgs e)
        {
            Logger.Debug(() => $"GlobalValuesViewModel:ConfigurationUpdated");
            RefreshConfigDisplay();
        }

        public void Initialise()
        {
            Logger.Debug(() => $"GlobalValuesViewModel:Initialise");
            RefreshConfigDisplay();
        }

        private void RefreshConfigDisplay()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();

            var freeSpaceSublabel = string.Format(ResourceProvider.GetString(Resource.String.download_free_space_label_fmt), controlFile.GetFreeSpaceToLeaveOnDownload());
            Observables.DownloadFreeSpace?.Invoke(this, freeSpaceSublabel);
            Observables.PlaylistFile?.Invoke(this, controlFile.GetPlaylistFileName());
        }

        [Lifecycle.Event.OnDestroy]
        [Java.Interop.Export]
        public void OnDestroy()
        {
            Logger.Debug(() => $"GlobalValuesViewModel:OnDestroy");
            ApplicationControlFileProvider.ConfigurationUpdated -= ConfigurationUpdated;
        }
    }

}
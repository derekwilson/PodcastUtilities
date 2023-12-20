using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common.Configuration;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidLogic.ViewModel.Configure
{
    public class FeedDefaultsViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> DisplayMessage;
            public EventHandler<string> MaxDaysOld;
            public EventHandler<string> DownloadStrategy;
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

        public FeedDefaultsViewModel(
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
            Logger.Debug(() => $"FeedDefaultsViewModel:ctor");

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
            Logger.Debug(() => $"FeedDefaultsViewModel:ConfigurationUpdated");
            RefreshConfigDisplay();
        }

        public void Initialise()
        {
            Logger.Debug(() => $"FeedDefaultsViewModel:Initialise");
            RefreshConfigDisplay();
        }

        private void RefreshConfigDisplay()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();

            var maxDaysOldSublabel = string.Format(ResourceProvider.GetString(Resource.String.max_days_old_label_fmt), controlFile.GetDefaultMaximumDaysOld());
            Observables.MaxDaysOld?.Invoke(this, maxDaysOldSublabel);
            Observables.DownloadStrategy?.Invoke(this, controlFile.GetDefaultDownloadStrategy().ToString());
        }

        [Lifecycle.Event.OnDestroy]
        [Java.Interop.Export]
        public void OnDestroy()
        {
            Logger.Debug(() => $"FeedDefaultsViewModel:OnDestroy");
            ApplicationControlFileProvider.ConfigurationUpdated -= ConfigurationUpdated;
        }

        public List<SelectableString> GetDownloadStrategyOptions()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            List<SelectableString> options = new List<SelectableString>()
            {
                GenerateOption(PodcastEpisodeDownloadStrategy.HighTide, controlFile.GetDefaultDownloadStrategy()),
                GenerateOption(PodcastEpisodeDownloadStrategy.All, controlFile.GetDefaultDownloadStrategy()),
                GenerateOption(PodcastEpisodeDownloadStrategy.Latest, controlFile.GetDefaultDownloadStrategy()),
            };
            return options;
        }

        /*
        private SelectableString GenerateOption(PodcastEpisodeDownloadStrategy strategy, PodcastEpisodeDownloadStrategy currentValue)
        {
            return new SelectableString((int)strategy, strategy.ToString(), strategy == currentValue);
        }
        */

        private SelectableString GenerateOption<TYPE>(TYPE enumOption, TYPE currentValue)
        {
            return new SelectableString(Convert.ToInt32(enumOption), enumOption.ToString(), EqualityComparer<TYPE>.Default.Equals(enumOption, currentValue));
        }

        public void DoDownloadStrategyOption(SelectableString item)
        {
            Logger.Debug(() => $"FeedDefaultsViewModel:DoDownloadStrategyOption = {item.Id}, {item.Name}");
            var strategy = (PodcastEpisodeDownloadStrategy)item.Id;
            var ControlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            ControlFile.SetDefaultDownloadStrategy(strategy);
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }
    }
}
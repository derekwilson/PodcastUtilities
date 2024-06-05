using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Settings;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace PodcastUtilities.AndroidLogic.ViewModel.Settings
{
    public class SettingsViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> Version;
            public EventHandler<string> DisplayMessage;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private IAndroidApplication AndroidApplication;
        private ILogger Logger;
        private ICrashReporter CrashReporter;
        private IUserSettings UserSettings;
        private IAnalyticsEngine AnalyticsEngine;

        public SettingsViewModel(
            Application app,
            ILogger logger,
            IAndroidApplication androidApplication,
            ICrashReporter crashReporter,
            IUserSettings userSettings,
            IAnalyticsEngine analyticsEngine) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"SettingsViewModel:ctor");

            AndroidApplication = androidApplication;
            CrashReporter = crashReporter;
            UserSettings = userSettings;
            AnalyticsEngine = analyticsEngine;
        }

        public void Initialise()
        {
            Logger.Debug(() => $"SettingsViewModel:Initialise");
            List<string> environment = WindowsEnvironmentInformationProvider.GetEnvironmentRuntimeDisplayInformation();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(AndroidApplication.DisplayVersion);
            builder.AppendLine(AndroidApplication.DisplayPackage);
            foreach (string line in environment)
            {
                builder.AppendLine(line);
            }
            Observables.Version?.Invoke(this, builder.ToString());
        }

        public void Pause()
        {
            // we should force the settings to be read again in case any values have changed
            UserSettings.ResetCache();
        }

        public void TestCrashReporting()
        {
            CrashReporter.TestReporting();
        }

        public void PrivacySelected()
        {
            AnalyticsEngine.ViewPageEvent(IAnalyticsEngine.Page_Privacy, 0);
        }
    }
}
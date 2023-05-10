using Android.App;
using Android.Content;
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
            public EventHandler<Tuple<string, Intent>> DisplayChooser;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private IAndroidApplication AndroidApplication;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private ICrashReporter CrashReporter;
        private IUserSettings UserSettings;
        private IApplicationControlFileProvider ApplicationControlFileProvider;

        public SettingsViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IAndroidApplication androidApplication,
            ICrashReporter crashReporter,
            IUserSettings userSettings,
            IApplicationControlFileProvider applicationControlFileProvider) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"SettingsViewModel:ctor");

            ResourceProvider = resProvider;
            AndroidApplication = androidApplication;
            CrashReporter = crashReporter;
            UserSettings = userSettings;
            ApplicationControlFileProvider = applicationControlFileProvider;
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

        public void ShareConfig()
        {
            if (ApplicationControlFileProvider == null)
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.settings_share_no_controlfile));
                return;
            }
            var intent = ApplicationControlFileProvider.GetApplicationConfigurationSharingIntent();
            if (intent != null)
            {
                Observables.DisplayChooser?.Invoke(this, Tuple.Create(ResourceProvider.GetString(Resource.String.settings_share_chooser_title), intent));
            } else
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.settings_share_no_controlfile));
            }
        }
    }
}
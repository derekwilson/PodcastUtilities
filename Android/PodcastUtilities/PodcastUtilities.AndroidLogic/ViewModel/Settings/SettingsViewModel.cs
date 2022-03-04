using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.AndroidLogic.ViewModel.Settings
{
    public class SettingsViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> Version;
            public EventHandler NavigateToOsl;
            public EventHandler NavigateToPrivacy;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private IAndroidApplication AndroidApplication;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;

        public SettingsViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IAndroidApplication androidApplication) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"SettingsViewModel:ctor");

            ApplicationContext = app;
            ResourceProvider = resProvider;
            AndroidApplication = androidApplication;
        }

        public void Initialise()
        {
            Logger.Debug(() => $"SettingsViewModel:Initialise");
            List<string> environment = WindowsEnvironmentInformationProvider.GetEnvironmentRuntimeDisplayInformation();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(AndroidApplication.DisplayVersion);
            foreach (string line in environment)
            {
                builder.AppendLine(line);
            }
            Observables.Version?.Invoke(this, builder.ToString());
        }
    }
}
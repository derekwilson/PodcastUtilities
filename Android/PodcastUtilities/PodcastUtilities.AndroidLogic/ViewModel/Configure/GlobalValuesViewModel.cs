using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using System;
using static Android.Provider.DocumentsContract;

namespace PodcastUtilities.AndroidLogic.ViewModel.Configure
{
    public class GlobalValuesViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> DisplayMessage;
            public EventHandler<string> DownloadFreeSpace;
            public EventHandler<string> PlaylistFile;
            public EventHandler<ValuePromptDialogFragment.ValuePromptDialogFragmentParameters> PromptForPlaylistFile;
            public EventHandler<NumericPromptDialogFragment.NumericPromptDialogFragmentParameters> PromptForDownloadFreespace;
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

        public void PlaylistFileOptions()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            ValuePromptDialogFragment.ValuePromptDialogFragmentParameters promptParams = new ValuePromptDialogFragment.ValuePromptDialogFragmentParameters()
            {
                Title = ResourceProvider.GetString(Resource.String.prompt_playlist_title),
                Ok = ResourceProvider.GetString(Resource.String.action_ok),
                Cancel = ResourceProvider.GetString(Resource.String.action_cancel),
                Prompt = ResourceProvider.GetString(Resource.String.prompt_playlist_prompt),
                Value = controlFile.GetPlaylistFileName(),
            };
            Observables.PromptForPlaylistFile?.Invoke(this, promptParams);
        }

        public void SetPlaylistFilename(string value)
        {
            Logger.Debug(() => $"GlobalValuesViewModel:SetPlaylistFilename = {value}");
            var ControlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            ControlFile.SetPlaylistFileName(value);
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }

        public void DownloadFreespaceOptions()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            NumericPromptDialogFragment.NumericPromptDialogFragmentParameters promptParams = new NumericPromptDialogFragment.NumericPromptDialogFragmentParameters()
            {
                Title = ResourceProvider.GetString(Resource.String.prompt_freespace_download_title),
                Ok = ResourceProvider.GetString(Resource.String.action_ok),
                Cancel = ResourceProvider.GetString(Resource.String.action_cancel),
                Prompt = ResourceProvider.GetString(Resource.String.prompt_freespace_download_prompt),
                Value = controlFile.GetFreeSpaceToLeaveOnDownload(),
            };
            Observables.PromptForDownloadFreespace?.Invoke(this, promptParams);
        }

        public void SetFreespaceOnDownload(long value)
        {
            Logger.Debug(() => $"GlobalValuesViewModel:SetFreespaceOnDownload = {value}");
            var ControlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            ControlFile.SetFreeSpaceToLeaveOnDownload(value);
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }

    }

}
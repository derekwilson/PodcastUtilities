using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Exceptions;
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
            public EventHandler<ValuePromptDialogFragment.ValuePromptDialogFragmentParameters> PromptForPlaylistFile;
            public EventHandler<DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters> PromptForDownloadFreespace;
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
        private IValueConverter ValueConverter;

        public GlobalValuesViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            ICrashReporter crashReporter,
            IAnalyticsEngine analyticsEngine,
            IFileSystemHelper fileSystemHelper,
            IApplicationControlFileFactory applicationControlFileFactory,
            IValueConverter valueConverter
            ) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"GlobalValuesViewModel:ctor");

            ApplicationContext = app;
            ResourceProvider = resProvider;
            ApplicationControlFileProvider = appControlFileProvider;
            CrashReporter = crashReporter;
            AnalyticsEngine = analyticsEngine;
            FileSystemHelper = fileSystemHelper;
            ApplicationControlFileFactory = applicationControlFileFactory;
            ValueConverter = valueConverter;
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

            if (controlFile.GetFreeSpaceToLeaveOnDownload() == 0)
            {
                Observables.DownloadFreeSpace?.Invoke(this, ResourceProvider.GetString(Resource.String.prompt_freespace_download_named_prompt));
            }
            else
            {
                var freeSpaceSublabel = string.Format(ResourceProvider.GetString(Resource.String.download_free_space_label_fmt), controlFile.GetFreeSpaceToLeaveOnDownload());
                Observables.DownloadFreeSpace?.Invoke(this, freeSpaceSublabel);
            }
            Observables.PlaylistFile?.Invoke(this, controlFile.GetPlaylistFileName());
        }

        [Lifecycle.Event.OnCreate]
        [Java.Interop.Export]
        public void OnCreate()
        {
            Logger.Debug(() => $"GlobalValuesViewModel:OnCreate");
            ApplicationControlFileProvider.ConfigurationUpdated += ConfigurationUpdated;
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
            DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters promptParams = new DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters()
            {
                Title = ResourceProvider.GetString(Resource.String.prompt_freespace_download_title),
                Ok = ResourceProvider.GetString(Resource.String.action_ok),
                Cancel = ResourceProvider.GetString(Resource.String.action_cancel),
                NamedPrompt = ResourceProvider.GetString(Resource.String.prompt_freespace_download_named_prompt),
                CustomPrompt = ResourceProvider.GetString(Resource.String.prompt_freespace_download_custom_prompt),
                CurrentValue = ValueConverter.ConvertToString(controlFile.GetFreeSpaceToLeaveOnDownload()),
                NamedValue = "0",
                IsNumeric = true,
            };
            if (controlFile.GetFreeSpaceToLeaveOnDownload() == 0)
            {
                promptParams.ValueType = DefaultableItemValuePromptDialogFragment.ItemValueType.Named;
            } 
            else
            {
                promptParams.ValueType = DefaultableItemValuePromptDialogFragment.ItemValueType.Custom;
            }
            Observables.PromptForDownloadFreespace?.Invoke(this, promptParams);
        }

        public void SetFreespaceOnDownload(string value, DefaultableItemValuePromptDialogFragment.ItemValueType valueType)
        {
            Logger.Debug(() => $"GlobalValuesViewModel:SetFreespaceOnDownload = {value}");
            var ControlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            // this is a global it cannot be defaulted
            switch (valueType)
            {
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Defaulted:
                    CrashReporter.LogNonFatalException(new ConfigurationException("Defaulted value type is not allowed here"));
                    break;
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Named:
                    ControlFile.SetFreeSpaceToLeaveOnDownload(0);
                    break;
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Custom:
                    ControlFile.SetFreeSpaceToLeaveOnDownload(ValueConverter.ConvertStringToLong(value));
                    break;
            }
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }

    }

}
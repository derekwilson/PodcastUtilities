using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Exceptions;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Playlists;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidLogic.ViewModel.Configure
{
    public class GlobalValuesViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string>? DisplayMessage;
            public EventHandler<string>? DownloadFreeSpace;
            public EventHandler<string>? MaxConcurrentDownloads;
            public EventHandler<string>? RetryWait;
            public EventHandler<string>? PlaylistFile;
            public EventHandler<string>? PlaylistFormat;
            public EventHandler<string>? PlaylistSeperator;
            public EventHandler<string>? DiagOutput;
            public EventHandler<Tuple<bool, string>>? DiagRetainTemp;
            public EventHandler<ValuePromptDialogFragment.ValuePromptDialogFragmentParameters>? PromptForPlaylistFile;
            public EventHandler<DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters>? PromptForDownloadFreespace;
            public EventHandler<ValuePromptDialogFragment.ValuePromptDialogFragmentParameters>? PromptForPlaylistSeperator;
            public EventHandler<ValuePromptDialogFragment.ValuePromptDialogFragmentParameters>? PromptForMaxConcurrentDownloads;
            public EventHandler<ValuePromptDialogFragment.ValuePromptDialogFragmentParameters>? PromptForRetrySeconds;
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
        private IValueFormatter ValueFormatter;

        public GlobalValuesViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            ICrashReporter crashReporter,
            IAnalyticsEngine analyticsEngine,
            IFileSystemHelper fileSystemHelper,
            IApplicationControlFileFactory applicationControlFileFactory,
            IValueConverter valueConverter,
            IValueFormatter valueFormatter) : base(app)
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
            ValueFormatter = valueFormatter;
        }

        private void ConfigurationUpdated(object? sender, EventArgs e)
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

            var maxConcurrentDownloadsSublabel = string.Format(ResourceProvider.GetString(Resource.String.max_concurrent_downloads_label_fmt), controlFile.GetMaximumNumberOfConcurrentDownloads());
            Observables.MaxConcurrentDownloads?.Invoke(this, maxConcurrentDownloadsSublabel);

            var retryWaitSublabel = string.Format(ResourceProvider.GetString(Resource.String.retry_seconds_label_fmt), controlFile.GetRetryWaitInSeconds());
            Observables.RetryWait?.Invoke(this, retryWaitSublabel);

            Observables.PlaylistFile?.Invoke(this, controlFile.GetPlaylistFileName());
            Observables.PlaylistFormat?.Invoke(this, ValueFormatter.GetPlaylistFormatTextLong(controlFile.GetPlaylistFormat()));

            var seperatorSublabel = string.Format(ResourceProvider.GetString(Resource.String.prompt_playlist_seperator_label_fmt), controlFile.GetPlaylistPathSeparator());
            Observables.PlaylistSeperator?.Invoke(this, seperatorSublabel);

            Observables.DiagOutput?.Invoke(this, ValueFormatter.GetDiagOutputTextLong(controlFile.GetDiagnosticOutput()));

            var labelId = Resource.String.diag_retain_temp_off;
            if (controlFile.GetDiagnosticRetainTemporaryFiles())
            {
                labelId = Resource.String.diag_retain_temp_on;
            }
            Observables.DiagRetainTemp?.Invoke(this, Tuple.Create(controlFile.GetDiagnosticRetainTemporaryFiles(), ResourceProvider.GetString(labelId)));
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

        public void DoMaxConcurrentDownloadsOptions()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            ValuePromptDialogFragment.ValuePromptDialogFragmentParameters promptParams = new ValuePromptDialogFragment.ValuePromptDialogFragmentParameters()
            {
                Title = ResourceProvider.GetString(Resource.String.prompt_max_concurrent_downloads_title),
                Ok = ResourceProvider.GetString(Resource.String.action_ok),
                Cancel = ResourceProvider.GetString(Resource.String.action_cancel),
                Prompt = ResourceProvider.GetString(Resource.String.prompt_max_concurrent_downloads_prompt),
                Value = ValueConverter.ConvertToString(controlFile.GetMaximumNumberOfConcurrentDownloads()),
                IsNumeric = true,
            };
            Observables.PromptForMaxConcurrentDownloads?.Invoke(this, promptParams);
        }

        public void SetMaxConcurrentDownloads(string value)
        {
            Logger.Debug(() => $"GlobalValuesViewModel:SetMaxConcurrentDownloads = {value}");
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            var number = ValueConverter.ConvertStringToInt(value);
            if (number < 1)
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.bad_number_need_positive));
            } else
            {
                controlFile.SetMaximumNumberOfConcurrentDownloads(number);
                ApplicationControlFileProvider.SaveCurrentControlFile();
            }
        }

        public void DoRetrySecondsOptions()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            ValuePromptDialogFragment.ValuePromptDialogFragmentParameters promptParams = new ValuePromptDialogFragment.ValuePromptDialogFragmentParameters()
            {
                Title = ResourceProvider.GetString(Resource.String.prompt_retry_seconds_title),
                Ok = ResourceProvider.GetString(Resource.String.action_ok),
                Cancel = ResourceProvider.GetString(Resource.String.action_cancel),
                Prompt = ResourceProvider.GetString(Resource.String.prompt_retry_seconds_title),
                Value = ValueConverter.ConvertToString(controlFile.GetRetryWaitInSeconds()),
                IsNumeric = true,
            };
            Observables.PromptForRetrySeconds?.Invoke(this, promptParams);
        }

        public void SetRetrySeconds(string value)
        {
            Logger.Debug(() => $"GlobalValuesViewModel:SetRetrySeconds = {value}");
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            var number = ValueConverter.ConvertStringToInt(value);
            if (number < 1)
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.bad_number_need_positive));
            }
            else
            {
                controlFile.SetRetryWaitInSeconds(number);
                ApplicationControlFileProvider.SaveCurrentControlFile();
            }
        }

        public List<SelectableString> GetPlaylistFormatOptions()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            List<SelectableString> options = new List<SelectableString>()
            {
                SelectableString.GenerateOption(PlaylistFormat.ASX, controlFile.GetPlaylistFormat()),
                SelectableString.GenerateOption(PlaylistFormat.M3U, controlFile.GetPlaylistFormat()),
                SelectableString.GenerateOption(PlaylistFormat.WPL, controlFile.GetPlaylistFormat()),
            };
            return options;
        }

        public void DoPlaylistFormatOption(SelectableString item)
        {
            Logger.Debug(() => $"GlobalValuesViewModel:DoPlaylistFormatOption = {item.Id}, {item.Name}");
            var format = (PlaylistFormat)item.Id;
            var ControlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            ControlFile.SetPlaylistFormat(format);
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }

        public void PlaylistSeperatorOptions()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            ValuePromptDialogFragment.ValuePromptDialogFragmentParameters promptParams = new ValuePromptDialogFragment.ValuePromptDialogFragmentParameters()
            {
                Title = ResourceProvider.GetString(Resource.String.prompt_playlist_seperator_title),
                Ok = ResourceProvider.GetString(Resource.String.action_ok),
                Cancel = ResourceProvider.GetString(Resource.String.action_cancel),
                Prompt = ResourceProvider.GetString(Resource.String.prompt_playlist_seperator_prompt),
                Value = controlFile.GetPlaylistPathSeparator(),
            };
            Observables.PromptForPlaylistSeperator?.Invoke(this, promptParams);
        }

        public void SetPlaylistSeperator(string value)
        {
            Logger.Debug(() => $"GlobalValuesViewModel:SetPlaylistSeperator = {value}");
            var ControlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            ControlFile.SetPlaylistPathSeparator(value);
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }

        public List<SelectableString> GetDiagOutputOptions()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            List<SelectableString> options = new List<SelectableString>()
            {
                SelectableString.GenerateOption(DiagnosticOutputLevel.Verbose, controlFile.GetDiagnosticOutput()),
                SelectableString.GenerateOption(DiagnosticOutputLevel.None, controlFile.GetDiagnosticOutput())
            };
            return options;
        }

        public void DoDiagOutputOption(SelectableString item)
        {
            Logger.Debug(() => $"GlobalValuesViewModel:DoDiagOutputOption = {item.Id}, {item.Name}");
            var level = (DiagnosticOutputLevel)item.Id;
            // we also need the event to fire
            ApplicationControlFileProvider.SetDiagnosticOutput(level);
        }

        public void DiagRetainTempOptions()
        {
            Logger.Debug(() => $"GlobalValuesViewModel:DiagRetainTempOptions");
            var ControlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            // we toggle
            ControlFile.SetDiagnosticRetainTemporaryFiles(!ControlFile.GetDiagnosticRetainTemporaryFiles());
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }
    }

}
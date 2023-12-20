using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Exceptions;
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
            public EventHandler<string> DownloadStrategy;
            public EventHandler<string> NamingStyle;
            public EventHandler<string> MaxDaysOld;
            public EventHandler<DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters> PromptForMaxDaysOld;
            public EventHandler<string> DeleteDaysOld;
            public EventHandler<DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters> PromptForDeleteDaysOld;
            public EventHandler<string> MaxDownloadItems;
            public EventHandler<DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters> PromptForMaxDownloadItems;
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

        public FeedDefaultsViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            ICrashReporter crashReporter,
            IAnalyticsEngine analyticsEngine,
            IFileSystemHelper fileSystemHelper,
            IApplicationControlFileFactory applicationControlFileFactory,
            IValueConverter valueConverter) : base(app)
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
            ValueConverter = valueConverter;
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

            var downloadStrategySubLabel = controlFile.GetDefaultDownloadStrategy().ToString();
            downloadStrategySubLabel += " - ";
            switch (controlFile.GetDefaultDownloadStrategy())
            {
                case PodcastEpisodeDownloadStrategy.All:
                    downloadStrategySubLabel += ResourceProvider.GetString(Resource.String.download_strategy_all);
                    break;
                case PodcastEpisodeDownloadStrategy.HighTide:
                    downloadStrategySubLabel += ResourceProvider.GetString(Resource.String.download_strategy_high_tide);
                    break;
                case PodcastEpisodeDownloadStrategy.Latest:
                    downloadStrategySubLabel += ResourceProvider.GetString(Resource.String.download_strategy_latest);
                    break;
            }
            Observables.DownloadStrategy?.Invoke(this, downloadStrategySubLabel);

            var namingStrategySubLabel = controlFile.GetDefaultNamingStyle().ToString();
            namingStrategySubLabel += " - ";
            switch (controlFile.GetDefaultNamingStyle())
            {
                case PodcastEpisodeNamingStyle.UrlFileName:
                    namingStrategySubLabel += ResourceProvider.GetString(Resource.String.naming_style_UrlFileName);
                    break;
                case PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime:
                    namingStrategySubLabel += ResourceProvider.GetString(Resource.String.naming_style_UrlFileNameAndPublishDateTime);
                    break;
                case PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTime:
                    namingStrategySubLabel += ResourceProvider.GetString(Resource.String.naming_style_UrlFileNameFeedTitleAndPublishDateTime);
                    break;
                case PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTimeInfolder:
                    namingStrategySubLabel += ResourceProvider.GetString(Resource.String.naming_style_UrlFileNameFeedTitleAndPublishDateTimeInfolder);
                    break;
                case PodcastEpisodeNamingStyle.EpisodeTitle:
                    namingStrategySubLabel += ResourceProvider.GetString(Resource.String.naming_style_EpisodeTitle);
                    break;
                case PodcastEpisodeNamingStyle.EpisodeTitleAndPublishDateTime:
                    namingStrategySubLabel += ResourceProvider.GetString(Resource.String.naming_style_EpisodeTitleAndPublishDateTime);
                    break;
            }
            Observables.NamingStyle?.Invoke(this, namingStrategySubLabel);

            if (controlFile.GetDefaultMaximumDaysOld() == int.MaxValue)
            {
                Observables.MaxDaysOld?.Invoke(this, ResourceProvider.GetString(Resource.String.prompt_max_days_old_named_prompt));
            }
            else
            {
                var maxDaysOldSublabel = string.Format(ResourceProvider.GetString(Resource.String.max_days_old_label_fmt), controlFile.GetDefaultMaximumDaysOld());
                Observables.MaxDaysOld?.Invoke(this, maxDaysOldSublabel);
            }

            if (controlFile.GetDefaultDeleteDownloadsDaysOld() == int.MaxValue)
            {
                Observables.DeleteDaysOld?.Invoke(this, ResourceProvider.GetString(Resource.String.prompt_delete_days_old_named_prompt));
            }
            else
            {
                var maxDaysOldSublabel = string.Format(ResourceProvider.GetString(Resource.String.delete_days_old_label_fmt), controlFile.GetDefaultDeleteDownloadsDaysOld());
                Observables.DeleteDaysOld?.Invoke(this, maxDaysOldSublabel);
            }

            if (controlFile.GetDefaultMaximumNumberOfDownloadedItems() == int.MaxValue)
            {
                Observables.MaxDownloadItems?.Invoke(this, ResourceProvider.GetString(Resource.String.prompt_max_download_items_named_prompt));
            }
            else
            {
                var maxDaysOldSublabel = string.Format(ResourceProvider.GetString(Resource.String.max_download_items_label_fmt), controlFile.GetDefaultMaximumNumberOfDownloadedItems());
                Observables.MaxDownloadItems?.Invoke(this, maxDaysOldSublabel);
            }
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
                SelectableString.GenerateOption(PodcastEpisodeDownloadStrategy.HighTide, controlFile.GetDefaultDownloadStrategy()),
                SelectableString.GenerateOption(PodcastEpisodeDownloadStrategy.All, controlFile.GetDefaultDownloadStrategy()),
                SelectableString.GenerateOption(PodcastEpisodeDownloadStrategy.Latest, controlFile.GetDefaultDownloadStrategy()),
            };
            return options;
        }

        public void DoDownloadStrategyOption(SelectableString item)
        {
            Logger.Debug(() => $"FeedDefaultsViewModel:DoDownloadStrategyOption = {item.Id}, {item.Name}");
            var strategy = (PodcastEpisodeDownloadStrategy)item.Id;
            var ControlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            ControlFile.SetDefaultDownloadStrategy(strategy);
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }

        public List<SelectableString> GetNamingStyleOptions()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            List<SelectableString> options = new List<SelectableString>()
            {
                SelectableString.GenerateOption(PodcastEpisodeNamingStyle.EpisodeTitle, controlFile.GetDefaultNamingStyle()),
                SelectableString.GenerateOption(PodcastEpisodeNamingStyle.EpisodeTitleAndPublishDateTime, controlFile.GetDefaultNamingStyle()),
                SelectableString.GenerateOption(PodcastEpisodeNamingStyle.UrlFileName, controlFile.GetDefaultNamingStyle()),
                SelectableString.GenerateOption(PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime, controlFile.GetDefaultNamingStyle()),
                SelectableString.GenerateOption(PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTime, controlFile.GetDefaultNamingStyle()),
                SelectableString.GenerateOption(PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTimeInfolder, controlFile.GetDefaultNamingStyle()),
            };
            return options;
        }

        public void DoNamingStyleOption(SelectableString item)
        {
            Logger.Debug(() => $"FeedDefaultsViewModel:DoNamingStyleOption = {item.Id}, {item.Name}");
            var style = (PodcastEpisodeNamingStyle)item.Id;
            var ControlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            ControlFile.SetDefaultNamingStyle(style);
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }

        public void MaxDaysOldOptions()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters promptParams = new DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters()
            {
                Title = ResourceProvider.GetString(Resource.String.prompt_max_days_old_title),
                Ok = ResourceProvider.GetString(Resource.String.action_ok),
                Cancel = ResourceProvider.GetString(Resource.String.action_cancel),
                NamedPrompt = ResourceProvider.GetString(Resource.String.prompt_max_days_old_named_prompt),
                CustomPrompt = ResourceProvider.GetString(Resource.String.prompt_max_days_old_custom_prompt),
                CurrentValue = ValueConverter.ConvertToString(controlFile.GetDefaultMaximumDaysOld()),
                NamedValue = int.MaxValue.ToString(),
                IsNumeric = true,
            };
            if (controlFile.GetDefaultMaximumDaysOld() == int.MaxValue)
            {
                promptParams.ValueType = DefaultableItemValuePromptDialogFragment.ItemValueType.Named;
            }
            else
            {
                promptParams.ValueType = DefaultableItemValuePromptDialogFragment.ItemValueType.Custom;
            }
            Observables.PromptForMaxDaysOld?.Invoke(this, promptParams);
        }

        public void SetMaxDaysOld(string value, DefaultableItemValuePromptDialogFragment.ItemValueType valueType)
        {
            Logger.Debug(() => $"FeedDefaultsViewModel:SetMaxDaysOld = {value}");
            var ControlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            // this is a global it cannot be defaulted
            switch (valueType)
            {
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Defaulted:
                    CrashReporter.LogNonFatalException(new ConfigurationException("Defaulted value type is not allowed here"));
                    break;
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Named:
                    ControlFile.SetDefaultMaximumDaysOld(int.MaxValue);
                    break;
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Custom:
                    ControlFile.SetDefaultMaximumDaysOld(ValueConverter.ConvertStringToInt(value));
                    break;
            }
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }

        public void MaxMaxDownloadItemsOptions()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters promptParams = new DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters()
            {
                Title = ResourceProvider.GetString(Resource.String.prompt_max_download_items_title),
                Ok = ResourceProvider.GetString(Resource.String.action_ok),
                Cancel = ResourceProvider.GetString(Resource.String.action_cancel),
                NamedPrompt = ResourceProvider.GetString(Resource.String.prompt_max_download_items_named_prompt),
                CustomPrompt = ResourceProvider.GetString(Resource.String.prompt_max_download_items_custom_prompt),
                CurrentValue = ValueConverter.ConvertToString(controlFile.GetDefaultMaximumNumberOfDownloadedItems()),
                NamedValue = int.MaxValue.ToString(),
                IsNumeric = true,
            };
            if (controlFile.GetDefaultMaximumNumberOfDownloadedItems() == int.MaxValue)
            {
                promptParams.ValueType = DefaultableItemValuePromptDialogFragment.ItemValueType.Named;
            }
            else
            {
                promptParams.ValueType = DefaultableItemValuePromptDialogFragment.ItemValueType.Custom;
            }
            Observables.PromptForMaxDownloadItems?.Invoke(this, promptParams);
        }

        public void SetMaxDownloadItems(string value, DefaultableItemValuePromptDialogFragment.ItemValueType valueType)
        {
            Logger.Debug(() => $"FeedDefaultsViewModel:SetMaxDownloadItems = {value}");
            var ControlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            // this is a global it cannot be defaulted
            switch (valueType)
            {
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Defaulted:
                    CrashReporter.LogNonFatalException(new ConfigurationException("Defaulted value type is not allowed here"));
                    break;
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Named:
                    ControlFile.SetDefaultMaximumNumberOfDownloadedItems(int.MaxValue);
                    break;
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Custom:
                    ControlFile.SetDefaultMaximumNumberOfDownloadedItems(ValueConverter.ConvertStringToInt(value));
                    break;
            }
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }

        public void DeleteDownloadDaysOldOptions()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters promptParams = new DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters()
            {
                Title = ResourceProvider.GetString(Resource.String.prompt_delete_days_old_title),
                Ok = ResourceProvider.GetString(Resource.String.action_ok),
                Cancel = ResourceProvider.GetString(Resource.String.action_cancel),
                NamedPrompt = ResourceProvider.GetString(Resource.String.prompt_delete_days_old_named_prompt),
                CustomPrompt = ResourceProvider.GetString(Resource.String.prompt_delete_days_old_custom_prompt),
                CurrentValue = ValueConverter.ConvertToString(controlFile.GetDefaultDeleteDownloadsDaysOld()),
                NamedValue = int.MaxValue.ToString(),
                IsNumeric = true,
            };
            if (controlFile.GetDefaultDeleteDownloadsDaysOld() == int.MaxValue)
            {
                promptParams.ValueType = DefaultableItemValuePromptDialogFragment.ItemValueType.Named;
            }
            else
            {
                promptParams.ValueType = DefaultableItemValuePromptDialogFragment.ItemValueType.Custom;
            }
            Observables.PromptForDeleteDaysOld?.Invoke(this, promptParams);
        }

        public void SetDeleteDaysOld(string value, DefaultableItemValuePromptDialogFragment.ItemValueType valueType)
        {
            Logger.Debug(() => $"FeedDefaultsViewModel:SetDeleteDaysOld = {value}");
            var ControlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            // this is a global it cannot be defaulted
            switch (valueType)
            {
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Defaulted:
                    CrashReporter.LogNonFatalException(new ConfigurationException("Defaulted value type is not allowed here"));
                    break;
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Named:
                    ControlFile.SetDefaultDeleteDownloadsDaysOld(int.MaxValue);
                    break;
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Custom:
                    ControlFile.SetDefaultDeleteDownloadsDaysOld(ValueConverter.ConvertStringToInt(value));
                    break;
            }
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }

    }
}
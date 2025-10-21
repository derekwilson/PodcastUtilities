using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Converter;
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
            public EventHandler<string>? DisplayMessage;
            public EventHandler<string>? DownloadStrategy;
            public EventHandler<string>? NamingStyle;
            public EventHandler<string>? MaxDaysOld;
            public EventHandler<DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters>? PromptForMaxDaysOld;
            public EventHandler<string>? DeleteDaysOld;
            public EventHandler<DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters>? PromptForDeleteDaysOld;
            public EventHandler<string>? MaxDownloadItems;
            public EventHandler<DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters>? PromptForMaxDownloadItems;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IApplicationControlFileProvider ApplicationControlFileProvider;
        private ICrashReporter CrashReporter;
        private IValueConverter ValueConverter;
        private IValueFormatter ValueFormatter;

        public FeedDefaultsViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            ICrashReporter crashReporter,
            IValueConverter valueConverter,
            IValueFormatter valueFormatter) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"FeedDefaultsViewModel:ctor");

            ApplicationContext = app;
            ResourceProvider = resProvider;
            ApplicationControlFileProvider = appControlFileProvider;
            CrashReporter = crashReporter;
            ValueConverter = valueConverter;
            ValueFormatter = valueFormatter;
        }

        private void ConfigurationUpdated(object? sender, EventArgs e)
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

            Observables.DownloadStrategy?.Invoke(this, ValueFormatter.GetDownloadStratagyTextLong(controlFile.GetDefaultDownloadStrategy()));
            Observables.NamingStyle?.Invoke(this, ValueFormatter.GetNamingStyleTextLong(controlFile.GetDefaultNamingStyle()));

            Observables.MaxDaysOld?.Invoke(this, ValueFormatter.GetCustomOrNamedIntValue(
                Resource.String.prompt_max_days_old_named_prompt,
                int.MaxValue,
                Resource.String.max_days_old_label_fmt,
                controlFile.GetDefaultMaximumDaysOld()
                )
            );

            Observables.DeleteDaysOld?.Invoke(this, ValueFormatter.GetCustomOrNamedIntValue(
                Resource.String.prompt_delete_days_old_named_prompt,
                int.MaxValue,
                Resource.String.delete_days_old_label_fmt,
                controlFile.GetDefaultDeleteDownloadsDaysOld()
                )
            );

            Observables.MaxDownloadItems?.Invoke(this, ValueFormatter.GetCustomOrNamedIntValue(
                Resource.String.prompt_max_download_items_named_prompt,
                int.MaxValue,
                Resource.String.max_download_items_label_fmt,
                controlFile.GetDefaultMaximumNumberOfDownloadedItems()
                )
            );
        }

        [Lifecycle.Event.OnCreate]
        [Java.Interop.Export]
        public void OnCreate()
        {
            Logger.Debug(() => $"FeedDefaultsViewModel:OnCreate");
            ApplicationControlFileProvider.ConfigurationUpdated += ConfigurationUpdated;
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

        public void MaxDownloadItemsOptions()
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
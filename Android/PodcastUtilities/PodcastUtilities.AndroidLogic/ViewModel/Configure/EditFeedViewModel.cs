﻿using Android.App;
using Android.Content;
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
    public class EditFeedViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> Title;
            public EventHandler<string> DisplayMessage;
            public EventHandler<string> NavigateToDownload;
            public EventHandler<string> Folder;
            public EventHandler<ValuePromptDialogFragment.ValuePromptDialogFragmentParameters> PromptForFolder;
            public EventHandler<string> Url;
            public EventHandler<ValuePromptDialogFragment.ValuePromptDialogFragmentParameters> PromptForUrl;
            public EventHandler<Tuple<bool, string>> DownloadStrategy;
            public EventHandler<Tuple<bool, string>> NamingStyle;
            public EventHandler<Tuple<bool, string>> MaxDaysOld;
            public EventHandler<DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters> PromptForMaxDaysOld;
            public EventHandler<Tuple<bool, string>> DeleteDaysOld;
            public EventHandler<DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters> PromptForDeleteDaysOld;
            public EventHandler<Tuple<bool, string>> MaxDownloadItems;
            public EventHandler<DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters> PromptForMaxDownloadItems;
            public EventHandler<Tuple<string, string, string, string>> DeletePrompt;
            public EventHandler Exit;
            public EventHandler<Tuple<string, Intent>> DisplayChooser;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IApplicationControlFileProvider ApplicationControlFileProvider;
        private ICrashReporter CrashReporter;
        private IValueConverter ValueConverter;
        private IValueFormatter ValueFormatter;
        private IAnalyticsEngine AnalyticsEngine;

        private int PodcastFeedToEditId = -1;

        public EditFeedViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            ICrashReporter crashReporter,
            IValueConverter valueConverter,
            IValueFormatter valueFormatter,
            IAnalyticsEngine analyticsEngine) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"EditFeedViewModel:ctor");

            ApplicationContext = app;
            ResourceProvider = resProvider;
            ApplicationControlFileProvider = appControlFileProvider;
            CrashReporter = crashReporter;
            ValueConverter = valueConverter;
            ValueFormatter = valueFormatter;
            AnalyticsEngine = analyticsEngine;
        }

        private void ConfigurationUpdated(object sender, EventArgs e)
        {
            Logger.Debug(() => $"EditFeedViewModel:ConfigurationUpdated");
            RefreshConfigDisplay();
        }

        public void Initialise(string id)
        {
            Logger.Debug(() => $"EditFeedViewModel:Initialise {id}");
            PodcastFeedToEditId = Convert.ToInt32(id);
            RefreshConfigDisplay();
        }

        private IPodcastInfo GetFeedToEdit()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            int index = 0;
            foreach (var podcastInfo in controlFile.GetPodcasts())
            {
                if (index == PodcastFeedToEditId)
                {
                    if (podcastInfo.Feed == null)
                    {
                        Logger.Debug(() => $"EditFeedViewModel:GetFeedToEdit podcast has no feed {PodcastFeedToEditId}");
                        return null;
                    }
                    else
                    {
                        return podcastInfo;
                    }
                }
                index++;
            }
            Logger.Debug(() => $"EditFeedViewModel:GetFeedToEdit cannot find podcast {PodcastFeedToEditId}");
            return null;
        }

        private void RefreshConfigDisplay()
        {
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                CrashReporter.LogNonFatalException(new ConfigurationException($"Cannot find podcast {PodcastFeedToEditId}"));
                return;
            }
            Observables.Title?.Invoke(this, feed.Folder);
            Observables.Folder?.Invoke(this, feed.Folder);
            Observables.Url?.Invoke(this, feed.Feed.Address.ToString());

            Observables.DownloadStrategy?.Invoke(this, Tuple.Create(feed.Feed.DownloadStrategy.IsSet,
                                                    ValueFormatter.GetDefaultableDownloadStratagyTextLong(feed.Feed.DownloadStrategy))
                );
            Observables.NamingStyle?.Invoke(this, Tuple.Create(feed.Feed.NamingStyle.IsSet,
                                                    ValueFormatter.GetDefaultableNamingStyleTextLong(feed.Feed.NamingStyle))
                );

            Observables.MaxDaysOld?.Invoke(this, Tuple.Create(feed.Feed.MaximumDaysOld.IsSet,
                                                    ValueFormatter.GetDefaultableCustomOrNamedIntValue(
                                                        Resource.String.prompt_max_days_old_named_prompt,
                                                        int.MaxValue,
                                                        Resource.String.max_days_old_label_fmt,
                                                        feed.Feed.MaximumDaysOld
                                                        )
                                                    )
            );

            Observables.DeleteDaysOld?.Invoke(this, Tuple.Create(feed.Feed.DeleteDownloadsDaysOld.IsSet,
                                                    ValueFormatter.GetDefaultableCustomOrNamedIntValue(
                                                        Resource.String.prompt_delete_days_old_named_prompt,
                                                        int.MaxValue,
                                                        Resource.String.delete_days_old_label_fmt,
                                                        feed.Feed.DeleteDownloadsDaysOld
                                                        )
                                                    )
            );

            Observables.MaxDownloadItems?.Invoke(this, Tuple.Create(feed.Feed.MaximumNumberOfDownloadedItems.IsSet,
                                                    ValueFormatter.GetDefaultableCustomOrNamedIntValue(
                                                        Resource.String.prompt_max_download_items_named_prompt,
                                                        int.MaxValue,
                                                        Resource.String.max_download_items_label_fmt,
                                                        feed.Feed.MaximumNumberOfDownloadedItems
                                                        )
                                                    )
            );
        }

        [Lifecycle.Event.OnCreate]
        [Java.Interop.Export]
        public void OnCreate()
        {
            Logger.Debug(() => $"EditFeedViewModel:OnCreate");
            ApplicationControlFileProvider.ConfigurationUpdated += ConfigurationUpdated;
        }

        [Lifecycle.Event.OnDestroy]
        [Java.Interop.Export]
        public void OnDestroy()
        {
            Logger.Debug(() => $"EditFeedViewModel:OnDestroy");
            ApplicationControlFileProvider.ConfigurationUpdated -= ConfigurationUpdated;
        }


        public bool IsActionAvailable(int itemId)
        {
            Logger.Debug(() => $"EditFeedViewModel:isActionAvailable = {itemId}");
            if (itemId == Resource.Id.action_test_feed)
            {
                var feed = GetFeedToEdit();
                if (feed != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ActionSelected(int itemId)
        {
            Logger.Debug(() => $"EditFeedViewModel:ActionSelected = {itemId}");
            if (itemId == Resource.Id.action_test_feed)
            {
                var feed = GetFeedToEdit();
                if (feed != null)
                {
                    Logger.Debug(() => $"EditFeedViewModel: FeedItemSelected {feed.Folder}");
                    Observables.NavigateToDownload?.Invoke(this, feed.Folder);
                    return true;
                }
            }
            return false;
        }

        private string AddDefaultPrefix(string str)
        {
            return $"{ResourceProvider.GetString(Resource.String.feed_uses_default_prefix)} {str}";
        }

        private const int OPTION_ID_USE_DEFAULT = -1;

        public List<SelectableString> GetDownloadStrategyOptions()
        {
            List<SelectableString> options = new List<SelectableString>();
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return options;
            }
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            if (feed.Feed.DownloadStrategy.IsSet)
            {
                options.Add(new SelectableString(OPTION_ID_USE_DEFAULT, AddDefaultPrefix(controlFile.GetDefaultDownloadStrategy().ToString())));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeDownloadStrategy.HighTide, feed.Feed.DownloadStrategy.Value));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeDownloadStrategy.All, feed.Feed.DownloadStrategy.Value));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeDownloadStrategy.Latest, feed.Feed.DownloadStrategy.Value));
            }
            else
            {
                // we are using the default
                options.Add(new SelectableString(OPTION_ID_USE_DEFAULT, AddDefaultPrefix(controlFile.GetDefaultDownloadStrategy().ToString()), true));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeDownloadStrategy.HighTide, false));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeDownloadStrategy.All, false));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeDownloadStrategy.Latest, false));
            }
            return options;
        }

        public void DoDownloadStrategyOption(SelectableString item)
        {
            Logger.Debug(() => $"EditFeedViewModel:DoDownloadStrategyOption = {item.Id}, {item.Name}");
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return;
            }
            if (item.Id == OPTION_ID_USE_DEFAULT)
            {
                feed.Feed.DownloadStrategy.RevertToDefault();
            }
            else
            {
                var strategy = (PodcastEpisodeDownloadStrategy)item.Id;
                feed.Feed.DownloadStrategy.Value = strategy;
            }
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }

        public List<SelectableString> GetNamingStyleOptions()
        {
            List<SelectableString> options = new List<SelectableString>();
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return options;
            }
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            if (feed.Feed.NamingStyle.IsSet)
            {
                options.Add(new SelectableString(OPTION_ID_USE_DEFAULT, AddDefaultPrefix(controlFile.GetDefaultNamingStyle().ToString())));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeNamingStyle.EpisodeTitle, feed.Feed.NamingStyle.Value));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeNamingStyle.EpisodeTitleAndPublishDateTime, feed.Feed.NamingStyle.Value));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeNamingStyle.UrlFileName, feed.Feed.NamingStyle.Value));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime, feed.Feed.NamingStyle.Value));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTime, feed.Feed.NamingStyle.Value));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTimeInfolder, feed.Feed.NamingStyle.Value));

            }
            else
            {
                // we are using the default
                options.Add(new SelectableString(OPTION_ID_USE_DEFAULT, AddDefaultPrefix(controlFile.GetDefaultNamingStyle().ToString()), true));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeNamingStyle.EpisodeTitle, false));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeNamingStyle.EpisodeTitleAndPublishDateTime, false));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeNamingStyle.UrlFileName, false));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime, false));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTime, false));
                options.Add(SelectableString.GenerateOption(PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTimeInfolder, false));
            }
            return options;
        }
        public void DoNamingStyleOption(SelectableString item)
        {
            Logger.Debug(() => $"EditFeedViewModel:DoNamingStyleOption = {item.Id}, {item.Name}");
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return;
            }
            if (item.Id == OPTION_ID_USE_DEFAULT)
            {
                feed.Feed.NamingStyle.RevertToDefault();
            }
            else
            {
                var strategy = (PodcastEpisodeNamingStyle)item.Id;
                feed.Feed.NamingStyle.Value = strategy;
            }
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }

        public void MaxDaysOldOptions()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return;
            }
            var defaultPrompt = ValueFormatter.GetCustomOrNamedIntValue(
                Resource.String.prompt_max_days_old_named_prompt,
                int.MaxValue,
                Resource.String.max_days_old_label_fmt,
                controlFile.GetDefaultMaximumDaysOld()
                );
            DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters promptParams = new DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters()
            {
                Title = ResourceProvider.GetString(Resource.String.prompt_max_days_old_title),
                Ok = ResourceProvider.GetString(Resource.String.action_ok),
                Cancel = ResourceProvider.GetString(Resource.String.action_cancel),
                DefaultPrompt = $"{ResourceProvider.GetString(Resource.String.feed_uses_default_prefix)} {defaultPrompt}",
                DefaultValue = ValueConverter.ConvertToString(controlFile.GetDefaultMaximumDaysOld()),
                NamedPrompt = ResourceProvider.GetString(Resource.String.prompt_max_days_old_named_prompt),
                CustomPrompt = ResourceProvider.GetString(Resource.String.prompt_max_days_old_custom_prompt),
                CurrentValue = ValueConverter.ConvertToString(feed.Feed.MaximumDaysOld.Value),
                NamedValue = int.MaxValue.ToString(),
                IsNumeric = true,
            };
            promptParams.ValueType = DefaultableItemValuePromptDialogFragment.GetIntItemType(feed.Feed.MaximumDaysOld, int.MaxValue);
            Observables.PromptForMaxDaysOld?.Invoke(this, promptParams);
        }

        public void SetMaxDaysOld(string value, DefaultableItemValuePromptDialogFragment.ItemValueType valueType)
        {
            Logger.Debug(() => $"EditFeedViewModel:SetMaxDaysOld = {value}");
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return;
            }
            switch (valueType)
            {
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Defaulted:
                    feed.Feed.MaximumDaysOld.RevertToDefault();
                    break;
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Named:
                    feed.Feed.MaximumDaysOld.Value = int.MaxValue;
                    break;
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Custom:
                    feed.Feed.MaximumDaysOld.Value = ValueConverter.ConvertStringToInt(value);
                    break;
            }
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }

        public void DeleteDownloadDaysOldOptions()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return;
            }
            var defaultPrompt = ValueFormatter.GetCustomOrNamedIntValue(
                Resource.String.prompt_delete_days_old_named_prompt,
                int.MaxValue,
                Resource.String.delete_days_old_label_fmt,
                controlFile.GetDefaultDeleteDownloadsDaysOld()
                );
            DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters promptParams = new DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters()
            {
                Title = ResourceProvider.GetString(Resource.String.prompt_delete_days_old_title),
                Ok = ResourceProvider.GetString(Resource.String.action_ok),
                Cancel = ResourceProvider.GetString(Resource.String.action_cancel),
                DefaultPrompt = $"{ResourceProvider.GetString(Resource.String.feed_uses_default_prefix)} {defaultPrompt}",
                DefaultValue = ValueConverter.ConvertToString(controlFile.GetDefaultDeleteDownloadsDaysOld()),
                NamedPrompt = ResourceProvider.GetString(Resource.String.prompt_delete_days_old_named_prompt),
                CustomPrompt = ResourceProvider.GetString(Resource.String.prompt_delete_days_old_custom_prompt),
                CurrentValue = ValueConverter.ConvertToString(feed.Feed.DeleteDownloadsDaysOld.Value),
                NamedValue = int.MaxValue.ToString(),
                IsNumeric = true,
            };
            promptParams.ValueType = DefaultableItemValuePromptDialogFragment.GetIntItemType(feed.Feed.DeleteDownloadsDaysOld, int.MaxValue);
            Observables.PromptForDeleteDaysOld?.Invoke(this, promptParams);
        }

        public void SetDeleteDaysOld(string value, DefaultableItemValuePromptDialogFragment.ItemValueType valueType)
        {
            Logger.Debug(() => $"EditFeedViewModel:SetDeleteDaysOld = {value}");
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return;
            }
            switch (valueType)
            {
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Defaulted:
                    feed.Feed.DeleteDownloadsDaysOld.RevertToDefault();
                    break;
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Named:
                    feed.Feed.DeleteDownloadsDaysOld.Value = int.MaxValue;
                    break;
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Custom:
                    feed.Feed.DeleteDownloadsDaysOld.Value = ValueConverter.ConvertStringToInt(value);
                    break;
            }
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }

        public void MaxDownloadItemsOptions()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return;
            }
            var defaultPrompt = ValueFormatter.GetCustomOrNamedIntValue(
                Resource.String.prompt_max_download_items_named_prompt,
                int.MaxValue,
                Resource.String.max_download_items_label_fmt,
                controlFile.GetDefaultMaximumNumberOfDownloadedItems()
                );
            DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters promptParams = new DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters()
            {
                Title = ResourceProvider.GetString(Resource.String.prompt_max_download_items_title),
                Ok = ResourceProvider.GetString(Resource.String.action_ok),
                Cancel = ResourceProvider.GetString(Resource.String.action_cancel),
                DefaultPrompt = $"{ResourceProvider.GetString(Resource.String.feed_uses_default_prefix)} {defaultPrompt}",
                DefaultValue = ValueConverter.ConvertToString(controlFile.GetDefaultMaximumNumberOfDownloadedItems()),
                NamedPrompt = ResourceProvider.GetString(Resource.String.prompt_max_download_items_named_prompt),
                CustomPrompt = ResourceProvider.GetString(Resource.String.prompt_max_download_items_custom_prompt),
                CurrentValue = ValueConverter.ConvertToString(feed.Feed.MaximumNumberOfDownloadedItems.Value),
                NamedValue = int.MaxValue.ToString(),
                IsNumeric = true,
            };
            promptParams.ValueType = DefaultableItemValuePromptDialogFragment.GetIntItemType(feed.Feed.MaximumNumberOfDownloadedItems, int.MaxValue);
            Observables.PromptForMaxDownloadItems?.Invoke(this, promptParams);
        }

        public void SetMaxDownloadItems(string value, DefaultableItemValuePromptDialogFragment.ItemValueType valueType)
        {
            Logger.Debug(() => $"EditFeedViewModel:SetMaxDownloadItems = {value}");
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return;
            }
            switch (valueType)
            {
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Defaulted:
                    feed.Feed.MaximumNumberOfDownloadedItems.RevertToDefault();
                    break;
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Named:
                    feed.Feed.MaximumNumberOfDownloadedItems.Value = int.MaxValue;
                    break;
                case DefaultableItemValuePromptDialogFragment.ItemValueType.Custom:
                    feed.Feed.MaximumNumberOfDownloadedItems.Value = ValueConverter.ConvertStringToInt(value);
                    break;
            }
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }

        public void UrlOptions()
        {
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return;
            }
            ValuePromptDialogFragment.ValuePromptDialogFragmentParameters promptParams = new ValuePromptDialogFragment.ValuePromptDialogFragmentParameters()
            {
                Title = ResourceProvider.GetString(Resource.String.prompt_url_title),
                Ok = ResourceProvider.GetString(Resource.String.action_ok),
                Cancel = ResourceProvider.GetString(Resource.String.action_cancel),
                Prompt = ResourceProvider.GetString(Resource.String.prompt_url_prompt),
                Value = feed.Feed.Address.ToString(),
            };
            Observables.PromptForUrl?.Invoke(this, promptParams);
        }

        public void SetUrl(string value)
        {
            Logger.Debug(() => $"EditFeedViewModel:SetUrl = {value}");
            var feed = GetFeedToEdit();
            if (feed == null || value == null)
            {
                return;
            }
            if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
            {
                feed.Feed.Address = new Uri(value);
                ApplicationControlFileProvider.SaveCurrentControlFile();
            }
            else
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.bad_url));
            }
        }

        public void FolderOptions()
        {
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return;
            }
            ValuePromptDialogFragment.ValuePromptDialogFragmentParameters promptParams = new ValuePromptDialogFragment.ValuePromptDialogFragmentParameters()
            {
                Title = ResourceProvider.GetString(Resource.String.prompt_folder_title),
                Ok = ResourceProvider.GetString(Resource.String.action_ok),
                Cancel = ResourceProvider.GetString(Resource.String.action_cancel),
                Prompt = ResourceProvider.GetString(Resource.String.prompt_folder_prompt),
                Value = feed.Folder,
            };
            Observables.PromptForFolder?.Invoke(this, promptParams);
        }

        public void SetFolder(string value)
        {
            Logger.Debug(() => $"EditFeedViewModel:SetFolder = {value}");
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.bad_folder_empty));
            }
            else if (!ApplicationControlFileProvider.SetFoldernameIfUnique(feed, value))
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.bad_folder_duplicate));
            }
        }

        public void RemovePodcastSelected()
        {
            Logger.Debug(() => $"EditFeedViewModel:RemovePodcastSelected");
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return;
            }
            Observables.DeletePrompt?.Invoke(this,
                Tuple.Create(
                    string.Format(ResourceProvider.GetString(Resource.String.prompt_delete_podcast_title_fmt), feed.Folder),
                    ResourceProvider.GetString(Resource.String.prompt_delete_podcast_prompt),
                    ResourceProvider.GetString(Resource.String.prompt_delete_podcast_ok),
                    ResourceProvider.GetString(Resource.String.action_cancel)
                )
            );
        }

        public void DeleteConfirmed()
        {
            // find the podcast
            Logger.Debug(() => $"EditFeedViewModel:DeleteConfirmed");
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return;
            }
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            Logger.Debug(() => $"EditFeedViewModel:DeleteConfirmed deleting {feed.Folder}");
            AnalyticsEngine.RemovePodcastEvent(feed.Folder);
            controlFile.DeletePodcast(feed);
            ApplicationControlFileProvider.SaveCurrentControlFile();
            Observables.Exit?.Invoke(this, null);
        }

        public void SharePodcastSelected()
        {
            Logger.Debug(() => $"EditFeedViewModel:SharePodcastSelected");
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return;
            }
            var subject = string.Format(ResourceProvider.GetString(Resource.String.share_feed_subject),
                feed.Folder
                );
            var body = string.Format(ResourceProvider.GetString(Resource.String.share_feed_body),
                feed.Folder,
                feed.Feed.Address,
                ResourceProvider.GetString(Resource.String.share_advert_app_url)
                );
            var intent = GetSharingIntent(subject, body);
            Observables.DisplayChooser?.Invoke(this, Tuple.Create(ResourceProvider.GetString(Resource.String.share_chooser_title), intent));
        }

        private Intent GetSharingIntent(string subject, string shareText)
        {
            Intent sharingIntent = new Intent(Intent.ActionSend);
            sharingIntent.SetType("text/plain");
            sharingIntent.PutExtra(Intent.ExtraSubject, subject);
            sharingIntent.PutExtra(Intent.ExtraText, shareText);
            return sharingIntent;
        }
    }
}
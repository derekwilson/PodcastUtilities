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
    public class EditFeedViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> DisplayMessage;
            public EventHandler<Tuple<bool, string>> DownloadStrategy;
            public EventHandler<Tuple<bool, string>> NamingStyle;
            public EventHandler<Tuple<bool, string>> MaxDaysOld;
            public EventHandler<DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters> PromptForMaxDaysOld;
            public EventHandler<Tuple<bool, string>> DeleteDaysOld;
            public EventHandler<DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters> PromptForDeleteDaysOld;
            public EventHandler<Tuple<bool, string>> MaxDownloadItems;
            public EventHandler<DefaultableItemValuePromptDialogFragment.DefaultableItemValuePromptDialogFragmentParameters> PromptForMaxDownloadItems;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IApplicationControlFileProvider ApplicationControlFileProvider;
        private ICrashReporter CrashReporter;
        private IValueConverter ValueConverter;
        private IValueFormatter ValueFormatter;

        private int PodcastFeedToEditId = -1;
        private string PodcastFeedToEditFolder = null;

        public EditFeedViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            ICrashReporter crashReporter,
            IValueConverter valueConverter,
            IValueFormatter valueFormatter) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"EditFeedViewModel:ctor");

            ApplicationContext = app;
            ResourceProvider = resProvider;
            ApplicationControlFileProvider = appControlFileProvider;
            CrashReporter = crashReporter;
            ValueConverter = valueConverter;
            ValueFormatter = valueFormatter;
        }

        private void ConfigurationUpdated(object sender, EventArgs e)
        {
            Logger.Debug(() => $"EditFeedViewModel:ConfigurationUpdated");
            RefreshConfigDisplay();
        }

        private IPodcastInfo GetFeedToEdit()
        {
            // TODO - fix this crappy code
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            int index = 0;
            foreach (var podcastInfo in controlFile.GetPodcasts())
            {
                if (index == PodcastFeedToEditId)
                {
                    if (podcastInfo.Folder == PodcastFeedToEditFolder)
                    {
                        return podcastInfo;
                    }
                }
                index++;
            }
            Logger.Debug(() => $"EditFeedViewModel:GetFeedToEdit cannot find podcast {PodcastFeedToEditId}, {PodcastFeedToEditFolder}");
            return null;
        }

        public void Initialise(string id, string folder)
        {
            Logger.Debug(() => $"EditFeedViewModel:Initialise {id}, {folder} ");
            PodcastFeedToEditId = Convert.ToInt32(id);
            PodcastFeedToEditFolder = folder;
            RefreshConfigDisplay();
        }

        private void RefreshConfigDisplay()
        {
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                CrashReporter.LogNonFatalException(new ConfigurationException($"Cannot find podcast {PodcastFeedToEditId}, {PodcastFeedToEditFolder}"));
                return;
            }

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
    }
}
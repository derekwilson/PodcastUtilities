using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.CustomViews;
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
            Logger.Debug(() => $"EditFeedViewModel:GetFeedToEdit cannot find folder {PodcastFeedToEditId}, {PodcastFeedToEditFolder}");
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
                return;
            }

            Observables.DownloadStrategy?.Invoke(this, ValueFormatter.GetDownloadStratagyTextLong(feed.Feed.DownloadStrategy.Value));
            Observables.NamingStyle?.Invoke(this, ValueFormatter.GetNamingStyleTextLong(feed.Feed.NamingStyle.Value));

            Observables.MaxDaysOld?.Invoke(this, ValueFormatter.GetCustomOrNamedIntValue(
                Resource.String.prompt_max_days_old_named_prompt,
                int.MaxValue,
                Resource.String.max_days_old_label_fmt,
                feed.Feed.MaximumDaysOld.Value
                )
            );

            Observables.DeleteDaysOld?.Invoke(this, ValueFormatter.GetCustomOrNamedIntValue(
                Resource.String.prompt_delete_days_old_named_prompt,
                int.MaxValue,
                Resource.String.delete_days_old_label_fmt,
                feed.Feed.DeleteDownloadsDaysOld.Value
                )
            );

            Observables.MaxDownloadItems?.Invoke(this, ValueFormatter.GetCustomOrNamedIntValue(
                Resource.String.prompt_max_download_items_named_prompt,
                int.MaxValue,
                Resource.String.max_download_items_label_fmt,
                feed.Feed.MaximumNumberOfDownloadedItems.Value
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

        public List<SelectableString> GetDownloadStrategyOptions()
        {
            List<SelectableString> options = new List<SelectableString>();
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return options;
            }
            return options;
        }

        public List<SelectableString> GetNamingStyleOptions()
        {
            List<SelectableString> options = new List<SelectableString>();
            var feed = GetFeedToEdit();
            if (feed == null)
            {
                return options;
            }
            return options;
        }
    }
}
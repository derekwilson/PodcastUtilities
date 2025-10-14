using Android.App;
using Android.Content;
using Android.Views;
using AndroidX.DocumentFile.Provider;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PodcastUtilities.AndroidLogic.ViewModel.Configure
{
    public class EditConfigViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string>? DisplayMessage;
            public EventHandler<Tuple<string, Intent>>? DisplayChooser;
            public EventHandler<Tuple<string, string, string, string>>? ResetPrompt;
            public EventHandler<Tuple<string, string, string, string, string>>? DeletePrompt;
            public EventHandler? SelectFolder;
            public EventHandler? SelectControlFile;
            public EventHandler<string>? SetCacheRoot;
            public EventHandler<Tuple<string, List<ConfigPodcastFeedRecyclerItem>>>? SetFeedItems;
            public EventHandler<string>? NavigateToFeed;
            public EventHandler<ValuePromptDialogFragment.ValuePromptDialogFragmentParameters>? PromptToAddFeed;
            public EventHandler<ValuePromptDialogFragment.ValuePromptDialogFragmentParameters>? PromptForCacheRoot;
            public EventHandler? NavigateToAddFeed;
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
        private IValueFormatter ValueFormatter;

        private List<ConfigPodcastFeedRecyclerItem> AllFeedItems = new List<ConfigPodcastFeedRecyclerItem>(20);

        public EditConfigViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            ICrashReporter crashReporter,
            IAnalyticsEngine analyticsEngine,
            IFileSystemHelper fileSystemHelper,
            IApplicationControlFileFactory applicationControlFileFactory,
            IValueFormatter valueFormatter
            ) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"EditConfigViewModel:ctor");

            ApplicationContext = app;
            ResourceProvider = resProvider;
            ApplicationControlFileProvider = appControlFileProvider;
            CrashReporter = crashReporter;
            AnalyticsEngine = analyticsEngine;
            FileSystemHelper = fileSystemHelper;
            ApplicationControlFileFactory = applicationControlFileFactory;
            ValueFormatter = valueFormatter;
        }

        private void ConfigurationUpdated(object? sender, EventArgs e)
        {
            Logger.Debug(() => $"EditConfigViewModel:ConfigurationUpdated");
            RefreshConfigDisplay();
        }

        public void Initialise()
        {
            Logger.Debug(() => $"EditConfigViewModel:Initialise");
            RefreshConfigDisplay();
        }

        private void RefreshConfigDisplay()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            if (controlFile == null)
            {
                Logger.Debug(() => $"EditConfigViewModel:resetting control file");
                controlFile = ApplicationControlFileProvider.ResetControlFile();
            }

            if (controlFile != null)
            {
                var cacheRootSublabel = string.Format(ResourceProvider.GetString(Resource.String.cache_root_label_fmt), controlFile.GetSourceRoot());
                Observables.SetCacheRoot?.Invoke(this, cacheRootSublabel);

                AllFeedItems.Clear();
                int index = 0;
                foreach (var podcastInfo in controlFile.GetPodcasts())
                {
                    var item = new ConfigPodcastFeedRecyclerItem()
                    {
                        Id = index.ToString(),
                        PodcastFeed = podcastInfo
                    };
                    AllFeedItems.Add(item);
                    index++;
                }
                Logger.Debug(() => $"EditConfigViewModel:RefreshFeedList {AllFeedItems.Count}");
                var heading = ResourceProvider.GetQuantityString(Resource.Plurals.feed_list_heading, AllFeedItems.Count);
                Observables.SetFeedItems?.Invoke(this, Tuple.Create(heading, AllFeedItems));
            }
        }

        [Lifecycle.Event.OnCreate]
        [Java.Interop.Export]
        public void OnCreate()
        {
            Logger.Debug(() => $"EditConfigViewModel:OnCreate");
            ApplicationControlFileProvider.ConfigurationUpdated += ConfigurationUpdated;
        }

        [Lifecycle.Event.OnDestroy]
        [Java.Interop.Export]
        public void OnDestroy()
        {
            Logger.Debug(() => $"EditConfigViewModel:OnDestroy");
            ApplicationControlFileProvider.ConfigurationUpdated -= ConfigurationUpdated;
        }

        public bool KeyEvent(KeyEvent e)
        {
            Logger.Debug(() => $"EditConfigViewModel:KeyEvent = {e.Action}, {e.KeyCode}");
            if (e == null || e.Action != KeyEventActions.Up)
            {
                // lets get rid of most of the stuff we are not interested in
                return false;
            }
            switch (e.KeyCode)
            {
                case Keycode.L:
                    return DoIfPossible(Resource.Id.action_edit_load_control);
                case Keycode.S:
                    return DoIfPossible(Resource.Id.action_edit_share_control);
                case Keycode.R:
                    return DoIfPossible(Resource.Id.action_edit_reset_control);
            }
            return false;
        }

        private bool DoIfPossible(int itemId)
        {
            if (IsActionAvailable(itemId))
            {
                return ActionSelected(itemId);
            }
            return false;
        }

        public bool IsActionAvailable(int itemId)
        {
            Logger.Debug(() => $"EditConfigViewModel:isActionAvailable = {itemId}");
            if (itemId == Resource.Id.action_edit_load_control)
            {
                return true;
            }
            if (itemId == Resource.Id.action_edit_share_control)
            {
                return true;
            }
            if (itemId == Resource.Id.action_edit_reset_control)
            {
                return true;
            }
            return false;
        }

        public bool ActionSelected(int itemId)
        {
            Logger.Debug(() => $"EditConfigViewModel:ActionSelected = {itemId}");
            if (itemId == Resource.Id.action_edit_load_control)
            {
                Observables.SelectControlFile?.Invoke(this, EventArgs.Empty);
                return true;
            }
            if (itemId == Resource.Id.action_edit_share_control)
            {
                ShareConfig();
                return true;
            }
            if (itemId == Resource.Id.action_edit_reset_control)
            {
                ResetConfig();
                return true;
            }
            return false;
        }

        public void ShareConfig()
        {
            if (ApplicationControlFileProvider == null)
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.share_no_controlfile));
                return;
            }
            var intent = ApplicationControlFileProvider.GetApplicationConfigurationSharingIntent();
            if (intent != null)
            {
                var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
                AnalyticsEngine.ShareControlFileEvent(controlFile?.GetPodcasts().Count() ?? -1);
                Observables.DisplayChooser?.Invoke(this, Tuple.Create(ResourceProvider.GetString(Resource.String.share_chooser_title), intent));
            }
            else
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.share_no_controlfile));
            }
        }

        private void ResetConfig()
        {
            Observables.ResetPrompt?.Invoke(this,
                Tuple.Create(
                    ResourceProvider.GetString(Resource.String.dialog_title),
                    ResourceProvider.GetString(Resource.String.edit_reset_prompt),
                    ResourceProvider.GetString(Resource.String.edit_reset_ok),
                    ResourceProvider.GetString(Resource.String.edit_reset_cancel)
                )
            );
        }

        public void ResetConfirmed()
        {
            if (ApplicationControlFileProvider == null)
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.share_no_controlfile));
                return;
            }
            ApplicationControlFileProvider.ResetControlFile();
            AnalyticsEngine.ResetControlFileEvent();
            Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.edit_reset));
        }

        public void FolderSelected(DocumentFile file)
        {
            var folder = FileSystemHelper.GetRealPathFromDocumentTreeFile(file);
            SetCacheRoot(folder);
        }

        private void SetCacheRoot(string root)
        {
            Logger.Debug(() => $"EditConfigViewModel:SetCacheRoot = {root}");
            var ControlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            ControlFile.SetSourceRoot(root);
            ApplicationControlFileProvider.SaveCurrentControlFile();
        }

        public void LoadContolFile(Android.Net.Uri data)
        {
            var controlFile = OpenControlFile(data);
            if (controlFile != null)
            {
                var items = ApplicationControlFileProvider.ReplaceApplicationConfiguration(controlFile);
                AnalyticsEngine.LoadControlFileEvent(items);
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.control_file_loaded));
            }
        }

        private IReadWriteControlFile? OpenControlFile(Android.Net.Uri uri)
        {
            try
            {
                return ApplicationControlFileFactory.CreateControlFile(FileSystemHelper.LoadXmlFromContentUri(uri));
            }
            catch (Exception ex)
            {
                Logger.LogException(() => $"EditConfigViewModel:OpenControlFile", ex);
                CrashReporter.LogNonFatalException(ex);
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.error_reading_control_file));
                return null;
            }
        }

        public List<SelectableString> GetCacheRootOptions()
        {
            List<SelectableString> options = new List<SelectableString>()
            {
                new SelectableString(OPTION_ID_PHONE_ROOT, ResourceProvider.GetString(Resource.String.cache_root_option_phone)),
                new SelectableString(OPTION_ID_WSA_ROOT, ResourceProvider.GetString(Resource.String.cache_root_option_wsa)),
            };
            // add private folders (for SD card its all we can do)
            int index = 0;
            Java.IO.File[]? files = FileSystemHelper.GetApplicationExternalFilesDirs();
            if (files != null)
            {
                foreach (Java.IO.File file in files)
                {
                    Logger.Debug(() => $"ExternalFile = {file.AbsolutePath}");
                    options.Add(new SelectableString(OPTION_ID_PRIVATE_ROOT + index, file.AbsolutePath));
                    index++;
                }
            }
            options.Add(new SelectableString(OPTION_ID_SELECT_FOLDER, ResourceProvider.GetString(Resource.String.cache_root_option_select)));
            options.Add(new SelectableString(OPTION_ID_CUSTOM, ResourceProvider.GetString(Resource.String.cache_root_option_custom)));
            return options;
        }

        public void DoCacheRootOption(SelectableString item)
        {
            Logger.Debug(() => $"EditConfigViewModel:DoCacheRootOption = {item.Id}, {item.Name}");
            switch (item.Id)
            {
                case OPTION_ID_SELECT_FOLDER:
                    Observables.SelectFolder?.Invoke(this, EventArgs.Empty);
                    break;
                case OPTION_ID_CUSTOM:
                    var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
                    ValuePromptDialogFragment.ValuePromptDialogFragmentParameters promptParams = new ValuePromptDialogFragment.ValuePromptDialogFragmentParameters()
                    {
                        Title = ResourceProvider.GetString(Resource.String.prompt_cache_root_title),
                        Ok = ResourceProvider.GetString(Resource.String.action_ok),
                        Cancel = ResourceProvider.GetString(Resource.String.action_cancel),
                        Prompt = ResourceProvider.GetString(Resource.String.prompt_cache_root_prompt),
                        Value = controlFile.GetSourceRoot(),
                    };
                    Observables.PromptForCacheRoot?.Invoke(this, promptParams);
                    break;
                case OPTION_ID_PHONE_ROOT:
                case OPTION_ID_WSA_ROOT:
                case OPTION_ID_PRIVATE_ROOT:
                default:
                    SetCacheRoot(item.Name);
                    break;
            }
        }

        private const int OPTION_ID_SELECT_FOLDER = 10;
        private const int OPTION_ID_PHONE_ROOT = 11;
        private const int OPTION_ID_WSA_ROOT = 12;
        private const int OPTION_ID_CUSTOM = 13;
        private const int OPTION_ID_PRIVATE_ROOT = 20;

        public void CacheRootConfirmed(string value)
        {
            SetCacheRoot(value);
        }

        internal string GetFeedSubLabel(IPodcastInfo podcastFeed)
        {
            return ValueFormatter.GetFeedOverrideSummary(podcastFeed);
        }

        internal void FeedItemSelected(string id, IPodcastInfo podcastFeed)
        {
            Logger.Debug(() => $"EditConfigViewModel: FeedItemSelected {podcastFeed.Folder}");
            if (podcastFeed.Feed == null)
            {
                // this will happen if a podcast element is created without a feed element - for example after importing an XML control file
                ValuePromptDialogFragment.ValuePromptDialogFragmentParameters promptParams = new ValuePromptDialogFragment.ValuePromptDialogFragmentParameters()
                {
                    Title = ResourceProvider.GetString(Resource.String.prompt_add_feed_title),
                    Ok = ResourceProvider.GetString(Resource.String.action_ok),
                    Cancel = ResourceProvider.GetString(Resource.String.action_cancel),
                    Prompt = ResourceProvider.GetString(Resource.String.prompt_add_feed_prompt),
                    Data = id,
                };
                Observables.PromptToAddFeed?.Invoke(this, promptParams);
            }
            else
            {
                Observables.NavigateToFeed?.Invoke(this, id);
            }
        }

        internal void FeedItemOptionSelected(string id, IPodcastInfo podcastFeed)
        {
            Observables.DeletePrompt?.Invoke(this,
                Tuple.Create(
                    string.Format(ResourceProvider.GetString(Resource.String.prompt_delete_podcast_title_fmt), podcastFeed.Folder),
                    ResourceProvider.GetString(Resource.String.prompt_delete_podcast_prompt),
                    ResourceProvider.GetString(Resource.String.prompt_delete_podcast_ok),
                    ResourceProvider.GetString(Resource.String.action_cancel),
                    id
                )
            );
        }

        private IPodcastInfo? GetPodcastByPosition(string position)
        {
            // find the podcast
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            int index = 0;
            int indexToFind = Convert.ToInt32(position);
            foreach (var podcastInfo in controlFile.GetPodcasts())
            {
                if (index == indexToFind)
                {
                    return podcastInfo;
                }
                index++;
            }
            return null;
        }

        public void DeleteConfirmed(string data)
        {
            // find the podcast
            Logger.Debug(() => $"EditConfigViewModel:FeedItemOptionSelected {data}");
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            IPodcastInfo? podcastToDelete = GetPodcastByPosition(data);
            if (podcastToDelete != null)
            {
                Logger.Debug(() => $"EditConfigViewModel:FeedItemOptionSelected deleting {podcastToDelete.Folder}");
                AnalyticsEngine.RemovePodcastEvent(podcastToDelete.Folder);
                controlFile.DeletePodcast(podcastToDelete);
                ApplicationControlFileProvider.SaveCurrentControlFile();
            }
        }

        public void AddPodcastSelected()
        {
            Observables.NavigateToAddFeed?.Invoke(this, EventArgs.Empty);
        }

        public void AddFeedConfirmed(string value, string data)
        {
            // this will happen if a podcast element is created without a feed element - for example after importing an XML control file
            Logger.Debug(() => $"EditConfigViewModel:Add Feed = {value}");
            if (string.IsNullOrWhiteSpace(value))
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.bad_url));
                return;
            }
            if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.bad_url));
                return;
            }

            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            IPodcastInfo? podcastToEdit = GetPodcastByPosition(data);
            if (podcastToEdit != null)
            {
                if (podcastToEdit.Feed == null)
                {
                    podcastToEdit.Feed = new FeedInfo(controlFile);
                }
                podcastToEdit.Feed.Address = new Uri(value);
                ApplicationControlFileProvider.SaveCurrentControlFile();
                AnalyticsEngine.AddPodcastFeedEvent(value);
                Observables.NavigateToFeed?.Invoke(this, data);
            }
        }
    }
}
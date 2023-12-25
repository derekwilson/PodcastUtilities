using Android.App;
using Android.Content;
using Android.Views;
using AndroidX.DocumentFile.Provider;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Adapters;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidLogic.ViewModel.Edit
{
    public class EditConfigViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> DisplayMessage;
            public EventHandler<Tuple<string, Intent>> DisplayChooser;
            public EventHandler<Tuple<string, string, string, string>> ResetPrompt;
            public EventHandler SelectFolder;
            public EventHandler SelectControlFile;
            public EventHandler<string> SetCacheRoot;
            public EventHandler<Tuple<string, List<PodcastFeedRecyclerItem>>> SetFeedItems;
            public EventHandler<string> NavigateToFeed;
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

        private List<PodcastFeedRecyclerItem> AllFeedItems = new List<PodcastFeedRecyclerItem>(20);

        public EditConfigViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            ICrashReporter crashReporter,
            IAnalyticsEngine analyticsEngine,
            IFileSystemHelper fileSystemHelper,
            IApplicationControlFileFactory applicationControlFileFactory,
            IValueFormatter valueFormatter) : base(app)
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

        private void ConfigurationUpdated(object sender, EventArgs e)
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

            var cacheRootSublabel = string.Format(ResourceProvider.GetString(Resource.String.cache_root_label_fmt), controlFile.GetSourceRoot());
            Observables.SetCacheRoot?.Invoke(this, cacheRootSublabel);

            AllFeedItems.Clear();
            int index = 0;
            foreach (var podcastInfo in controlFile.GetPodcasts())
            {
                var item = new PodcastFeedRecyclerItem()
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
                Observables.SelectControlFile?.Invoke(this, null);
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
                ApplicationControlFileProvider.ReplaceApplicationConfiguration(controlFile);
                AnalyticsEngine.LoadControlFileEvent();
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.control_file_loaded));
            }
        }

        private IReadWriteControlFile OpenControlFile(Android.Net.Uri uri)
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
            Java.IO.File[] files = FileSystemHelper.GetApplicationExternalFilesDirs();
            foreach (Java.IO.File file in files)
            {
                Logger.Debug(() => $"ExternalFile = {file.AbsolutePath}");
                options.Add(new SelectableString(OPTION_ID_PRIVATE_ROOT + index, file.AbsolutePath));
                index++;
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
                    Observables.SelectFolder?.Invoke(this, null);
                    break;
                case OPTION_ID_CUSTOM:
                    Observables.DisplayMessage?.Invoke(this, "Not yet implemented");
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


        internal void FeedItemSelected(string id, IPodcastInfo podcastFeed)
        {
            Logger.Debug(() => $"EditConfigViewModel: FeedItemSelected {podcastFeed.Folder}");
            if (podcastFeed.Feed == null)
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.config_no_feed));
            }
            else
            {
                Observables.NavigateToFeed?.Invoke(this, id);
            }
        }

        internal void FeedItemOptionSelected(string id, IPodcastInfo podcastFeed)
        {
            Observables.DisplayMessage?.Invoke(this, "Option - " + podcastFeed.Folder);
        }

        internal string GetFeedSubLabel(IPodcastInfo podcastFeed)
        {
            return ValueFormatter.GetFeedOverrideSummary(podcastFeed);
        }

        public void AddPodcastSelected()
        {
            Observables.DisplayMessage?.Invoke(this, "Add feed");
        }
    }
}
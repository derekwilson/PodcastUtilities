using Android.App;
using Android.Content;
using Android.Views;
using AndroidX.DocumentFile.Provider;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using System;

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
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IApplicationControlFileProvider ApplicationControlFileProvider;
        private ICrashReporter CrashReporter;
        private IAnalyticsEngine AnalyticsEngine;
        private IFileSystemHelper FileSystemHelper;

        public EditConfigViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            ICrashReporter crashReporter,
            IAnalyticsEngine analyticsEngine,
            IFileSystemHelper fileSystemHelper) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"EditConfigViewModel:ctor");

            ApplicationContext = app;
            ResourceProvider = resProvider;
            ApplicationControlFileProvider = appControlFileProvider;
            ApplicationControlFileProvider.ConfigurationUpdated += ConfigurationUpdated;
            CrashReporter = crashReporter;
            AnalyticsEngine = analyticsEngine;
            FileSystemHelper = fileSystemHelper;
        }

        private void ConfigurationUpdated(object sender, EventArgs e)
        {
            Logger.Debug(() => $"EditConfigViewModel:ConfigurationUpdated");
        }

        public void Initialise()
        {
            Logger.Debug(() => $"EditConfigViewModel:Initialise");
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
                case Keycode.S:
                    return DoIfPossible(Resource.Id.action_edit_share_control);
                case Keycode.R:
                    return DoIfPossible(Resource.Id.action_edit_reset_control);
                case Keycode.C:
                    return DoIfPossible(Resource.Id.action_edit_cache_root);
                case Keycode.G:
                    return DoIfPossible(Resource.Id.action_edit_globals);
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
            if (itemId == Resource.Id.action_edit_share_control)
            {
                return true;
            }
            if (itemId == Resource.Id.action_edit_reset_control)
            {
                return true;
            }
            if (itemId == Resource.Id.action_edit_cache_root)
            {
                return true;
            }
            if (itemId == Resource.Id.action_edit_globals)
            {
                return true;
            }
            return false;
        }

        public bool ActionSelected(int itemId)
        {
            Logger.Debug(() => $"EditConfigViewModel:ActionSelected = {itemId}");
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
            if (itemId == Resource.Id.action_edit_cache_root)
            {
                Observables.SelectFolder?.Invoke(this, null);
                return true;
            }
            return false;
        }

        public void ShareConfig()
        {
            if (ApplicationControlFileProvider == null)
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.settings_share_no_controlfile));
                return;
            }
            var intent = ApplicationControlFileProvider.GetApplicationConfigurationSharingIntent();
            if (intent != null)
            {
                Observables.DisplayChooser?.Invoke(this, Tuple.Create(ResourceProvider.GetString(Resource.String.settings_share_chooser_title), intent));
            }
            else
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.settings_share_no_controlfile));
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
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.settings_share_no_controlfile));
                return;
            }
            ApplicationControlFileProvider.ResetControlFile();
            AnalyticsEngine.ResetControlFileEvent();
            Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.edit_reset));
        }

        public void FolderSelected(DocumentFile file)
        {
            var folder = FileSystemHelper.GetRealPathFromDocumentTreeFile(file);
            Logger.Debug(() => $"EditConfigViewModel:FolderSelected = {folder}");
            var ControlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            ControlFile.SetSourceRoot(folder);
            ApplicationControlFileProvider.SaveCurrentControlFile();
            Observables.DisplayMessage?.Invoke(this, folder);
        }
    }
}
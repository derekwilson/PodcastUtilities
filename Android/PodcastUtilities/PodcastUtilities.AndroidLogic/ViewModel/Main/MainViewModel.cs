﻿using Android.App;
using Android.Views;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Playlists;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PodcastUtilities.AndroidLogic.ViewModel.Main
{
    public class MainViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> Title;
            public EventHandler<View> AddInfoView;
            public EventHandler ShowNoDriveMessage;
            public EventHandler NavigateToSettings;
            public EventHandler SelectControlFile;
            public EventHandler<string> ToastMessage;
            public EventHandler<Tuple<string, List<PodcastFeedRecyclerItem>>> SetFeedItems;
            public EventHandler<string> SetCacheRoot;
            public EventHandler<string> NavigateToDownload;
            public EventHandler NavigateToPurge;
            public EventHandler NavigateToEditConfig;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IApplicationControlFileProvider ApplicationControlFileProvider;
        private IFileSystemHelper FileSystemHelper;
        private IByteConverter ByteConverter;
        private ICrashReporter CrashReporter;
        private IAnalyticsEngine AnalyticsEngine;
        private IGenerator PlaylistGenerator;
        private IDriveVolumeInfoViewFactory DriveVolumeInfoViewFactory;
        private IValueFormatter ValueFormatter;
        private IAndroidApplication AndroidApplication;

        private List<PodcastFeedRecyclerItem> AllFeedItems = new List<PodcastFeedRecyclerItem>(20);

        public MainViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            IFileSystemHelper fsHelper,
            IByteConverter byteConverter,
            ICrashReporter crashReporter,
            IAnalyticsEngine analyticsEngine,
            IGenerator playlistGenerator,
            IDriveVolumeInfoViewFactory driveVolumeInfoViewFactory,
            IValueFormatter valueFormatter,
            IAndroidApplication androidApplication) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"MainViewModel:ctor");

            ApplicationContext = app;
            ResourceProvider = resProvider;
            ApplicationControlFileProvider = appControlFileProvider;
            ApplicationControlFileProvider.ConfigurationUpdated += ConfigurationUpdated;
            ApplicationControlFileProvider.LoggingLevelUpdated += LoggingLevelUpdated;
            FileSystemHelper = fsHelper;
            ByteConverter = byteConverter;
            CrashReporter = crashReporter;
            AnalyticsEngine = analyticsEngine;
            PlaylistGenerator = playlistGenerator;
            // we only want to add this once, really we should remove it if the IGenerator is a singleton
            PlaylistGenerator.StatusUpdate += GenerateStatusUpdate;
            DriveVolumeInfoViewFactory = driveVolumeInfoViewFactory;
            ValueFormatter = valueFormatter;
            AndroidApplication = androidApplication;
        }

        private void LoggingLevelUpdated(object sender, EventArgs e)
        {
            Logger.Debug(() => $"MainViewModel:LoggingLevelUpdated");
            SetLoggingLevel();
        }

        private void ConfigurationUpdated(object sender, EventArgs e)
        {
            Logger.Debug(() => $"MainViewModel:ConfigurationUpdated");
            RefreshFeedList();
        }

        public void Initialise()
        {
            Logger.Debug(() => $"MainViewModel:Initialise");
            Observables.Title?.Invoke(this, ResourceProvider.GetString(Resource.String.main_activity_title));
            SetLoggingLevel();
            RefreshFeedList();
        }

        [Lifecycle.Event.OnResume]
        [Java.Interop.Export]
        public void OnResume()
        {
            Logger.Debug(() => $"MainViewModel:OnResume");
        }

        [Lifecycle.Event.OnDestroy]
        [Java.Interop.Export]
        public void OnDestroy()
        {
            Logger.Debug(() => $"MainViewModel:OnDestroy");
            ApplicationControlFileProvider.ConfigurationUpdated -= ConfigurationUpdated;
            ApplicationControlFileProvider.LoggingLevelUpdated -= LoggingLevelUpdated;
            PlaylistGenerator.StatusUpdate -= GenerateStatusUpdate;
        }

        public void RefreshFileSystemInfo()
        {
            Logger.Debug(() => $"MainViewModel:RefreshFileSystemInfo");
            try
            {
                Observables.ShowNoDriveMessage?.Invoke(this, null);

                Java.IO.File[] files = FileSystemHelper.GetApplicationExternalFilesDirs();
                foreach (Java.IO.File file in files)
                {
                    Logger.Debug(() => $"ExternalFile = {file.AbsolutePath}");
                    AddFileSystem(file.AbsolutePath);
                }
            } catch (Exception ex)
            {
                Logger.LogException(() => $"MainViewModel:InitFileSystemInfo", ex);
                CrashReporter.LogNonFatalException(ex);
                // if we didnt add any drive info views then the default message will remain
            }
        }

        private void SetLoggingLevel()
        {
            Logger.Debug(() => $"MainViewModel:SetLoggingLevel");
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            if (controlFile != null)
            {
                switch (controlFile.GetDiagnosticOutput())
                    {
                        case DiagnosticOutputLevel.None:
                            Logger.Debug(() => $"MainViewModel:SetLoggingLevel = {controlFile.GetDiagnosticOutput()}");
                            AndroidApplication.SetLoggingNone();
                            break;
                        case DiagnosticOutputLevel.Verbose:
                            AndroidApplication.SetLoggingVerbose();
                            Logger.Debug(() => $"MainViewModel:SetLoggingLevel = {controlFile.GetDiagnosticOutput()}");
                            break;
                    }
            }
            Logger.Debug(() => $"MainViewModel:SetLoggingLevel - end");
        }

        private void RefreshFeedList()
        {
            AllFeedItems.Clear();
            var cacheRoot = "";
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            if (controlFile != null)
            {
                cacheRoot = controlFile.GetSourceRoot();
                foreach (var podcastInfo in controlFile.GetPodcasts())
                {
                    var item = new PodcastFeedRecyclerItem()
                    {
                        PodcastFeed = podcastInfo
                    };
                    AllFeedItems.Add(item);
                }
            }
            var heading = ResourceProvider.GetQuantityString(Resource.Plurals.feed_list_heading, AllFeedItems.Count);
            Observables.SetCacheRoot?.Invoke(this, cacheRoot);
            Logger.Debug(() => $"MainViewModel:RefreshFeedList {AllFeedItems.Count}");
            Observables.SetFeedItems?.Invoke(this, Tuple.Create(heading, AllFeedItems));
        }

        private void AddFileSystem(string absolutePath)
        {
            long freeBytes = FileSystemHelper.GetAvailableFileSystemSizeInBytes(absolutePath);
            long totalBytes = FileSystemHelper.GetTotalFileSystemSizeInBytes(absolutePath);
            long usedBytes = totalBytes - freeBytes;
            string[] freeSize = DisplayFormatter.RenderFileSize(freeBytes).Split(' ');
            string[] totalSize = DisplayFormatter.RenderFileSize(totalBytes).Split(' ');

            var view = DriveVolumeInfoViewFactory.GetNewView(ApplicationContext);
            view.Title = ConvertPathToTitle(absolutePath);
            view.SetSpace(
                Convert.ToInt32(ByteConverter.BytesToMegabytes(usedBytes)), 
                Convert.ToInt32(ByteConverter.BytesToMegabytes(totalBytes)), 
                freeSize[0], freeSize[1], 
                totalSize[0], totalSize[1]);

            Observables?.AddInfoView?.Invoke(this, view.GetView());
        }

        private string ConvertPathToTitle(string absolutePath)
        {
            var retval = absolutePath; 
            // strip off our package name
            var pos = retval.IndexOf(ApplicationContext.PackageName);
            if (pos > 0)
            {
                retval = retval.Substring(0, pos);
            }
            // strip off the standard prefix
            pos = retval.IndexOf("/Android/data", StringComparison.InvariantCultureIgnoreCase);
            if (pos > 0)
            {
                retval = retval.Substring(0, pos);
            }

            return retval;
        }

        public bool IsActionAvailable(int itemId)
        {
            Logger.Debug(() => $"MainViewModel:isActionAvailable = {itemId}");
            if (itemId == Resource.Id.action_settings)
            {
                return true;
            }
            if (itemId == Resource.Id.action_edit_config)
            {
                return true;
            }
            if (itemId == Resource.Id.action_purge)
            {
                return AllFeedItems.Count > 0;
            }
            if (itemId == Resource.Id.action_download)
            {
                return AllFeedItems.Count > 0;
            }
            if (itemId == Resource.Id.action_playlist)
            {
                return AllFeedItems.Count > 0;
            }
            return false;
        }

        public bool ActionSelected(int itemId)
        {
            Logger.Debug(() => $"MainViewModel:ActionSelected = {itemId}");
            if (itemId == Resource.Id.action_settings)
            {
                Observables.NavigateToSettings?.Invoke(this, null);
                return true;
            }
            if (itemId == Resource.Id.action_edit_config)
            {
                Observables.NavigateToEditConfig?.Invoke(this, null);
                return true;
            }
            if (itemId == Resource.Id.action_purge)
            {
                Observables.NavigateToPurge?.Invoke(this, null);
                return true;
            }
            if (itemId == Resource.Id.action_download)
            {
                Observables.NavigateToDownload?.Invoke(this, null);
                return true;
            }
            if (itemId == Resource.Id.action_playlist)
            {
                Task.Run(() => GeneratePlaylist());
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

        public bool KeyEvent(KeyEvent e)
        {
            Logger.Debug(() => $"MainViewModel:KeyEvent = {e.Action}, {e.KeyCode}");
            if (e == null || e.Action != KeyEventActions.Up)
            {
                // lets get rid of most of the stuff we are not interested in
                return false;
            }
            switch (e.KeyCode)
            {
                case Keycode.P:
                    return DoIfPossible(Resource.Id.action_purge);
                case Keycode.D:
                    return DoIfPossible(Resource.Id.action_download);
                case Keycode.G:
                    return DoIfPossible(Resource.Id.action_playlist);
                case Keycode.C:
                    return DoIfPossible(Resource.Id.action_edit_config);
                case Keycode.S:
                    return DoIfPossible(Resource.Id.action_settings);
            }
            return false;
        }

        public void GeneratePlaylist()
        {
            try
            {
                Logger.Debug(() => $"MainViewModel: GeneratePlaylist");
                var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
                if (controlFile != null)
                {
                    // the status update is added once for the whole ViewModel
                    PlaylistGenerator.GeneratePlaylist(controlFile, controlFile.GetSourceRoot(), true, null);
                    AnalyticsEngine.GeneratePlaylistEvent(controlFile.GetPlaylistFormat());
                }
                else
                {
                    Logger.Debug(() => $"MainViewModel: GeneratePlaylist - no control file");
                    return;
                }
                Logger.Debug(() => $"MainViewModel: GeneratePlaylist - done");
            }
            catch (Exception ex)
            {
                Logger.LogException(() => $"MainViewModel:GeneratePlaylist", ex);
                CrashReporter.LogNonFatalException(ex);
                Observables.ToastMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.error_generating_playlist));
            }
        }

        private void GenerateStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            Logger.Debug(() => $"MainViewModel: GenerateStatusUpdate {e.IsTaskCompletedSuccessfully}, {e.Message}");
            Observables.ToastMessage?.Invoke(this, e.Message);
            IPlaylist playlist = null;
            int items = -1;
            if (e.UserState != null && e.UserState is IPlaylist)
            {
                playlist = e.UserState as IPlaylist;
                items = playlist.NumberOfTracks;
            }
            AnalyticsEngine.GeneratePlaylistCompleteEvent(items);
        }
        internal void FeedItemSelected(IPodcastInfo podcastFeed)
        {
            Logger.Debug(() => $"MainViewModel: FeedItemSelected {podcastFeed.Folder}");
            Observables.NavigateToDownload?.Invoke(this, podcastFeed.Folder);
        }

        private string GetDownloadStratagyText(PodcastEpisodeDownloadStrategy value)
        {
            return ValueFormatter.GetDownloadStratagyText(value);
        }

        internal string GetFeedItemSubLabel(IPodcastInfo podcastFeed)
        {
            var fmt = ResourceProvider.GetString(Resource.String.feed_sublabel_fmt);
            return string.Format(fmt,
                GetSublabelDatePart(
                        podcastFeed.Feed.MaximumDaysOld.Value,
                        Resource.String.feed_sublabel_download_from,
                        Resource.String.feed_sublabel_download_all),
                GetSublabelPart(
                        podcastFeed.Feed.MaximumNumberOfDownloadedItems.Value,
                        Resource.Plurals.feed_sublabel_max,
                        Resource.String.feed_sublabel_no_max)
            );
        }

        internal string GetFeedItemSubLabel2(IPodcastInfo podcastFeed)
        {
            var fmt2 = ResourceProvider.GetString(Resource.String.feed_sublabel_fmt2);
            return string.Format(fmt2,
                GetSublabelDatePart(
                        podcastFeed.Feed.DeleteDownloadsDaysOld.Value,
                        Resource.String.feed_sublabel_delete_before,
                        Resource.String.feed_sublabel_delete_never),
                GetDownloadStratagyText(podcastFeed.Feed.DownloadStrategy.Value)
            );
        }

        private string GetSublabelDatePart(int value, int formattedId, int maxId)
        {
            if (value == int.MaxValue)
            {
                return ResourceProvider.GetString(maxId);
            }
            return string.Format(ResourceProvider.GetString(formattedId), ValueFormatter.GetDayOffsetAsDateString(-value));
        }

        private string GetSublabelPart(int value, int formattedId, int maxId)
        {
            if (value == int.MaxValue)
            {
                return ResourceProvider.GetString(maxId);
            }
            return ResourceProvider.GetQuantityString(formattedId, value);
        }
    }
}
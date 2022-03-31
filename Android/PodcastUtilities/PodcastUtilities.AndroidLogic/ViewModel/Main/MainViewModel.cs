using Android.App;
using Android.Content;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Xml;

namespace PodcastUtilities.AndroidLogic.ViewModel.Main
{
    public class MainViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> Title;
            public EventHandler<DriveVolumeInfoView> AddInfoView;
            public EventHandler ShowNoDriveMessage;
            public EventHandler NavigateToSettings;
            public EventHandler SelectControlFile;
            public EventHandler<string> ToastMessage;
            public EventHandler<Tuple<string, List<PodcastFeedRecyclerItem>>> SetFeedItems;
            public EventHandler<string> SetCacheRoot;
            public EventHandler NavigateToDownload;
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

        private List<PodcastFeedRecyclerItem> AllFeedItems = new List<PodcastFeedRecyclerItem>(20);

        public MainViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            IFileSystemHelper fsHelper,
            IByteConverter byteConverter, 
            ICrashReporter crashReporter, 
            IAnalyticsEngine analyticsEngine) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"MainViewModel:ctor");

            ApplicationContext = app;
            ResourceProvider = resProvider;
            ApplicationControlFileProvider = appControlFileProvider;
            FileSystemHelper = fsHelper;
            ByteConverter = byteConverter;
            CrashReporter = crashReporter;
            AnalyticsEngine = analyticsEngine;
        }

        public void Initialise()
        {
            Logger.Debug(() => $"MainViewModel:Initialise");
            Observables.Title?.Invoke(this, ResourceProvider.GetString(Resource.String.main_activity_title));
            RefreshFeedList();
        }

        [Lifecycle.Event.OnResume]
        [Java.Interop.Export]
        public void OnResume()
        {
            Logger.Debug(() => $"MainViewModel:OnResume");
        }

        public void RefreshFileSystemInfo()
        {
            Logger.Debug(() => $"MainViewModel:RefreshFileSystemInfo");
            try
            {
                Observables.ShowNoDriveMessage?.Invoke(this, null);

                Java.IO.File[] files = ApplicationContext.GetExternalFilesDirs(null);
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
                    Logger.Debug(() => $"MainViewModel:RefreshFeedList {podcastInfo.Folder}");
                    var item = new PodcastFeedRecyclerItem()
                    {
                        PodcastFeed = podcastInfo
                    };
                    AllFeedItems.Add(item);
                }
            }
            var heading = ResourceProvider.GetQuantityString(Resource.Plurals.feed_list_heading, AllFeedItems.Count);
            Observables.SetCacheRoot?.Invoke(this, cacheRoot);
            Observables.SetFeedItems?.Invoke(this, Tuple.Create(heading, AllFeedItems));
        }

        private void AddFileSystem(string absolutePath)
        {
            long freeBytes = FileSystemHelper.GetAvailableFileSystemSizeInBytes(absolutePath);
            long totalBytes = FileSystemHelper.GetTotalFileSystemSizeInBytes(absolutePath);
            long usedBytes = totalBytes - freeBytes;
            string[] freeSize = DisplayFormatter.RenderFileSize(freeBytes).Split(' ');
            string[] totalSize = DisplayFormatter.RenderFileSize(totalBytes).Split(' ');

            var view = new DriveVolumeInfoView(ApplicationContext);
            view.Title = ConvertPathToTitle(absolutePath);
            view.SetSpace(
                Convert.ToInt32(ByteConverter.BytesToMegabytes(usedBytes)), 
                Convert.ToInt32(ByteConverter.BytesToMegabytes(totalBytes)), 
                freeSize[0], freeSize[1], 
                totalSize[0], totalSize[1]);

            Observables?.AddInfoView.Invoke(this, view);
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
            if (itemId == Resource.Id.action_load_control)
            {
                return true;
            }
            if (itemId == Resource.Id.action_download)
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
            if (itemId == Resource.Id.action_load_control)
            {
                Observables.SelectControlFile?.Invoke(this, null);
                return true;
            }
            if (itemId == Resource.Id.action_download)
            {
                Observables.NavigateToDownload?.Invoke(this, null);
                return true;
            }
            return false;
        }

        public void LoadContolFile(Android.Net.Uri data)
        {
            var controlFile = OpenControlFile(data);
            if (controlFile != null)
            {
                ApplicationControlFileProvider.ReplaceApplicationConfiguration(controlFile);
                AnalyticsEngine.LoadControlFileEvent();
                RefreshFeedList();
                Observables.ToastMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.control_file_loaded));
            }
        }

        private ReadWriteControlFile OpenControlFile(Android.Net.Uri uri)
        {
            try
            {
                Logger.Debug(() => $"MainViewModel:OpenControlFile = {uri.ToString()}");
                ContentResolver resolver = ApplicationContext.ContentResolver;
                var stream = resolver.OpenInputStream(uri);
                var xml = new XmlDocument();
                xml.Load(stream);
                return new ReadWriteControlFile(xml);
            }
            catch (Exception ex)
            {
                Logger.LogException(() => $"MainViewModel:OpenControlFile", ex);
                CrashReporter.LogNonFatalException(ex);
                Observables.ToastMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.error_reading_control_file));
                return null;
            }
        }

    }
}
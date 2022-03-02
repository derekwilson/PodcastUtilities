using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common;
using System;

namespace PodcastUtilities.AndroidLogic.ViewModel.Main
{
    public class MainViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> Title;
            public EventHandler<DriveVolumeInfoView> AddInfoView;
            public EventHandler ShowNoDriveMessage;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IFileSystemHelper FileSystemHelper;
        private IByteConverter ByteConverter;

        public MainViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IFileSystemHelper fsHelper,
            IByteConverter byteConverter) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"MainViewModel:ctor");

            ApplicationContext = app;
            ResourceProvider = resProvider;
            FileSystemHelper = fsHelper;
            ByteConverter = byteConverter;
        }

        public void Initialise()
        {
            Logger.Debug(() => $"MainViewModel:Initialise");
            Observables.Title?.Invoke(this, ResourceProvider.GetString(Resource.String.main_activity_title));
        }

        [Lifecycle.Event.OnResume]
        [Java.Interop.Export]
        public void OnResume()
        {
            Logger.Debug(() => $"MainViewModel:OnResume");
        }

        public void RefreshFileSystemInfo()
        {
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
                // if we didnt add any drive info views then the default message will remain
            }
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

        public bool isActionAvailable(int itemId)
        {
            Logger.Debug(() => $"isActionAvailable = {itemId}");
            if (itemId == Resource.Id.action_settings)
            {
                return true;
            }
            if (itemId == Resource.Id.action_load_config)
            {
                return false;
            }
            return false;
        }
    }
}
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.Common.Configuration;
using System;
using System.IO;

namespace PodcastUtilities.AndroidLogic.Utilities
{

    public interface IApplicationControlFileProvider
    {
        IReadWriteControlFile GetApplicationConfiguration();
        void ReplaceApplicationConfiguration(IReadWriteControlFile file);
    }

    public class ApplicationControlFileProvider : IApplicationControlFileProvider
    {
        private IReadWriteControlFile ControlFile = null;
        // do not make this anything other than private
        private object SyncLock = new object();

        private ILogger Logger;
        private IFileSystemHelper FileSystemHelper;
        private IControlFileFactory ControlFileFactory;

        public ApplicationControlFileProvider(
            ILogger logger,
            IFileSystemHelper fileSystemHelper,
            IControlFileFactory factory
        )
        {
            Logger = logger;
            FileSystemHelper = fileSystemHelper;
            ControlFileFactory = factory;
        }


        private string GetApplicationControlFilePath()
        {
            var folder = FileSystemHelper.GetApplicationFolderOnSdCard("config", true);
            var fileName = Path.Combine(folder, "PodcastUtilities.xml");
            return fileName;
        }

        public IReadWriteControlFile GetApplicationConfiguration()
        {
            lock (SyncLock)
            {
                if (ControlFile != null)
                {
                    return ControlFile;
                }
                try
                {
                    var filename = GetApplicationControlFilePath();
                    Logger.Debug(() => $"ApplicationControlFileProvider:GetApplicationConfiguration loading {filename}");
                    if (FileSystemHelper.Exists(filename))
                    {
                        Logger.Debug(() => $"ApplicationControlFileProvider:GetApplicationConfiguration exists {filename}");
                        ControlFile = ControlFileFactory.OpenControlFile(filename);
                    }
                } catch (Exception ex) 
                {
                    Logger.LogException(() => $"ApplicationControlFileProvider:GetApplicationConfiguration", ex);
                }
            }
            return ControlFile;
        }

        public void ReplaceApplicationConfiguration(IReadWriteControlFile file)
        {
            Logger.Debug(() => $"ApplicationControlFileProvider:ReplaceApplicationConfiguration");
            lock (SyncLock)
            {
                ControlFile = null;
                file.SaveToFile(GetApplicationControlFilePath());
                ControlFile = file;
            }
        }
    }
}
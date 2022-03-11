using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.Common.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IApplicationControlFileProvider
    {
        string GetApplicationControlFilePath();
        ReadWriteControlFile LoadApplicationConfiguration();
    }

    public class ApplicationControlFileProvider : IApplicationControlFileProvider
    {
        private Context ApplicationContext;
        private ILogger Logger;
        private IFileSystemHelper FileSystemHelper;

        public ApplicationControlFileProvider(
            Context context,
            ILogger logger,
            IFileSystemHelper fileSystemHelper
        )
        {
            ApplicationContext = context;
            Logger = logger;
            FileSystemHelper = fileSystemHelper;
        }


        public string GetApplicationControlFilePath()
        {
            var folder = FileSystemHelper.GetApplicationFolderOnSdCard("config", true);
            var fileName = Path.Combine(folder, "PodcastUtilities.xml");
            return fileName;
        }

        public ReadWriteControlFile LoadApplicationConfiguration()
        {
            return null;
        }
    }
}
using Android.Content;
using Android.OS;
using PodcastUtilities.AndroidLogic.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IFileSystemHelper
    {
        string GetApplicationFolderOnSdCard(string subFolder, bool ensureExists);
        long GetAvailableFileSystemSizeInBytes(string path);
        long GetTotalFileSystemSizeInBytes(string path);
        string GetApplicationFolderOnSdCard();
        List<string> GetFolderFiles(string foldername);
        string GetFileContents(string filename, bool addLineEndings);
    }

    public class FileSystemHelper : IFileSystemHelper
    {
        private Context ApplicationContext;
        private ILogger Logger;

        public FileSystemHelper(
            Context context,
            ILogger logger
        )
        {
            ApplicationContext = context;
            Logger = logger;
        }

        public string GetApplicationFolderOnSdCard(string subFolder, bool ensureExists)
        {
            // we are limited where we can write unless we ask for extra permissions in android 11+
            var dir = ApplicationContext.GetExternalFilesDir(null);
            // throw if we cannot get the path
            var path = dir.AbsolutePath;
            if (!String.IsNullOrEmpty(subFolder))
            {
                path = Path.Combine(path, subFolder);
            }
            if (ensureExists)
            {
                var targetFolder = new DirectoryInfo(path);
                if (!targetFolder.Exists)
                {
                    targetFolder.Create();
                }
            }
            return path;
        }

        public string GetApplicationFolderOnSdCard()
        {
            return GetApplicationFolderOnSdCard(null, false);
        }

        public long GetAvailableFileSystemSizeInBytes(string path)
        {
            StatFs stat = new StatFs(path);
            long blockSize = stat.BlockSizeLong;
            long availableBlocks = stat.AvailableBlocksLong;
            return availableBlocks * blockSize;
        }

        public string GetFileContents(string filename, bool addLineEndings)
        {
            Stream inputStream = null;
            StringBuilder builder = new StringBuilder();
            try
            {
                inputStream = ApplicationContext.Assets.Open(filename);
                Logger.Debug(() => $"FileSystemHelper:GetFileContents - {filename}");
                using (var streamReader = new StreamReader(inputStream, Encoding.UTF8, true, 8192))
                {
                    String line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        builder.Append(line);
                        if (addLineEndings)
                        {
                            builder.Append("\n");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(() => $"FileSystemHelper:GetFileContents - {filename}", ex);
            }
            finally
            {
                try
                {
                    inputStream?.Close();
                }
                catch (Exception ex)
                {
                    Logger.LogException(() => $"FileSystemHelper:GetFileContents closing - {filename}", ex);
                }
            }
            return builder.ToString();
        }

        public List<string> GetFolderFiles(string foldername)
        {
            var allFiles = new List<string>(10);
            try
            {
                var files = ApplicationContext.Assets.List(foldername);
                if (files == null)
                {
                    return null;
                }
                foreach (var file in files)
                {
                    var filename = $"{foldername}/{file}";
                    var subFolderFiles = GetFolderFiles(filename);
                    if (subFolderFiles != null && subFolderFiles.Count > 0)
                    {
                        // the file is a folder, add its files
                        allFiles.AddRange(subFolderFiles);
                    }
                    else
                    {
                        // the file is a file
                        allFiles.Add(filename);
                    }
                }
            }
            catch (Exception ex) 
            {
                Logger.LogException(() => $"FileSystemHelper:GetFolderFiles - {foldername}", ex);
            }
            return allFiles;
        }

        public long GetTotalFileSystemSizeInBytes(string path)
        {
            StatFs stat = new StatFs(path);
            long blockSize = stat.BlockSizeLong;
            long blocks = stat.BlockCountLong;
            return blocks * blockSize;
        }
    }
}
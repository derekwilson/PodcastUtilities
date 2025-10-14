using Android.Content;
using Android.OS;
using Android.Provider;
using AndroidX.Core.Content;
using AndroidX.DocumentFile.Provider;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.Common.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IFileSystemHelper
    {
        const string CONFIG_FOLDER = "config";

        string GetApplicationFolderOnSdCard(string subFolder, bool ensureExists);
        long GetAvailableFileSystemSizeInBytes(string path);
        long GetTotalFileSystemSizeInBytes(string path);
        string GetApplicationFolderOnSdCard();
        string? GetApplicationFirstExternalPath();
        Java.IO.File[]? GetApplicationExternalFilesDirs();
        List<string>? GetAssetsFolderFiles(string foldername);
        string GetAssetsFileContents(string filename, bool addLineEndings);
        bool Exists(string pathname);
        void LogPersistantPermissions();
        void TakePersistantPermission(Android.Net.Uri uri);

        XmlDocument LoadXmlFromContentUri(Android.Net.Uri uri);
        XmlDocument? LoadXmlFromAssetFile(string filename);
        Android.Net.Uri GetAttachmentUri(String filename);

        string GetRealPathFromDocumentTreeFile(DocumentFile documentFile);

        string RemoveInvalidFilenameChars(string filename);
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

        public bool Exists(string pathname)
        {
            var targetFile = new FileInfo(pathname);
            if (targetFile != null)
            {
                return targetFile.Exists;
            }
            return false;
        }

        public string GetApplicationFolderOnSdCard(string? subFolder, bool ensureExists)
        {
            // we are limited where we can write unless we ask for extra permissions in android 11+
            var dir = ApplicationContext.GetExternalFilesDir(null)!;
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

        public string? GetApplicationFirstExternalPath()
        {
            var dirs = ApplicationContext.GetExternalFilesDirs(null);
            if (dirs != null && dirs[0] != null)
            {
                return dirs[0].AbsolutePath;
            }
            return null;
        }

        public Java.IO.File[]? GetApplicationExternalFilesDirs()
        {
            return ApplicationContext.GetExternalFilesDirs(null);
        }

        public XmlDocument? LoadXmlFromAssetFile(string filename)
        {
            Stream? inputStream = null;
            try
            {
                inputStream = ApplicationContext.Assets!.Open(filename);
                Logger.Debug(() => $"FileSystemHelper:LoadXmlFromAssetFile - {filename}");
                using (var streamReader = new StreamReader(inputStream, Encoding.UTF8, true, 8192))
                {
                    var xml = new XmlDocument();
                    xml.Load(streamReader);
                    return xml;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(() => $"FileSystemHelper:LoadXmlFromAssetFile - {filename}", ex);
            }
            finally
            {
                try
                {
                    inputStream?.Close();
                }
                catch (Exception ex)
                {
                    Logger.LogException(() => $"FileSystemHelper:LoadXmlFromAssetFile closing - {filename}", ex);
                }
            }
            return null;
        }

        public string GetAssetsFileContents(string filename, bool addLineEndings)
        {
            Stream? inputStream = null;
            StringBuilder builder = new StringBuilder();
            try
            {
                inputStream = ApplicationContext.Assets!.Open(filename);
                Logger.Debug(() => $"FileSystemHelper:GetAssetsFileContents - {filename}");
                using (var streamReader = new StreamReader(inputStream, Encoding.UTF8, true, 8192))
                {
                    string? line;
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
                Logger.LogException(() => $"FileSystemHelper:GetAssetsFileContents - {filename}", ex);
            }
            finally
            {
                try
                {
                    inputStream?.Close();
                }
                catch (Exception ex)
                {
                    Logger.LogException(() => $"FileSystemHelper:GetAssetsFileContents closing - {filename}", ex);
                }
            }
            return builder.ToString();
        }

        public List<string>? GetAssetsFolderFiles(string foldername)
        {
            var allFiles = new List<string>(10);
            try
            {
                var files = ApplicationContext.Assets!.List(foldername);
                if (files == null)
                {
                    return null;
                }
                foreach (var file in files)
                {
                    var filename = $"{foldername}/{file}";
                    var subFolderFiles = GetAssetsFolderFiles(filename);
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
                Logger.LogException(() => $"FileSystemHelper:GetAssetsFolderFiles - {foldername}", ex);
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

        public void LogPersistantPermissions()
        {
            foreach (UriPermission permission in ApplicationContext.ContentResolver!.PersistedUriPermissions) 
            {
                Logger.Debug(() => $"FileSystemHelper:LogPersistantPermissions - {permission?.Uri?.ToString()}");
            }
        }

        public void TakePersistantPermission(Android.Net.Uri uri)
        {
            ApplicationContext.ContentResolver!.TakePersistableUriPermission(uri, ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
        }

        public XmlDocument LoadXmlFromContentUri(Android.Net.Uri uri)
        {
            Logger.Debug(() => $"FileSystemHelper:LoadXmlFromContentUri = {uri.ToString()}");
            ContentResolver resolver = ApplicationContext.ContentResolver!;
            var stream = resolver.OpenInputStream(uri);
            var xml = new XmlDocument();
            if (stream != null)
            {
                xml.Load(stream);
            }
            return xml;
        }

        public Android.Net.Uri GetAttachmentUri(String filename)
        {
            Logger.Debug(() => $"FileSystemHelper:getAttachmentUri - {filename}");
            Java.IO.File shareFile = new Java.IO.File(filename);
            Android.Net.Uri shareableUri = FileProvider.GetUriForFile(ApplicationContext, ApplicationContext?.ApplicationContext?.PackageName + ".provider", shareFile)
                ?? throw new InvalidOperationException("cannot get URI");
            Logger.Debug(() => $"FileSystemHelper:getAttachmentUri uri - {shareableUri}");
            return shareableUri;
        }

        public string GetRealPathFromDocumentTreeFile(DocumentFile documentFile)
        {
            // see https://stackoverflow.com/questions/29713587/how-to-get-the-real-path-with-action-open-document-tree-intent
            //
            // we get back values like this for internal storage
            // /tree/primary:Podcasts/document/primary:Podcasts
            // and this for external
            // /tree/82E7-140A:Podcasts/document/82E7-140A:Podcasts

            Logger.Debug(() => $"FileSystemHelper:GetRealPathFromDocumentTreeFile {documentFile.Uri.Path}");

            if (documentFile == null || documentFile.Uri == null)
                return "";

            var path1 = documentFile.Uri.Path!;
            if (path1.StartsWith("/tree/"))
            {
                var path2 = path1.Remove(0, "/tree/".Length);
                if (path2.StartsWith("primary:"))
                {
                    // internal storage
                    var primary = path2.Remove(0, "primary:".Length);
                    if (primary.Contains(':'))
                    {
                        var storeName = "/storage/emulated/0/";
                        var last = path2.Split(':').LastOrDefault();
                        var realPath = storeName + last;
                        return realPath;
                    }
                }
                else
                {
                    // external storage
                    if (path2.Contains(':'))
                    {
                        var path3 = path2.Split(':').FirstOrDefault();
                        var storeName = path3;
                        var last = path2.Split(':').LastOrDefault();
                        var realPath = "/storage/" + storeName + "/" + last;
                        return realPath;
                    }
                }
            }
            return path1;
        }

        // these are chars that are invalid in the file system but not in Path.GetInvalidFileNameChars()
        // for example the ? and : on Android scoped file storage (MediaStore)
        // to be fair to Mono its not possible to get Path.GetInvalidFileNameChars() correct as the rules change depending on the folder (thanks google)
        private static char[] additional_invalid_chars = { '?', ':', '\'', '’', '|', '*' };

        public string RemoveInvalidFilenameChars(string filename)
        {
            Logger.Debug(() => $"FileSystemHelper:RemoveInvalidFilenameChars input = {filename}");
            if (Path.GetInvalidFileNameChars() != null && filename.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                Logger.Debug(() => $"FileSystemHelper:RemoveInvalidFilenameChars Path invalid = {Path.GetInvalidFileNameChars()}");
                filename = RemoveInvalidChars(filename, Path.GetInvalidFileNameChars());
            }
            if (filename.IndexOfAny(additional_invalid_chars) != -1)
            {
                filename = RemoveInvalidChars(filename, additional_invalid_chars);
            }
            Logger.Debug(() => $"FileSystemHelper:RemoveInvalidFilenameChars output = {filename}");
            return filename;
        }

        private static string RemoveInvalidChars(string filename, char[] invalid)
        {
            foreach (char invalidFileNameChar in invalid)
            {
                filename = filename.Replace(invalidFileNameChar, '_');
            }
            return filename;
        }
    }
}
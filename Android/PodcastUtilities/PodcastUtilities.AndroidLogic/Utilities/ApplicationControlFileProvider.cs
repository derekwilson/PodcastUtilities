using Android.Content;
using Android.OS;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.Common.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace PodcastUtilities.AndroidLogic.Utilities
{

    public interface IApplicationControlFileProvider
    {
        IReadWriteControlFile GetApplicationConfiguration();
        void ReplaceApplicationConfiguration(IReadWriteControlFile file);
        Intent GetApplicationConfigurationSharingIntent();
    }

    public class ApplicationControlFileProvider : IApplicationControlFileProvider
    {
        private IReadWriteControlFile ControlFile = null;
        // do not make this anything other than private
        private object SyncLock = new object();

        private ILogger Logger;
        private IFileSystemHelper FileSystemHelper;
        private IControlFileFactory ControlFileFactory;
        private IResourceProvider ResourceProvider;

        public ApplicationControlFileProvider(
            ILogger logger,
            IFileSystemHelper fileSystemHelper,
            IControlFileFactory factory,
            IResourceProvider resourceProvider)
        {
            Logger = logger;
            FileSystemHelper = fileSystemHelper;
            ControlFileFactory = factory;
            ResourceProvider = resourceProvider;
        }


        private string GetApplicationControlFilePath()
        {
            var folder = FileSystemHelper.GetApplicationFolderOnSdCard(IFileSystemHelper.CONFIG_FOLDER, true);
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

        public Intent GetApplicationConfigurationSharingIntent()
        {
            if (ControlFile == null)
            {
                Logger.Debug(() => $"ApplicationControlFileProvider:GetApplicationConfigurationSharingIntent - no controlfile");
                return null;
            }

            string controlFilename = GetApplicationControlFilePath();
            Android.Net.Uri uri = FileSystemHelper.GetAttachmentUri(controlFilename);
            var attachmentUris = new List<IParcelable>() { uri };
            var intent = GetSharingIntent(
                ResourceProvider.GetString(Resource.String.settings_share_all_subject),
                ResourceProvider.GetString(Resource.String.settings_share_all_body),
                attachmentUris);
            return intent;
        }

        private Intent GetSharingIntent(string subject, string shareText, List<IParcelable> attachmentUris)
        {
            //Intent sharingIntent = new Intent(Intent.ActionSend);
            Intent sharingIntent = new Intent(Intent.ActionSendMultiple);
            sharingIntent.SetType("vnd.android.cursor.dir/email");
            sharingIntent.PutExtra(Intent.ExtraSubject, subject);
            sharingIntent.PutExtra(Intent.ExtraText, shareText);
            sharingIntent.PutParcelableArrayListExtra(Intent.ExtraStream, attachmentUris);
            sharingIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
            return sharingIntent;
        }
    }
}
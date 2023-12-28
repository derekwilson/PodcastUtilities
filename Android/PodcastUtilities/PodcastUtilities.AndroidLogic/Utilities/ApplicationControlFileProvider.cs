using Android.Content;
using Android.OS;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.Common.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PodcastUtilities.AndroidLogic.Utilities
{

    public interface IApplicationControlFileProvider
    {
        /// <summary>
        /// event that is fired when the control file is updated
        /// the implementer must be a songleton for this to work properly
        /// </summary>
        event EventHandler<EventArgs> ConfigurationUpdated;

        IReadWriteControlFile GetApplicationConfiguration();
        void ReplaceApplicationConfiguration(IReadWriteControlFile file);
        Intent GetApplicationConfigurationSharingIntent();
        IReadWriteControlFile ResetControlFile();
        void SaveCurrentControlFile();
        bool SetFoldernameIfUnique(IPodcastInfo podcast, string foldername);
        bool AddPodcastIfFoldernameUnique(IPodcastInfo podcast);
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
        private IApplicationControlFileFactory ApplicationControlFileFactory;

        public event EventHandler<EventArgs> ConfigurationUpdated;

        public ApplicationControlFileProvider(
            ILogger logger,
            IFileSystemHelper fileSystemHelper,
            IControlFileFactory factory,
            IResourceProvider resourceProvider,
            IApplicationControlFileFactory applicationControlFileFactory
            )
        {
            Logger = logger;
            FileSystemHelper = fileSystemHelper;
            ControlFileFactory = factory;
            ResourceProvider = resourceProvider;
            ApplicationControlFileFactory = applicationControlFileFactory;
        }

        private void OnConfigurationUpdated()
        {
            ConfigurationUpdated?.Invoke( this, EventArgs.Empty );
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
                    ControlFile = ApplicationControlFileFactory.CreateEmptyControlFile();
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
            OnConfigurationUpdated();
        }

        public IReadWriteControlFile ResetControlFile()
        {
            Logger.Debug(() => $"ApplicationControlFileProvider:ResetControlFile");
            lock (SyncLock)
            {
                ControlFile = null;
                var newFile = ApplicationControlFileFactory.CreateEmptyControlFile();
                newFile.SaveToFile(GetApplicationControlFilePath());
                ControlFile = newFile;
            }
            OnConfigurationUpdated();
            return ControlFile;
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
                ResourceProvider.GetString(Resource.String.share_all_subject),
                ResourceProvider.GetString(Resource.String.share_all_body),
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

        public void SaveCurrentControlFile()
        {
            Logger.Debug(() => $"ApplicationControlFileProvider:SaveCurrentControlFile");
            lock (SyncLock)
            {
                if (ControlFile == null)
                {
                    Logger.Debug(() => $"ApplicationControlFileProvider: cannot save null file");
                    return;
                }
                ControlFile.SaveToFile(GetApplicationControlFilePath());
            }
            OnConfigurationUpdated();
        }

        public bool AddPodcastIfFoldernameUnique(IPodcastInfo podcast)
        {
            Logger.Debug(() => $"ApplicationControlFileProvider:AddPodcastIfFoldernameUnique - {podcast.Folder}");
            lock (SyncLock)
            {
                if (ControlFile == null)
                {
                    Logger.Debug(() => $"ApplicationControlFileProvider: IsFoldernameDuplicated - null control file");
                    return false;
                }
                var found = ControlFile.GetPodcasts().FirstOrDefault(item => item.Folder.Equals(podcast.Folder, StringComparison.InvariantCultureIgnoreCase));
                if (found != null)
                {
                    return false;
                }
                ControlFile.AddPodcast(podcast);
                ControlFile.SaveToFile(GetApplicationControlFilePath());
            }
            OnConfigurationUpdated();
            return true;
        }

        public bool SetFoldernameIfUnique(IPodcastInfo podcast, string foldername)
        {
            Logger.Debug(() => $"ApplicationControlFileProvider:SetFoldernameIfNotDuplicated - {foldername}");
            lock (SyncLock)
            {
                if (ControlFile == null)
                {
                    Logger.Debug(() => $"ApplicationControlFileProvider: SetFoldernameIfNotDuplicated - null control file");
                    return false;
                }
                // PU allows for things in a control file that does not make much sense for this app
                foreach (var thisPodcastInfo in ControlFile.GetPodcasts())
                {
                    // make sure each podcastinfo has a unique folder but do not compare with self
                    if (thisPodcastInfo != podcast && thisPodcastInfo.Folder.Equals(foldername, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Logger.Debug(() => $"ApplicationControlFileProvider: SetFoldernameIfNotDuplicated - duplicate foldername");
                        return false;
                    }
                }
                podcast.Folder = foldername;
                ControlFile.SaveToFile(GetApplicationControlFilePath());
            }
            OnConfigurationUpdated();
            return true;
        }
    }
}
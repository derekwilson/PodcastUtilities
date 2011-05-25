using System;
using System.Collections.Generic;
using System.IO;
using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// copy files in the file system
    /// </summary>
    public class FileCopier : IFileCopier
    {
    	///<summary>
    	/// construct the copier
    	///</summary>
    	///<param name="driveInfoProvider">abstract access to the file system drive</param>
    	///<param name="fileUtilities">abstract file utilities</param>
    	public FileCopier(
			IDriveInfoProvider driveInfoProvider,
			IFileUtilities fileUtilities)
    	{
    		DriveInfoProvider = driveInfoProvider;
    		FileUtilities = fileUtilities;
    	}

        /// <summary>
        /// event that is fired whenever a file is copied of an error occurs
        /// </summary>
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

    	private IDriveInfoProvider DriveInfoProvider { get; set; }
    	private IFileUtilities FileUtilities { get; set; }

        /// <summary>
        /// perform the copy operation
        /// </summary>
        /// <param name="sourceFiles">the list of SyncItem s to be copied</param>
        /// <param name="sourceRootPath">the root pathname of the source</param>
        /// <param name="destinationRootPath">the root pathname to the destination</param>
        /// <param name="freeSpaceToLeaveOnDestination">free space to meave on the destination in MB</param>
        /// <param name="whatif">true to emit all the status update but not actually perform the copy, false to do the copy</param>
        public void CopyFilesToTarget(
			List<FileSyncItem> sourceFiles,
			string sourceRootPath,
			string destinationRootPath,
			long freeSpaceToLeaveOnDestination,
			bool whatif)
        {
            foreach (FileSyncItem thisItem in sourceFiles)
            {
                string sourceRelativePath = thisItem.Source.FullName;
                string absRoot = Path.GetFullPath(sourceRootPath);
                if (sourceRelativePath.StartsWith(absRoot))
                {
                    sourceRelativePath = sourceRelativePath.Substring(absRoot.Length);
                }

                string destFilename = Path.GetFullPath(destinationRootPath) + sourceRelativePath;
                if (!FileUtilities.FileExists(destFilename))
                {
                    OnStatusUpdate(string.Format("Copying to: {0}", destFilename));
                    if (!whatif)
                    {
                        try
                        {
                            if (IsDestinationDriveFull(destinationRootPath, freeSpaceToLeaveOnDestination))
                                return;

                            FileUtilities.FileCopy(thisItem.Source.FullName, destFilename);
                            thisItem.DestinationPath = destFilename;
                            thisItem.Copied = true;
                        }
                        catch (IOException ex)
                        {
                            OnStatusUpdate(
                                new StatusUpdateEventArgs(
                                    StatusUpdateEventArgs.Level.Error, 
                                    string.Format("Error writing file: {0}", ex.Message)
                                )
                            );
                            return;
                        }
                    }
                }
            }
        }

        private bool IsDestinationDriveFull(string destinationRootPath, long freeSpaceToLeaveOnDestination)
        {
        	var driveInfo = DriveInfoProvider.GetDriveInfo(destinationRootPath);
        	long availableFreeSpace = driveInfo.AvailableFreeSpace;

			long freeKb = 0;
            double freeMb = 0;
            double freeGb = 0;
			if (availableFreeSpace > 0)
				freeKb = (availableFreeSpace / 1024);
            if (freeKb > 0)
                freeMb = (freeKb / 1024);
            if (freeMb > 0)
                freeGb = (freeMb / 1024);

            if (freeMb < freeSpaceToLeaveOnDestination)
            {
                OnStatusUpdate(string.Format("Destination drive is full leaving {0:#,0.##} MB free", freeSpaceToLeaveOnDestination));
                OnStatusUpdate(string.Format("Free Space on drive {0} is {1:#,0.##} KB, {2:#,0.##} MB, {3:#,0.##} GB", driveInfo.Name, freeKb, freeMb, freeGb));
                return true;
            }
            return false;
        }

		private void OnStatusUpdate(string message)
		{
			OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Status, message));
		}

		private void OnStatusUpdate(StatusUpdateEventArgs e)
		{
			if (StatusUpdate != null)
				StatusUpdate(this, e);
		}
	}
}

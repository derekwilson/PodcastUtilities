using System;
using System.Collections.Generic;
using System.IO;
using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common
{
    public class FileCopier : IFileCopier
    {
    	public FileCopier(
			IDriveInfoProvider driveInfoProvider,
			IFileUtilities fileUtilities)
    	{
    		DriveInfoProvider = driveInfoProvider;
    		FileUtilities = fileUtilities;
    	}

    	public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

    	private IDriveInfoProvider DriveInfoProvider { get; set; }
    	private IFileUtilities FileUtilities { get; set; }

    	private void OnStatusUpdate(string message)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Status, message));
        }

        private void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            if (StatusUpdate != null)
                StatusUpdate(this, e);
        }

        public void CopyFilesToTarget(
			List<SyncItem> sourceFiles,
			string sourceRootPath,
			string destinationRootPath,
			long freeSpaceToLeaveOnDestination,
			bool whatif)
        {
            foreach (SyncItem thisItem in sourceFiles)
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

			long freeKB = 0;
            double freeMB = 0;
            double freeGB = 0;
			if (availableFreeSpace > 0)
				freeKB = (availableFreeSpace / 1024);
            if (freeKB > 0)
                freeMB = (freeKB / 1024);
            if (freeMB > 0)
                freeGB = (freeMB / 1024);

            if (freeMB < freeSpaceToLeaveOnDestination)
            {
                OnStatusUpdate(string.Format("Destination drive is full leaving {0:#,0.##} MB free", freeSpaceToLeaveOnDestination));
                OnStatusUpdate(string.Format("Free Space on drive {0} is {1:#,0.##} KB, {2:#,0.##} MB, {3:#,0.##} GB", driveInfo.Name, freeKB, freeMB, freeGB));
                return true;
            }
            return false;
        }
    }
}

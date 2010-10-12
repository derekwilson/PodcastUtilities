using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PodcastUtilities.Common
{
    public class FileCopier
    {
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        private void OnStatusUpdate(string message)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Status, message));
        }

        private void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            if (StatusUpdate != null)
                StatusUpdate(this, e);
        }

        public void CopyFilesToTarget(List<SyncItem> sourceFiles, ControlFile control, bool whatif)
        {
            foreach (SyncItem thisItem in sourceFiles)
            {
                string sourceRelativePath = thisItem.Source.FullName;
                string absRoot = Path.GetFullPath(control.SourceRoot);
                if (sourceRelativePath.StartsWith(absRoot))
                {
                    sourceRelativePath = sourceRelativePath.Substring(absRoot.Length);
                }

                string destFilename = Path.GetFullPath(control.DestinationRoot) + sourceRelativePath;
                if (!File.Exists(destFilename))
                {
                    OnStatusUpdate(string.Format("Copying to: {0}", destFilename));
                    if (!whatif)
                    {
                        try
                        {
                            if (IsDestinationDriveFull(control))
                                return;

                            File.Copy(thisItem.Source.FullName, destFilename);
                            thisItem.DestinationPath = destFilename;
                            thisItem.Copied = true;
                        }
                        catch (System.IO.IOException ex)
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

        private bool IsDestinationDriveFull(ControlFile control)
        {
            DirectoryInfo dest = new DirectoryInfo(control.DestinationRoot);
            DriveInfo drive = new DriveInfo(dest.Root.FullName);
            long freeKB = 0;
            double freeMB = 0;
            double freeGB = 0;
            if (drive.AvailableFreeSpace > 0)
                freeKB = (drive.AvailableFreeSpace / 1024);
            if (freeKB > 0)
                freeMB = (freeKB / 1024);
            if (freeMB > 0)
                freeGB = (freeMB / 1024);

            if (freeMB < control.FreeSpaceToLeaveOnDestination)
            {
                OnStatusUpdate(string.Format("Destination drive is full leaving {0:#,0.##} MB free", control.FreeSpaceToLeaveOnDestination));
                OnStatusUpdate(string.Format("Free Space on drive {0} is {1:#,0.##} KB, {2:#,0.##} MB, {3:#,0.##} GB", drive.Name, freeKB, freeMB, freeGB));
                return true;
            }
            return false;
        }
    }
}

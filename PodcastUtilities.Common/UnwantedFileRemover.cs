using System;
using System.Collections.Generic;
using System.Linq;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// remove unwanted files, when files are removed from the source and we need to synchronise the delete
    /// </summary>
    public class UnwantedFileRemover : IUnwantedFileRemover
	{
		/// <summary>
		/// construct the remover
		/// </summary>
        ///<param name="directoryInfoProvider">abstract access to the file system</param>
        ///<param name="fileUtilities">abstract file utilities</param>
        public UnwantedFileRemover(
			IDirectoryInfoProvider directoryInfoProvider,
			IFileUtilities fileUtilities)
		{
			DirectoryInfoProvider = directoryInfoProvider;
			FileUtilities = fileUtilities;
		}

		private IDirectoryInfoProvider DirectoryInfoProvider { get; set; }
		private IFileUtilities FileUtilities { get; set; }

		#region Implementation of IUnwantedFileRemover

        /// <summary>
        /// event that is fired whenever a file is removed of an error occurs
        /// </summary>
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        /// <summary>
        /// remove the files that are not specified in the list of files to keep
        /// </summary>
        /// <param name="filesToKeep">the files to be kept</param>
        /// <param name="folderToRemoveFrom">folder to remove files from</param>
        /// <param name="pattern">file patter to look for eg. *.mp3</param>
        /// <param name="whatIf">true to emit all the status updates but not actually perform the deletes, false to do the delete</param>
        public void RemoveUnwantedFiles(IEnumerable<IFileInfo> filesToKeep, string folderToRemoveFrom, string pattern, bool whatIf)
		{
			var removeDirectory = DirectoryInfoProvider.GetDirectoryInfo(folderToRemoveFrom);
			if (!removeDirectory.Exists)
			{
				return;
			}

			var filesInDirectory = removeDirectory.GetFiles(pattern);

			foreach (var file in filesInDirectory)
			{
				var thisFile = file;
				if (!filesToKeep.Any(f => f.Name == thisFile.Name))
				{
					//we cannot find the file that is in the destination in the source
					OnStatusUpdate(string.Format("Removing: {0}", thisFile.FullName));
					if (!whatIf)
						FileUtilities.FileDelete(thisFile.FullName);
				}
			}
		}

		#endregion

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
using System.Collections.Generic;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Files
{
    /// <summary>
    /// supports the ability to remove unwanted files
    /// </summary>
    public interface IUnwantedFileRemover : IStatusUpdate
	{
		/// <summary>
		/// remove the files that are not specified in the list of files to keep
		/// </summary>
		/// <param name="filesToKeep">the files to be kept</param>
		/// <param name="folderToRemoveFrom">folder to remove files from</param>
		/// <param name="pattern">file patter to look for eg. *.mp3</param>
        /// <param name="whatIf">true to emit all the status updates but not actually perform the deletes, false to do the delete</param>
        void RemoveUnwantedFiles(IEnumerable<IFileInfo> filesToKeep, string folderToRemoveFrom, string pattern, bool whatIf);
	}
}

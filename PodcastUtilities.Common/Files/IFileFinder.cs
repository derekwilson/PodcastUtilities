using System.Collections.Generic;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Files
{
    /// <summary>
    /// supports the ability find files
    /// </summary>
    public interface IFileFinder
	{
		/// <summary>
        /// gets a number of files that match a given pattern
        /// </summary>
        /// <param name="folderPath">folder to look in</param>
        /// <param name="pattern">pattern to look for eg. *.mp3</param>
        /// <param name="maximumNumberOfFiles">maximum number of files to find</param>
		/// <param name="sortField">field to sort on</param>
		/// <param name="ascendingSort">true to sort ascending false to sort descending</param>
		/// <returns></returns>
        List<IFileInfo> GetFiles(
			string folderPath,
			string pattern,
			int maximumNumberOfFiles,
			string sortField,
		    bool ascendingSort);

		/// <summary>
		/// gets all the files that match a given pattern
		/// </summary>
		/// <param name="folderPath">folder to look in</param>
		/// <param name="pattern">pattern to look for eg. *.mp3</param>
		/// <returns></returns>
        List<IFileInfo> GetFiles(
			string folderPath,
			string pattern);
	}
}

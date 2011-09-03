using System.Collections.Generic;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Files
{
	/// <summary>
	/// supports the ability to sort files
	/// </summary>
    public interface IFileSorter
	{
		/// <summary>
		/// sort the supplied list of abstract files
		/// </summary>
		/// <param name="files">list of abstract files</param>
        /// <param name="sortField">field to sort on "creationtime" to use the file created time anything else to use the file name</param>
        /// <param name="ascendingSort">true to sort ascending false to sort descending</param>
        void Sort(List<IFileInfo> files, string sortField, bool ascendingSort);
	}
}

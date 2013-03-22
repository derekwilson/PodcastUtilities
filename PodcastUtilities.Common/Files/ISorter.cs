using System.Collections.Generic;
using System.Collections.ObjectModel;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Files
{
	/// <summary>
	/// supports the ability to sort files
	/// </summary>
    public interface ISorter
	{
	    /// <summary>
	    /// sort the supplied list of abstract files
	    /// </summary>
	    /// <param name="files">list of abstract files</param>
	    /// <param name="sortField">field to sort on "creationtime" to use the file created time anything else to use the file name</param>
	    /// <param name="ascendingSort">true to sort ascending false to sort descending</param>
	    IList<IFileInfo> Sort(IEnumerable<IFileInfo> files, PodcastFileSortField sortField, bool ascendingSort);
	}
}

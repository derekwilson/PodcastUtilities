using System;
using System.Collections.Generic;
using System.Globalization;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Files
{
	/// <summary>
	/// a file sorter
	/// </summary>
    public class Sorter : ISorter
	{
		#region Implementation of IFileSorter

	    /// <summary>
	    /// sort the supplied list of abstract files
	    /// </summary>
	    /// <param name="files">list of abstract files</param>
	    /// <param name="sortField">field to sort on "creationtime" to use the file created time anything else to use the file name</param>
	    /// <param name="ascendingSort">true to sort ascending false to sort descending</param>
	    public void Sort(List<IFileInfo> files, string sortField, bool ascendingSort)
		{
			switch (sortField.ToUpperInvariant())
			{
				case "CREATIONTIME":
					files.Sort((f1, f2) => f1.CreationTime.CompareTo(f2.CreationTime));
					break;

				default:
					files.Sort((f1, f2) => string.Compare(f1.Name, f2.Name,StringComparison.OrdinalIgnoreCase));
					break;
			}

			if (!ascendingSort)
			{
				files.Reverse();
			}
		}

		#endregion
	}
}
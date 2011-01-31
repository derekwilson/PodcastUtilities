using System.Collections.Generic;
using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common
{
	public class FileSorter : IFileSorter
	{
		#region Implementation of IFileSorter

		public void Sort(List<IFileInfo> files, string sortField, bool ascendingSort)
		{
			switch (sortField.ToLower())
			{
				case "creationtime":
					files.Sort((f1, f2) => f1.CreationTime.CompareTo(f2.CreationTime));
					break;

				default:
					files.Sort((f1, f2) => f1.Name.CompareTo(f2.Name));
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
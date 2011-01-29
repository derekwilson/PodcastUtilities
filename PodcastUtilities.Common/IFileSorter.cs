using System.Collections.Generic;
using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common
{
	public interface IFileSorter
	{
		void Sort(List<IFileInfo> files, string sortField, bool ascendingSort);
	}
}

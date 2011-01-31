using System.Collections.Generic;
using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common
{
	public interface IFileFinder
	{
		List<IFileInfo> GetFiles(
			string folderPath,
			string pattern,
			int maximumNumberOfFiles,
			string sortField,
		    bool ascendingSort);

		List<IFileInfo> GetFiles(
			string folderPath,
			string pattern);
	}
}

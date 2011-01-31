using System;
using System.Collections.Generic;
using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common
{
	public interface IUnwantedFileRemover
	{
		event EventHandler<StatusUpdateEventArgs> StatusUpdate;

		void RemoveUnwantedFiles(IEnumerable<IFileInfo> filesToKeep, string folderToRemoveFrom, string pattern, bool whatif);
	}
}

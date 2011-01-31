using System;
using System.Collections.Generic;

namespace PodcastUtilities.Common
{
	public interface IFileCopier
	{
		event EventHandler<StatusUpdateEventArgs> StatusUpdate;

		void CopyFilesToTarget(
			List<SyncItem> sourceFiles,
			string sourceRootPath,
			string destinationRootPath,
			long freeSpaceToLeaveOnDestination,
			bool whatif);
	}
}
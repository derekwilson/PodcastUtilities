using System.Collections.Generic;

namespace PodcastUtilities.Common.Files
{
    /// <summary>
    /// supports the ability to copy a number of SyncItem
    /// </summary>
    public interface ICopier : IStatusUpdate
	{
        /// <summary>
        /// perform the copy operation
        /// </summary>
        /// <param name="sourceFiles">the list of SyncItem s to be copied</param>
        /// <param name="sourceRootPath">the root pathname of the source</param>
        /// <param name="destinationRootPath">the root pathname to the destination</param>
        /// <param name="freeSpaceToLeaveOnDestination">free space to meave on the destination in MB</param>
        /// <param name="whatif">true to emit all the status updates but not actually perform the copy, false to do the copy</param>
        void CopyFilesToTarget(IEnumerable<FileSyncItem> sourceFiles, string sourceRootPath, string destinationRootPath, long freeSpaceToLeaveOnDestination, bool whatif);
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common
{
	public class UnwantedFileRemover : IUnwantedFileRemover
	{
		public UnwantedFileRemover(
			IDirectoryInfoProvider directoryInfoProvider,
			IFileUtilities fileUtilities)
		{
			DirectoryInfoProvider = directoryInfoProvider;
			FileUtilities = fileUtilities;
		}

		private IDirectoryInfoProvider DirectoryInfoProvider { get; set; }
		private IFileUtilities FileUtilities { get; set; }

		#region Implementation of IUnwantedFileRemover

		public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

		public void RemoveUnwantedFiles(IEnumerable<IFileInfo> filesToKeep, string folderToRemoveFrom, string pattern, bool whatif)
		{
			var removeDirectory = DirectoryInfoProvider.GetDirectoryInfo(folderToRemoveFrom);
			if (!removeDirectory.Exists)
			{
				return;
			}

			var filesInDirectory = removeDirectory.GetFiles(pattern);

			foreach (var file in filesInDirectory)
			{
				var thisFile = file;
				if (!filesToKeep.Any(f => f.Name == thisFile.Name))
				{
					//we cannot find the file that is in the destination in the source
					OnStatusUpdate(string.Format("Removing: {0}", thisFile.FullName));
					if (!whatif)
						FileUtilities.FileDelete(thisFile.FullName);
				}
			}
		}

		#endregion

		private void OnStatusUpdate(string message)
		{
			OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Status, message));
		}

		private void OnStatusUpdate(StatusUpdateEventArgs e)
		{
			if (StatusUpdate != null)
				StatusUpdate(this, e);
		}
	}
}
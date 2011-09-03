using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Files
{
	/// <summary>
	/// synchronise the files from the podcasts in a control file
	/// </summary>
    public class Synchronizer
	{
		/// <summary>
		/// construct a podcast synchroniser
		/// </summary>
		/// <param name="fileFinder">abstract interface to the file system to find media files</param>
		/// <param name="fileCopier">abstract file copier</param>
		/// <param name="fileRemover">abstract file remover, to synchronise deleted files in the source to the destination</param>
        public Synchronizer(
			IFinder fileFinder,
			ICopier fileCopier,
			IUnwantedFileRemover fileRemover)
		{
			FileFinder = fileFinder;
			FileCopier = fileCopier;
			FileRemover = fileRemover;
		}

        /// <summary>
        /// event that is fired whenever as each operation is performed or if an error occurs
        /// </summary>
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate
		{
			add
			{
				FileCopier.StatusUpdate += value;
				FileRemover.StatusUpdate += value;
			}
			remove
			{
				FileCopier.StatusUpdate -= value;
				FileRemover.StatusUpdate -= value;
			}
		}

		private IFinder FileFinder { get; set; }
		private ICopier FileCopier { get; set; }
		private IUnwantedFileRemover FileRemover { get; set; }

		/// <summary>
		/// synchronise podcast media files
		/// </summary>
		/// <param name="controlFile">control file to use to control the process</param>
		/// <param name="whatIf">true to generate the status messages but not to actually perform the file copy / deletes</param>
        public void Synchronize(IControlFile controlFile, bool whatIf)
		{
			var filesToCopy = new List<FileSyncItem>();

			foreach (var podcast in controlFile.Podcasts)
			{
				var podcastSourcePath = Path.Combine(controlFile.SourceRoot, podcast.Folder);
				var podcastDestinationPath = Path.Combine(controlFile.DestinationRoot, podcast.Folder);

				var podcastSourceFiles = FileFinder.GetFiles(
						podcastSourcePath,
						podcast.Pattern,
						podcast.MaximumNumberOfFiles,
						podcast.SortField,
						podcast.AscendingSort);

				FileRemover.RemoveUnwantedFiles(podcastSourceFiles, podcastDestinationPath, podcast.Pattern, whatIf);

				var podcastSyncItems = podcastSourceFiles.Select(p => new FileSyncItem {Source = p});

				filesToCopy.AddRange(podcastSyncItems);
			}

			FileCopier.CopyFilesToTarget(
				filesToCopy,
				controlFile.SourceRoot,
				controlFile.DestinationRoot,
				controlFile.FreeSpaceToLeaveOnDestination,
				whatIf);
		}
	}
}

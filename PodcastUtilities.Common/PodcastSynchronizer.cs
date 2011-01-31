using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PodcastUtilities.Common
{
	public class PodcastSynchronizer
	{
		public PodcastSynchronizer(
			IFileFinder fileFinder,
			IFileCopier fileCopier,
			IUnwantedFileRemover fileRemover)
		{
			FileFinder = fileFinder;
			FileCopier = fileCopier;
			FileRemover = fileRemover;
		}

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

		private IFileFinder FileFinder { get; set; }
		private IFileCopier FileCopier { get; set; }
		private IUnwantedFileRemover FileRemover { get; set; }

		public void Synchronize(IControlFile controlFile, bool whatIf)
		{
			var filesToCopy = new List<SyncItem>();

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

				var podcastSyncItems = podcastSourceFiles.Select(p => new SyncItem {Source = p});

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

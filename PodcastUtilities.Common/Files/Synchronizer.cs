#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Platform;

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
	    /// <param name="folderRemover">abstract folder remover, to remove folder that have gone empty during sync</param>
	    public Synchronizer(
			IFinder fileFinder,
			ICopier fileCopier,
			IUnwantedFileRemover fileRemover,
            IUnwantedFolderRemover folderRemover)
		{
			FileFinder = fileFinder;
			FileCopier = fileCopier;
			FileRemover = fileRemover;
		    FolderRemover = folderRemover;
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
			    FolderRemover.StatusUpdate += value;
			}
			remove
			{
				FileCopier.StatusUpdate -= value;
				FileRemover.StatusUpdate -= value;
			    FolderRemover.StatusUpdate -= value;
			}
		}

		private IFinder FileFinder { get; set; }
		private ICopier FileCopier { get; set; }
		private IUnwantedFileRemover FileRemover { get; set; }
        private IUnwantedFolderRemover FolderRemover { get; set; }

		/// <summary>
		/// synchronise podcast media files
		/// </summary>
		/// <param name="controlFile">control file to use to control the process</param>
		/// <param name="whatIf">true to generate the status messages but not to actually perform the file copy / deletes</param>
        public void Synchronize(IReadOnlyControlFile controlFile, bool whatIf)
		{
			var filesToCopy = new List<FileSyncItem>();

			foreach (PodcastInfo podcast in controlFile.GetPodcasts())
			{
				string podcastSourcePath = Path.Combine(controlFile.GetSourceRoot(), podcast.Folder);
				string podcastDestinationPath = Path.Combine(controlFile.GetDestinationRoot(), podcast.Folder);

				IList<IFileInfo> podcastSourceFiles = FileFinder.GetFiles(
						podcastSourcePath,
						podcast.Pattern.Value,
						podcast.MaximumNumberOfFiles.Value,
						podcast.SortField.Value,
						podcast.AscendingSort.Value);

				FileRemover.RemoveUnwantedFiles(podcastSourceFiles, podcastDestinationPath, podcast.Pattern.Value, whatIf);

				IEnumerable<FileSyncItem> podcastSyncItems = podcastSourceFiles.Select(p => new FileSyncItem {Source = p});

				filesToCopy.AddRange(podcastSyncItems);
			}

			FileCopier.CopyFilesToTarget(
				filesToCopy,
				controlFile.GetSourceRoot(),
				controlFile.GetDestinationRoot(),
				controlFile.GetFreeSpaceToLeaveOnDestination(),
				whatIf);

		    foreach (PodcastInfo podcast in controlFile.GetPodcasts())
		    {
		        if (podcast.DeleteEmptyFolder.Value)
		        {
                    string podcastDestinationPath = Path.Combine(controlFile.GetDestinationRoot(), podcast.Folder);
                    FolderRemover.RemoveFolderIfEmpty(podcastDestinationPath, whatIf);
		        }
		    }
		}
	}
}

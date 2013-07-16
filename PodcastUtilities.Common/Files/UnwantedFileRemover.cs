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
using System.Globalization;
using System.Linq;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Files
{
    /// <summary>
    /// remove unwanted files, when files are removed from the source and we need to synchronise the delete
    /// </summary>
    public class UnwantedFileRemover : IUnwantedFileRemover
	{
		/// <summary>
		/// construct the remover
		/// </summary>
        ///<param name="directoryInfoProvider">abstract access to the file system</param>
        ///<param name="fileUtilities">abstract file utilities</param>
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

        /// <summary>
        /// event that is fired whenever a file is removed of an error occurs
        /// </summary>
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        /// <summary>
        /// remove the files that are not specified in the list of files to keep
        /// </summary>
        /// <param name="filesToKeep">the files to be kept</param>
        /// <param name="folderToRemoveFrom">folder to remove files from</param>
        /// <param name="pattern">file patter to look for eg. *.mp3</param>
        /// <param name="whatIf">true to emit all the status updates but not actually perform the deletes, false to do the delete</param>
        public void RemoveUnwantedFiles(IEnumerable<IFileInfo> filesToKeep, string folderToRemoveFrom, string pattern, bool whatIf)
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
					OnStatusUpdate(string.Format(CultureInfo.InvariantCulture,"Removing: {0}", thisFile.FullName));
					if (!whatIf)
						FileUtilities.FileDelete(thisFile.FullName);
				}
			}
		}

		#endregion

		private void OnStatusUpdate(string message)
		{
			OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateLevel.Status, message));
		}

		private void OnStatusUpdate(StatusUpdateEventArgs e)
		{
			if (StatusUpdate != null)
				StatusUpdate(this, e);
		}
	}
}
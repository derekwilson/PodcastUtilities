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
using System.Globalization;
using System.IO;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Files
{
    /// <summary>
    /// support the ability to remove unwanted empty folders
    /// </summary>
    public class UnwantedFolderRemover : IUnwantedFolderRemover
    {
        private readonly IDirectoryInfoProvider _directoryInfoProvider;
        private readonly IFileUtilities _fileUtilities;

        /// <summary>
        /// event that is fired whenever a file is removed of an error occurs
        /// </summary>
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        /// <summary>
        /// construct the removed
        /// </summary>
        ///<param name="directoryInfoProvider">abstract access to the file system</param>
        ///<param name="fileUtilities">abstract file utilities</param>
        public UnwantedFolderRemover(IDirectoryInfoProvider directoryInfoProvider, IFileUtilities fileUtilities)
        {
            _directoryInfoProvider = directoryInfoProvider;
            _fileUtilities = fileUtilities;
        }

        /// <summary>
        /// remove a folder if there are no files in it, used by the synchroniser
        /// </summary>
        /// <param name="folder">folder to delete</param>
        /// <param name="whatIf">true to emit all the status updates but not actually perform the deletes, false to do the delete</param>
        public void RemoveFolderIfEmpty(string folder, bool whatIf)
        {
            IDirectoryInfo directoryInfo = _directoryInfoProvider.GetDirectoryInfo(folder);

            if (!directoryInfo.Exists)
            {
                return;
            }

            IDirectoryInfo[] subFolders;
            try
            {
                subFolders = directoryInfo.GetDirectories("*.*");
            }
            catch (DirectoryNotFoundException)
            {
                // if the folder is not there then there is nothing to do
                return;
            }

            if (subFolders != null && subFolders.Length > 0)
            {
                // the folder is not empty - there are subfolders
                return;
            }

            IFileInfo[] files;
            try
            {
                files = directoryInfo.GetFiles("*.*");
            }
            catch (DirectoryNotFoundException)
            {
                // if the folder is not there then there is nothing to do
                return;
            }

            if (files != null && files.Length > 0)
            {
                // the folder is not empty there are files
                return;
            }

            OnStatusUpdate(string.Format(CultureInfo.InvariantCulture, "Removing folder: {0}", directoryInfo.FullName));
            if (!whatIf)
            {
                directoryInfo.Delete();
            }
        }

        private void OnStatusUpdate(string message)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateLevel.Status, message, false, null));
        }

        private void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            StatusUpdate?.Invoke(this, e);
        }
    }
}
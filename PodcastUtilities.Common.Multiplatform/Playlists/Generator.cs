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
using System.Linq;
using System.IO;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using System.Collections.Generic;

namespace PodcastUtilities.Common.Playlists
{

    /// <summary>
    /// generate a playlist
    /// </summary>
    public class Generator : IGenerator
    {
        /// <summary>
        /// create a playlist generator
        /// </summary>
        /// <param name="fileFinder">abstract access to the file system to find the files for the playlist</param>
        /// <param name="fileUtilities">abstract file utilities</param>
        /// <param name="pathUtilities">abstract path utilities</param>
        /// <param name="playlistFactory">factpry to generate the correct playlist object depending upon the selected format</param>
        public Generator(
			IFinder fileFinder,
            IFileUtilities fileUtilities,
            IPathUtilities pathUtilities,
			IPlaylistFactory playlistFactory)
    	{
    		FileFinder = fileFinder;
    		FileUtilities = fileUtilities;
            PathUtilities = pathUtilities;
            PlaylistFactory = playlistFactory;
    	}

        /// <summary>
        /// event that is fired when the playlist is generated or copied
        /// </summary>
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

    	private IFinder FileFinder { get; set; }
    	private IFileUtilities FileUtilities { get; set; }
        private IPathUtilities PathUtilities { get; set; }
        private IPlaylistFactory PlaylistFactory { get; set; }

    	private void OnStatusUpdate(string message, Boolean complete)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateLevel.Status, message, complete, null));
        }

        private void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            StatusUpdate?.Invoke(this, e);
        }

        /// <summary>
        /// generate a playlist
        /// </summary>
        /// <param name="control">control file to use to find the podcasts and playlist format</param>
        /// <param name="rootFolder">root folder to find episodes</param>
        /// <param name="copyToDestination">true to copy the playlist to the root folder, false to write it with the control file</param>
        private void GeneratePlaylist(IReadOnlyControlFile control, string rootFolder, bool copyToDestination)
        {
            IEnumerable<IFileInfo> allDestFiles = control.GetPodcasts().SelectMany(
        		podcast => FileFinder.GetFiles(Path.Combine(rootFolder, podcast.Folder), podcast.Pattern.Value));
            List<IFileInfo> allDestFilesSorted = allDestFiles.OrderBy(item => item.FullName).ToList();

            IPlaylist p = PlaylistFactory.CreatePlaylist(control.GetPlaylistFormat(), control.GetPlaylistFileName());

			string pathSeparator = PathUtilities.GetPathSeparator().ToString();
            foreach (IFileInfo thisFile in allDestFilesSorted)
            {
                string thisRelativeFile = thisFile.FullName;
                string absRoot = PathUtilities.GetFullPath(rootFolder);
                if (thisRelativeFile.StartsWith(absRoot,StringComparison.Ordinal))
                {
                    thisRelativeFile = thisRelativeFile.Substring(absRoot.Length);
                }
				thisRelativeFile = thisRelativeFile.Replace(pathSeparator, control.GetPlaylistPathSeparator());
                p.AddTrack("." + thisRelativeFile);
            }

            var tempFile = PathUtilities.GetTempFileName();
            OnStatusUpdate(string.Format(CultureInfo.InvariantCulture, "Generating Playlist with {0} items", p.NumberOfTracks), false);

            p.SaveFile(tempFile);

            var destPlaylist = copyToDestination
                                   ? Path.Combine(rootFolder, control.GetPlaylistFileName())
                                   : control.GetPlaylistFileName();

            OnStatusUpdate(string.Format(CultureInfo.InvariantCulture, "Writing playlist to {0}", destPlaylist), true);

            FileUtilities.FileCopy(tempFile, destPlaylist, true);
        }

        /// <summary>
        /// generate a playlist for the files in the destination folder
        /// </summary>
        /// <param name="control">control file to use to find the destinationRoot, and playlist format</param>
        /// <param name="copyToDestination">true to copy the playlist to the destination, false to write it locally</param>
        public void GeneratePlaylist(IReadOnlyControlFile control, bool copyToDestination)
        {
            GeneratePlaylist(control, control.GetDestinationRoot(), copyToDestination);
        }

        /// <summary>
        /// generate a playlist for the files in the destination folder
        /// </summary>
        /// <param name="control">control file to use to find the destinationRoot, and playlist format</param>
        /// <param name="rootFolder">root folder to find episodes</param>
        /// <param name="copyToDestination">true to copy the playlist to the destination, false to write it locally</param>
        /// <param name="statusUpdate">the update mechanism for the generation - can be null</param>
        public void GeneratePlaylist(IReadOnlyControlFile control, string rootFolder, bool copyToDestination, EventHandler<StatusUpdateEventArgs> statusUpdate)
        {
            if (statusUpdate != null)
            {
                StatusUpdate += statusUpdate;
            }
            GeneratePlaylist(control, rootFolder, copyToDestination);
        }
    }
}

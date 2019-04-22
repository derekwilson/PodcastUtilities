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
namespace PodcastUtilities.Common.Playlists
{
    /// <summary>
    /// supports the functionality needed to manage a playlist
    /// </summary>
    public interface IPlaylist
    {
        /// <summary>
        /// Add a track to the playlist
        /// </summary>
        /// <param name="filePath">pathname to add, can be relative or absolute</param>
        /// <returns>true if the file was added false if the track was already present</returns>
        bool AddTrack(string filePath);

        /// <summary>
        /// filename to use when saving the playlist file
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// number of tracks in the playlist
        /// </summary>
        int NumberOfTracks { get; }

        /// <summary>
        /// persist the playlist to disk
        /// </summary>
        void SaveFile();

        /// <summary>
        /// persist the playlist to disk
        /// </summary>
        void SaveFile(string overrideFilename);

        /// <summary>
        /// the title of the playlist
        /// </summary>
        string Title { get; set; }
    }
}

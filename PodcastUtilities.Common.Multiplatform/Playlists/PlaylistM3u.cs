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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PodcastUtilities.Common.Playlists
{
	/// <summary>
	/// M3U playlist
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
	public class PlaylistM3u : IPlaylist
	{
		private bool create;
		private List<string> mediaReferences;

		/// <summary>
		/// create a new playlist object
		/// </summary>
		/// <param name="fileName">filename that will be used to save the file, and possibly load an existing playlist from</param>
		/// <param name="create">true to load a blank template playlist false to load an existing playlist from disk</param>
		public PlaylistM3u(string fileName, bool create)
		{
			this.FileName = fileName;
			this.create = create;
			this.mediaReferences = new List<string>(10);
			if (create)
			{
				Title = Path.GetFileNameWithoutExtension(fileName);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// filename to use when saving the playlist file
		/// </summary>
		public string FileName { get; private set; }

		/// <summary>
		/// number of tracks in the playlist
		/// </summary>
		public int NumberOfTracks { get { return mediaReferences.Count; } }

		/// <summary>
		/// the title of the playlist
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Add a track to the playlist
		/// </summary>
		/// <param name="filePath">pathname to add, can be relative or absolute</param>
		/// <returns>true if the file was added false if the track was already present</returns>
		public bool AddTrack(string filePath)
		{
			if (mediaReferences.Contains(filePath))
			{
				return false;
			}
			mediaReferences.Add(filePath);
			return true;
		}

		/// <summary>
		/// persist the playlist to disk
		/// </summary>
		public void SaveFile()
		{
			SaveFile(FileName);
		}

		/// <summary>
		/// persist the playlist to disk
		/// </summary>
		public void SaveFile(string overrideFilename)
		{
			Assembly me = System.Reflection.Assembly.GetExecutingAssembly();
			AssemblyName name = me.GetName();

			using (TextWriter write = new StreamWriter(overrideFilename))
			{
				write.WriteLine("# created by PodcastUtilities v{0}", name.Version);
				foreach (String s in mediaReferences)
					write.WriteLine(s);
			}
		}
	}
}

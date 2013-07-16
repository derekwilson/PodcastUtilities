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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Xml.XPath;
using PodcastUtilities.Common.Exceptions;

namespace PodcastUtilities.Common.Playlists
{
	/// <summary>
	/// ASX playlist
	/// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class PlaylistAsx : XmlFileBase, IPlaylist
	{
        /// <summary>
        /// the resource path to the ASX template
        /// </summary>
        public const string EmptyAsxResource = "PodcastUtilities.Common.XML.asxPlaylist.xml";

		/// <summary>
		/// create a new playlist object
		/// </summary>
        /// <param name="fileName">filename that will be used to save the file, and possibly load an existing playlist from</param>
        /// <param name="create">true to load a blank template playlist false to load an existing playlist from disk</param>
        public PlaylistAsx(string fileName, bool create)
			: base(fileName, create, EmptyAsxResource, Assembly.GetExecutingAssembly())
		{
			if (create)
			{
				Title = Path.GetFileNameWithoutExtension(fileName);
			}
		}

	    /// <summary>
	    /// number of tracks in the playlist
	    /// </summary>
	    public int NumberOfTracks
		{
			get
			{
                return GetNumberOfNodes("ASX/ENTRY/REF");
			}
		}

	    /// <summary>
	    /// the title of the playlist
	    /// </summary>
	    public string Title
		{
			get { return GetNodeText("ASX/TITLE"); }
			set { SetNodeText("ASX/TITLE", value); }
		}

	    /// <summary>
	    /// Add a track to the playlist
	    /// </summary>
        /// <param name="filePath">pathname to add, can be relative or absolute</param>
	    /// <returns>true if the file was added false if the track was already present</returns>
	    public bool AddTrack(string filePath)
		{
            var encodedFilePath = XmlEncodeString(filePath);

            if (GetNumberOfNodes(string.Format(CultureInfo.InvariantCulture, "ASX/ENTRY/REF[@HREF = '{0}']", encodedFilePath)) > 0)
				return false;

            IXPathNavigable n = FindNode("ASX");
			if (n == null)
			{
                throw new XmlStructureException("AddTrack : ASX : path not found");
			}

			// we can find the parent node for the keys so create the key element
            n.CreateNavigator().AppendChild(string.Format(CultureInfo.InvariantCulture, "<ENTRY><REF HREF='{0}' /></ENTRY>", encodedFilePath)); 

            return true;
		}
	}
}

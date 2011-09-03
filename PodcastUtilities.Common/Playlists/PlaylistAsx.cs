using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.IO;
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
				XmlNodeList list = SelectNodes("ASX/ENTRY/REF");
				if (list != null)
					return list.Count;

				return 0;
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
            XmlNode n = SelectSingleNode(string.Format(CultureInfo.InvariantCulture,"ASX/ENTRY/REF[@HREF = '{0}']", filePath));
			if (n != null)
				return false;

			n = SelectSingleNode("ASX");
			if (n == null)
			{
                throw new XmlStructureException("AddTrack : ASX : path not found");
			}

			// we can find the parent node for the keys so create the key element
			XmlNode newParent = CreateElement("ENTRY");
			XmlNode newNode = CreateElement("REF");
			XmlAttribute attr = CreateAttribute("HREF");
            attr.Value = filePath;
			newNode.Attributes.Append(attr);
			newParent.AppendChild(newNode);
			n.AppendChild(newParent);
			return true;
		}
	}
}

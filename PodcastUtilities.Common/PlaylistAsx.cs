using System.Reflection;
using System.Xml;
using System.IO;

namespace PodcastUtilities.Common
{
	/// <summary>
	/// ASX playlist
	/// </summary>
    public class PlaylistAsx : XmlFileBase, IPlaylist
	{
        /// <summary>
        /// the resource path to the ASX template
        /// </summary>
        public static string EmptyAsxResource = "PodcastUtilities.Common.XML.asxPlaylist.xml";

		/// <summary>
		/// create a new playlist object
		/// </summary>
        /// <param name="filename">filename that will be used to save the file, and possibly load an existing playlist from</param>
        /// <param name="create">true to load a blank template playlist false to load an existing playlist from disk</param>
        public PlaylistAsx(string filename, bool create)
			: base(filename, create, EmptyAsxResource, Assembly.GetExecutingAssembly())
		{
			if (create)
			{
				Title = Path.GetFileNameWithoutExtension(filename);
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
	    /// <param name="filepath">pathname to add, can be relative or absolute</param>
	    /// <returns>true if the file was added false if the track was already present</returns>
	    public bool AddTrack(string filepath)
		{
			XmlNode n = SelectSingleNode(string.Format("ASX/ENTRY/REF[@HREF = '{0}']", filepath));
			if (n != null)
				return false;

			n = SelectSingleNode("ASX");
			if (n == null)
			{
				throw new System.Exception("AddTrack : ASX : path not found");
			}

			// we can find the parent node for the keys so create the key element
			XmlNode newParent = CreateElement("ENTRY");
			XmlNode newNode = CreateElement("REF");
			XmlAttribute attr = CreateAttribute("HREF");
			attr.Value = filepath;
			newNode.Attributes.Append(attr);
			newParent.AppendChild(newNode);
			n.AppendChild(newParent);
			return true;
		}
	}
}

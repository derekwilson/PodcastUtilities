using System.IO;
using System.Reflection;
using System.Xml;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// a windows playlist
    /// </summary>
    public class PlaylistWpl : XmlFileBase, IPlaylist
	{
        /// <summary>
        /// resource path to the WPL template
        /// </summary>
        public static string EmptyWplResource = "PodcastUtilities.Common.XML.wplPlaylist.xml";
		
		/// <summary>
		/// create a playlist object
		/// </summary>
		/// <param name="filename">filename that will be used to save the file, and possibly load an existing playlist from</param>
		/// <param name="create">true to load a blank template playlist false to load an existing playlist from disk</param>
        public PlaylistWpl(string filename, bool create)
			: base(filename, create, EmptyWplResource, Assembly.GetExecutingAssembly())
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
				XmlNodeList list = SelectNodes("smil/body/seq/media");
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
			get { return GetNodeText("smil/head/title"); }
			set { SetNodeText("smil/head/title", value); }
		}

        /// <summary>
        /// Add a track to the playlist
        /// </summary>
        /// <param name="filepath">pathname to add, can be relative or absolute</param>
        /// <returns>true if the file was added false if the track was already present</returns>
        public bool AddTrack(string filepath)
		{
			XmlNode n = SelectSingleNode(string.Format("smil/body/seq/media[@src = '{0}']",filepath));
			if (n != null)
				return false;

			n = SelectSingleNode("smil/body/seq");
			if (n == null)
			{
				throw new System.Exception("AddTrack : smil/body/seq : path not found");
			}

			// we can find the parent node for the keys so create the key element
			XmlNode newNode = CreateElement("media");
			XmlAttribute attr = CreateAttribute("src");
			attr.Value = filepath;
			newNode.Attributes.Append(attr);
			n.AppendChild(newNode); 
			return true;
		}
	}
}

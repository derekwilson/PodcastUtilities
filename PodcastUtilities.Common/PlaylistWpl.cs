using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using PodcastUtilities.Common.Exceptions;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// a windows playlist
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class PlaylistWpl : XmlFileBase, IPlaylist
	{
        /// <summary>
        /// resource path to the WPL template
        /// </summary>
        public const string EmptyWplResource = "PodcastUtilities.Common.XML.wplPlaylist.xml";
		
		/// <summary>
		/// create a playlist object
		/// </summary>
		/// <param name="fileName">filename that will be used to save the file, and possibly load an existing playlist from</param>
		/// <param name="create">true to load a blank template playlist false to load an existing playlist from disk</param>
        public PlaylistWpl(string fileName, bool create)
			: base(fileName, create, EmptyWplResource, Assembly.GetExecutingAssembly())
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
        /// <param name="filePath">pathname to add, can be relative or absolute</param>
        /// <returns>true if the file was added false if the track was already present</returns>
        public bool AddTrack(string filePath)
		{
            XmlNode n = SelectSingleNode(string.Format(CultureInfo.InvariantCulture,"smil/body/seq/media[@src = '{0}']", filePath));
			if (n != null)
				return false;

			n = SelectSingleNode("smil/body/seq");
			if (n == null)
			{
				throw new XmlStructureException("AddTrack : smil/body/seq : path not found");
			}

			// we can find the parent node for the keys so create the key element
			XmlNode newNode = CreateElement("media");
			XmlAttribute attr = CreateAttribute("src");
            attr.Value = filePath;
			newNode.Attributes.Append(attr);
			n.AppendChild(newNode); 
			return true;
		}
	}
}

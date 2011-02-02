using System.Xml;
using System.IO;

namespace PodcastUtilities.Common
{
	public class PlaylistAsx : XmlFileBase, IPlaylist
	{
        public static string EmptyAsxResource = "PodcastUtilities.Common.XML.asxPlaylist.xml";

		public PlaylistAsx(string filename, bool create)
			: base(filename, create, EmptyAsxResource)
		{
			if (create)
			{
				Title = Path.GetFileNameWithoutExtension(filename);
			}
		}

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

		public string Title
		{
			get { return GetNodeText("ASX/TITLE"); }
			set { SetNodeText("ASX/TITLE", value); }
		}

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

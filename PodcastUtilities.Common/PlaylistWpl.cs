using System.IO;
using System.Reflection;
using System.Xml;

namespace PodcastUtilities.Common
{
	public class PlaylistWpl : XmlFileBase, IPlaylist
	{
        public static string EmptyWplResource = "PodcastUtilities.Common.XML.wplPlaylist.xml";
		
		public PlaylistWpl(string filename, bool create)
			: base(filename, create, EmptyWplResource, Assembly.GetExecutingAssembly())
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
				XmlNodeList list = SelectNodes("smil/body/seq/media");
				if (list != null)
					return list.Count;

				return 0;
			}
		}

		public string Title
		{
			get { return GetNodeText("smil/head/title"); }
			set { SetNodeText("smil/head/title", value); }
		}		
		
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

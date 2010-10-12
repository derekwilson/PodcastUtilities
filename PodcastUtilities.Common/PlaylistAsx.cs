using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;

namespace PodcastUtilities.Common
{
    public class PlaylistAsx : XmlDocument, IPlaylist
	{
        public static string C_ASX_XML = "PodcastUtilities.Common.XML.asxPlaylist.xml";

		private string _filename;

		public PlaylistAsx(string filename, bool create)
		{
			_filename = filename;

			if (create)
			{
				Load(GetXmlStream(C_ASX_XML));
				Title = System.IO.Path.GetFileNameWithoutExtension(filename);
			}
			else
				Load(_filename);
		}

		protected Stream GetXmlStream(string xmlfile)
		{
			Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(xmlfile);
			return s;
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

		public string Filename
		{
			get
			{
				return _filename;
			}
		}

		private string GetNodeText(string xpath)
		{
			XmlNode n = SelectSingleNode(xpath);
			if (n == null)
			{
				throw new System.Exception("GetNodeText : Node path '" + xpath + "' not found");
			}
			return n.InnerText;
		}

		private void SetNodeText(string xpath, string val)
		{
			XmlNode n = SelectSingleNode(xpath);
			if (n == null)
			{
				throw new System.Exception("SetNodeText : Node path '" + xpath + "' not found");
			}
			n.InnerText = val;
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

		public void SavePlaylist()
		{
			this.Save(_filename);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;

namespace PodcastUtilities.Common
{
	public class PlaylistWpl : XmlDocument, IPlaylist
	{
        public static string C_WPL_XML = "PodcastUtilities.Common.XML.wplPlaylist.xml";
		
		private string _filename;
		
		public PlaylistWpl(string filename, bool create)
		{
			_filename = filename;
			
			if (create)
			{
				Load(GetXmlStream(C_WPL_XML));
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
				XmlNodeList list = SelectNodes("smil/body/seq/media");
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
		
		public void SavePlaylist()
		{
			this.Save(_filename);
		}
	}
}

using System.IO;
using System.Reflection;
using System.Xml;

namespace PodcastUtilities.Common
{
	public class XmlFileBase : XmlDocument
	{
		protected XmlFileBase(string filename, bool create, string emptyPlaylistResource)
		{
			Filename = filename;
			
			if (create)
			{
				Load(GetXmlStream(emptyPlaylistResource));
			}
			else
				Load(Filename);
		}

		/// <summary>
		/// override this to have an xml resource in a different assembly
		/// this is called from the constructor so you should make any overrides stateless - do not depend upoon member variables to be inialised
		/// </summary>
        protected virtual Stream GetXmlStream(string xmlfile)
		{
			Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(xmlfile);
			return s;
		}

		public string Filename { get; private set; }
		
		public void SaveFile()
		{
			Save(Filename);
		}

		protected string GetNodeText(string xpath)
		{
			XmlNode n = SelectSingleNode(xpath);
			if (n == null)
			{
				throw new System.Exception("GetNodeText : Node path '" + xpath + "' not found");
			}
			return n.InnerText;
		}

		protected void SetNodeText(string xpath, string val)
		{
			XmlNode n = SelectSingleNode(xpath);
			if (n == null)
			{
				throw new System.Exception("SetNodeText : Node path '" + xpath + "' not found");
			}
			n.InnerText = val;
		}
	}
}
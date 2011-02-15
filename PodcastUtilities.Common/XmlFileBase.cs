using System.IO;
using System.Reflection;
using System.Xml;

namespace PodcastUtilities.Common
{
	public class XmlFileBase : XmlDocument
	{
        protected XmlFileBase(string filename, bool create, string emptyPlaylistResource, Assembly resourceAssembly)
        {
            Filename = filename;

            if (create)
            {
                Load(GetXmlStream(resourceAssembly, emptyPlaylistResource));
            }
            else
                Load(Filename);
        }

        protected Stream GetXmlStream(Assembly assembly, string xmlfile)
		{
			Stream s = assembly.GetManifestResourceStream(xmlfile);
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
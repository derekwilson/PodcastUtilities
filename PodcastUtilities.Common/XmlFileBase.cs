using System.IO;
using System.Reflection;
using System.Xml;

namespace PodcastUtilities.Common
{
	/// <summary>
	/// base class for XML files for example playlists
	/// </summary>
    public class XmlFileBase : XmlDocument
	{
        /// <summary>
        /// create the XML file object
        /// </summary>
        /// <param name="filename">filename to read from and will be used to save to</param>
        /// <param name="create">true to load a template from the supplied resource path, false to load from disk using the filename</param>
        /// <param name="emptyPlaylistResource">resource pathname to load a nlank xml file from when creating</param>
        /// <param name="resourceAssembly">assembly to use to load the blank template from</param>
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

        /// <summary>
        /// load a stream from an assembly
        /// </summary>
        /// <param name="assembly">assembly to use</param>
        /// <param name="xmlFileResourcePath">resource path to the xml file</param>
        /// <returns></returns>
        protected static Stream GetXmlStream(Assembly assembly, string xmlFileResourcePath)
		{
			Stream s = assembly.GetManifestResourceStream(xmlFileResourcePath);
			return s;
		}

		/// <summary>
		/// the filename for the XML file
		/// </summary>
        public string Filename { get; private set; }
		
		/// <summary>
		/// persist the XML to disk
		/// </summary>
        public void SaveFile()
		{
			Save(Filename);
		}

		/// <summary>
		/// return the text from a specified node
		/// </summary>
		/// <param name="xpath">xpath to the node</param>
		/// <returns>the node text, an exception is thrown if the node does not ecist</returns>
        protected string GetNodeText(string xpath)
		{
			XmlNode n = SelectSingleNode(xpath);
			if (n == null)
			{
				throw new System.Exception("GetNodeText : Node path '" + xpath + "' not found");
			}
			return n.InnerText;
		}

		/// <summary>
		/// set the text for the specified node, an exception is thrown if the node does not exist
		/// </summary>
        /// <param name="xpath">xpath to the node</param>
        /// <param name="nodeValue">value to set</param>
        protected void SetNodeText(string xpath, string nodeValue)
		{
			XmlNode n = SelectSingleNode(xpath);
			if (n == null)
			{
				throw new System.Exception("SetNodeText : Node path '" + xpath + "' not found");
			}
			n.InnerText = nodeValue;
		}
	}
}
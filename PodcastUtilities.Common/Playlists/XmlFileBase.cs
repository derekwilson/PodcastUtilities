using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using PodcastUtilities.Common.Exceptions;

namespace PodcastUtilities.Common.Playlists
{
	/// <summary>
	/// base class for XML files for example playlists
	/// </summary>
    public class XmlFileBase
	{
	    private XmlDocument _xmlDocument;
	    private XmlEncoder _xmlEncoder;

	    /// <summary>
        /// create the XML file object
        /// </summary>
        /// <param name="fileName">filename to read from and will be used to save to</param>
        /// <param name="create">true to load a template from the supplied resource path, false to load from disk using the filename</param>
        /// <param name="emptyPlaylistResource">resource pathname to load a nlank xml file from when creating</param>
        /// <param name="resourceAssembly">assembly to use to load the blank template from</param>
        protected XmlFileBase(string fileName, bool create, string emptyPlaylistResource, Assembly resourceAssembly)
        {
            _xmlEncoder = new XmlEncoder();

            FileName = fileName;
            _xmlDocument = new XmlDocument();

            if (create)
            {
                _xmlDocument.Load(GetXmlStream(resourceAssembly, emptyPlaylistResource));
            }
            else
            {
                _xmlDocument.Load(FileName);
            }
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
        public string FileName { get; private set; }

		/// <summary>
		/// replace all the xml
		/// </summary>
		/// <param name="xml">the xml to inject</param>
        public void LoadXmlString(string xml)
		{
		    _xmlDocument.LoadXml(xml);
		}

        /// <summary>
        /// persist the XML to disk
        /// </summary>
        public void SaveFile()
        {
            _xmlDocument.Save(FileName);
        }

        /// <summary>
        /// persist the XML to disk
        /// </summary>
        public void SaveFile(string overrideFilename)
        {
            _xmlDocument.Save(overrideFilename);
        }

        /// <summary>
        /// number of tracks in the playlist
        /// </summary>
        public int GetNumberOfNodes(string xpath)
        {
            XmlNodeList list = _xmlDocument.SelectNodes(xpath);
            if (list != null)
                return list.Count;

            return 0;
        }

		/// <summary>
		/// return the text from a specified node
		/// </summary>
		/// <param name="xpath">xpath to the node</param>
		/// <returns>the node text, an exception is thrown if the node does not ecist</returns>
        protected string GetNodeText(string xpath)
		{
            XmlNode n = _xmlDocument.SelectSingleNode(xpath);
			if (n == null)
			{
				throw new XmlStructureException("GetNodeText : Node path '" + xpath + "' not found");
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
            XmlNode n = _xmlDocument.SelectSingleNode(xpath);
			if (n == null)
			{
                throw new XmlStructureException("SetNodeText : Node path '" + xpath + "' not found");
			}
			n.InnerText = nodeValue;
		}

        /// <summary>
        /// Encode/escape any special xml characters
        /// </summary>
        /// <param name="source">string which may contain special xml chracters</param>
        /// <returns>escaped output</returns>
        protected string XmlEncodeString(string source)
        {
            return _xmlEncoder.Encode(source);
        }

        /// <summary>
        /// find a specific node
        /// </summary>
        /// <param name="xpath">xpath to the node</param>
        /// <returns>node or null</returns>
        public IXPathNavigable FindNode(string xpath)
        {
            return _xmlDocument.SelectSingleNode(xpath);
        }
	}
}
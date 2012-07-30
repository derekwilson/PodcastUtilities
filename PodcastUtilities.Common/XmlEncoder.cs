using System;
using System.Security;

namespace PodcastUtilities.Common
{
    ///<summary>
    /// Class for encoding/escaping strings to be written to xml
    ///</summary>
    public class XmlEncoder : IXmlEncoder
    {
        /// <summary>
        /// Encode a string so that reserved xml characters ('&lt;', '&amp;', etc.) are escaped
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public string Encode(string source)
        {
            return SecurityElement.Escape(source);
        }
    }
}
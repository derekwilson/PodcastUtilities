namespace PodcastUtilities.Common
{
    ///<summary>
    /// Interface for encoding/escaping strings to be written to xml
    ///</summary>
    public interface IXmlEncoder
    {
        /// <summary>
        /// Encode a string so that reserved xml characters ('&lt;', '&amp;', etc.) are escaped
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        string Encode(string source);
    }
}
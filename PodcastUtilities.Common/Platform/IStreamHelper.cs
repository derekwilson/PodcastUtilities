using System.IO;

namespace PodcastUtilities.Common.Platform
{
    ///<summary>
    /// Stream helper methods
    ///</summary>
    public interface IStreamHelper
    {
        ///<summary>
        /// Open a readable stream on a file
        ///</summary>
        ///<param name="path"></param>
        ///<returns></returns>
        Stream OpenRead(string path);

        ///<summary>
        /// Open a writeable stream on a file
        ///</summary>
        ///<param name="path"></param>
        ///<param name="allowOverwrite"></param>
        ///<returns></returns>
        Stream OpenWrite(string path, bool allowOverwrite);

        ///<summary>
        /// Copy from one stream to another
        ///</summary>
        ///<param name="source">The source stream, must be readable</param>
        ///<param name="destination">The destination stream, must be writeable</param>
        void Copy(Stream source, Stream destination);
    }
}
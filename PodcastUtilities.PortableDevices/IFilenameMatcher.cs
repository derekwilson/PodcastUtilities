namespace PodcastUtilities.PortableDevices
{
    ///<summary>
    /// Interface for filename matching, primarily for file/directory-like MTP classes that can't just 
    /// delegate to appropriate built-in .Net versions.
    ///</summary>
    public interface IFilenameMatcher
    {
        ///<summary>
        /// Tests a filename to see if it matches the specified pattern.
        /// This is similar, but not identical, to the semantics of DirectoryInfo.GetFiles - specifically,
        /// it does not do the wacky stuff with file extensions and 8.3/long filenames: see
        /// http://msdn.microsoft.com/en-us/library/8he88b63.aspx
        ///</summary>
        ///<param name="filename">The filename to test</param>
        ///<param name="pattern">The pattern to test against</param>
        ///<returns></returns>
        bool IsMatch(string filename, string pattern);
    }
}
using System.Text.RegularExpressions;

namespace PodcastUtilities.PortableDevices
{
    ///<summary>
    /// Filename matching, primarily for file/directory-like MTP classes that can't just 
    /// delegate to appropriate built-in .Net versions.
    ///</summary>
    public class FilenameMatcher : IFilenameMatcher
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
        public bool IsMatch(string filename, string pattern)
        {
            // Approach was suggested by this Stackoverflow answer:
            // http://stackoverflow.com/questions/188892/glob-pattern-matching-in-net#4146349

            var patternRegex = 
                "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";

            return Regex.IsMatch(filename, patternRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }
    }
}
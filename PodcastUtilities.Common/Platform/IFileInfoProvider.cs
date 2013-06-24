namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// used to provide the physical file system to the main code and a mock file system to the units tests
    /// </summary>
    public interface IFileInfoProvider
    {
        /// <summary>
        /// create an abstract file info object
        /// </summary>
        /// <param name="path">full path to the file</param>
        /// <returns>the file info</returns>
        IFileInfo GetFileInfo(string path);
    }
}
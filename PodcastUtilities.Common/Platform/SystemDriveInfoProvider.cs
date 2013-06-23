namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// used to provide the physical file system to the main code and a mock file system to the units tests
    /// </summary>
    public class SystemDriveInfoProvider : IDriveInfoProvider
	{
        /// <summary>
        /// create an abstract drive info object
        /// </summary>
        /// <param name="path">name of the drive</param>
        /// <returns>an abstrcat object</returns>
        public IDriveInfo GetDriveInfoForPath(string path)
		{
			return new SystemDriveInfo(path);
		}
	}
}
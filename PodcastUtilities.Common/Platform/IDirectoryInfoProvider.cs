namespace PodcastUtilities.Common.Platform
{
	/// <summary>
	/// used to provide the physical file system to the main code and a mock file system to the units tests
	/// </summary>
    public interface IDirectoryInfoProvider
	{
		/// <summary>
		/// create an abstract directory object
		/// </summary>
		/// <param name="path">full path to the directory</param>
		/// <returns>an abstrcat object</returns>
        IDirectoryInfo GetDirectoryInfo(string path);
	}
}
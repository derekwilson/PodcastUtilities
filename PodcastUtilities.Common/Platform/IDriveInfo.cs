namespace PodcastUtilities.Common.Platform
{
	/// <summary>
    /// methods to query file system drives in the physical file system and abstract away the file system from the main body of code
	/// </summary>
    public interface IDriveInfo
	{
		/// <summary>
		/// the free space in bytes
		/// </summary>
        long AvailableFreeSpace { get; }

		/// <summary>
		/// the name of the drive
		/// </summary>
        string Name { get; }
	}
}

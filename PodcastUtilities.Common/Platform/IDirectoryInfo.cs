namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// methods to interact with directories in the physical file system and abstract away the file system from the main body of code
    /// </summary>
    public interface IDirectoryInfo
	{
		/// <summary>
		/// gets the abstract root of the filing system
		/// </summary>
        IDirectoryInfo Root { get; }

		/// <summary>
		/// the full pathname of the directory
		/// </summary>
        string FullName { get; }

		/// <summary>
		/// true if it exists
		/// </summary>
        bool Exists { get; }

		/// <summary>
		/// gets an abstract collection of files that are contained by by the directory
		/// </summary>
		/// <param name="pattern">a search patter for example *.mp3</param>
		/// <returns>a collection of abstracted files</returns>
        IFileInfo[] GetFiles(string pattern);

        /// <summary>
        /// gets an abstract collection of directories that are contained by the directory
        /// </summary>
        /// <param name="pattern">a search patter for example *.*</param>
        /// <returns>a collection of abstracted files</returns>
        IDirectoryInfo[] GetDirectories(string pattern);

		/// <summary>
		/// create the directory in the file system
		/// </summary>
        void Create();
	}
}
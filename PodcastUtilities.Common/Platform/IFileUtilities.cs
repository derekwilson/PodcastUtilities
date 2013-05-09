namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// methods to manipulate files in the physical file system and abstract away the file system from the main body of code
    /// </summary>
    public interface IFileUtilities
	{
        /// <summary>
        /// check if a file exists
        /// </summary>
        /// <param name="path">pathname to check</param>
        /// <returns>true if the file exists</returns>
        bool FileExists(string path);

        /// <summary>
        /// rename / move a file
        /// </summary>
        /// <param name="sourceFileName">source pathname</param>
        /// <param name="destinationFileName">destination pathname</param>
        void FileRename(string sourceFileName, string destinationFileName);

        /// <summary>
        /// rename / move a file
        /// </summary>
        /// <param name="sourceFileName">source pathname</param>
        /// <param name="destinationFileName">destination pathname</param>
        /// <param name="allowOverwrite">set to true to overwrite an existing destination file</param>
        void FileRename(string sourceFileName, string destinationFileName, bool allowOverwrite);

		/// <summary>
		/// copy a file - will not overwrite an existing file
		/// the containing folder will be created if it does not exist
		/// </summary>
		/// <param name="sourceFileName">source pathname</param>
		/// <param name="destinationFileName">destination pathname</param>
        void FileCopy(string sourceFileName, string destinationFileName);

        /// <summary>
        /// copy a file - the containing folder will be created if it does not exist
        /// </summary>
        /// <param name="sourceFileName">source pathname</param>
        /// <param name="destinationFileName">destination pathname</param>
        /// <param name="allowOverwrite">set to true to overwrite an existing file</param>
        void FileCopy(string sourceFileName, string destinationFileName, bool allowOverwrite);

		/// <summary>
		/// delete a file
		/// </summary>
		/// <param name="path">pathname of the file to delete</param>
        void FileDelete(string path);
	}
}

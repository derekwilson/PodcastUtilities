using System.IO;

namespace PodcastUtilities.Common.Platform
{
	/// <summary>
	/// utility methods to manipulate files in the physical file system
	/// this class abstracts away the file system from the main body of code
	/// </summary>
	internal class FileUtilities : IFileUtilities
	{
		/// <summary>
		/// check if a file exists
		/// </summary>
		/// <param name="path">pathname to check</param>
		/// <returns>true if the file exists</returns>
        public bool FileExists(string path)
		{
			return File.Exists(path);
		}

        /// <summary>
        /// rename / move a file
        /// </summary>
        /// <param name="sourceFileName">source pathname</param>
        /// <param name="destinationFileName">destination pathname</param>
        public void FileRename(string sourceFileName, string destinationFileName)
        {
            FileRename(sourceFileName,destinationFileName,false);
        }

        /// <summary>
        /// rename / move a file
        /// </summary>
        /// <param name="sourceFileName">source pathname</param>
        /// <param name="destinationFileName">destination pathname</param>
        /// <param name="allowOverwrite">set to true to overwrite an existing destination file</param>
        public void FileRename(string sourceFileName, string destinationFileName, bool allowOverwrite)
        {
            if (allowOverwrite)
            {
                if (File.Exists(destinationFileName))
                {
                    File.Delete(destinationFileName);
                }
            }
            File.Move(sourceFileName, destinationFileName);
        }

        /// <summary>
	    /// copy a file - will not overwrite an existing file
	    /// the containing folder will be created if it does not exist
	    /// </summary>
	    /// <param name="sourceFileName">source pathname</param>
	    /// <param name="destinationFileName">destination pathname</param>
	    public void FileCopy(string sourceFileName, string destinationFileName)
		{
			FileCopy(sourceFileName, destinationFileName, false);
		}

	    /// <summary>
	    /// copy a file - the containing folder will be created if it does not exist
	    /// </summary>
	    /// <param name="sourceFileName">source pathname</param>
	    /// <param name="destinationFileName">destination pathname</param>
	    /// <param name="allowOverwrite">set to true to overwrite an existing file</param>
	    public void FileCopy(string sourceFileName, string destinationFileName, bool allowOverwrite)
		{
			// Make sure directory exists
			var destinationDirectory = Path.GetDirectoryName(destinationFileName);
			if (destinationDirectory != null)
			{
				Directory.CreateDirectory(destinationDirectory);
			}

			File.Copy(sourceFileName, destinationFileName, allowOverwrite);
		}

	    /// <summary>
	    /// delete a file
	    /// </summary>
	    /// <param name="path">pathname of the file to delete</param>
	    public void FileDelete(string path)
		{
			File.Delete(path);
		}
	}
}
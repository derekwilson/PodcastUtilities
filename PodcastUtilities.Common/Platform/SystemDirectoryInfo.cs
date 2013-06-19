using System.IO;
using System.Linq;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// methods to interact with directories in the physical file system and abstract away the file system from the main body of code
    /// </summary>
    internal class SystemDirectoryInfo : IDirectoryInfo
	{
		private readonly DirectoryInfo _directoryInfo;

		/// <summary>
		/// object constructor from a pathname as a string
		/// </summary>
		/// <param name="path">pathname</param>
        public SystemDirectoryInfo(string path)
			: this(new DirectoryInfo(path))
		{
		}

        /// <summary>
        /// object constructor from another abstracted object
        /// </summary>
        /// <param name="directoryInfo">object to be constructed from</param>
        public SystemDirectoryInfo(DirectoryInfo directoryInfo)
		{
			_directoryInfo = directoryInfo;
		}

        /// <summary>
        /// true if it exists
        /// </summary>
        public bool Exists
		{
			get { return _directoryInfo.Exists; }
		}

        /// <summary>
        /// the full pathname of the directory
        /// </summary>
        public string FullName
		{
			get { return _directoryInfo.FullName; }
		}

        /// <summary>
        /// gets an abstract collection of files that are contained by the directory
        /// </summary>
        /// <param name="pattern">a search patter for example *.mp3</param>
        /// <returns>a collection of abstracted files</returns>
        public IFileInfo[] GetFiles(string pattern)
        {
            var realFiles = _directoryInfo.GetFiles(pattern);

            return realFiles.Select(f => new SystemFileInfo(f)).ToArray();
        }

        /// <summary>
        /// gets an abstract collection of directories that are contained by the directory
        /// </summary>
        /// <param name="pattern">a search patter for example *.*</param>
        /// <returns>a collection of abstracted files</returns>
        public IDirectoryInfo[] GetDirectories(string pattern)
        {
            var realDirs = _directoryInfo.GetDirectories(pattern);

            return realDirs.Select(d => new SystemDirectoryInfo(d)).ToArray();
        }

        /// <summary>
        /// create the directory in the file system
        /// </summary>
        public void Create()
        {
            _directoryInfo.Create();
        }

        /// <summary>
        /// delete the directory in the file system
        /// </summary>
        public void Delete()
        {
            _directoryInfo.Delete();
        }
    }
}
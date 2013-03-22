using System.Reflection;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// provides information on the application environment
    /// </summary>
    public class WindowsEnvironmentInformationProvider : IEnvironmentInformationProvider
    {
        /// <summary>
        /// gett the directory that the currently executing application was loaded from
        /// </summary>
        /// <returns></returns>
        public IDirectoryInfo GetCurrentApplicationDirectory()
        {
            return new SystemDirectoryInfo(System.IO.Directory.GetParent(Assembly.GetEntryAssembly().Location));
        }
    }
}
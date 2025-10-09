using PodcastUtilities.AndroidLogic.Logging;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IAndroidApplication
    {
        string DisplayVersion { get; }
        string DisplayPackage { get; }
        public void SetLoggingNone();
        public void SetLoggingVerbose();

    }
}
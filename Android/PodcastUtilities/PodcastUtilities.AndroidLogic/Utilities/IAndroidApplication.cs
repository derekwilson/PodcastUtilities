namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IAndroidApplication
    {
        string DisplayVersion { get; }
        string DisplayPackage { get; }
    }
}
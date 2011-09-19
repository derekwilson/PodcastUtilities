namespace PodcastUtilities.Common.Configuration
{
    ///<summary>
    /// Factory interface for creating podcast objects
    ///</summary>
    public interface IPodcastFactory
    {
        ///<summary>
        /// Create a new podcast
        ///</summary>
        ///<param name="controlFile"></param>
        ///<returns></returns>
        IPodcastInfo CreatePodcast(IControlFileGlobalDefaults controlFile);
    }
}
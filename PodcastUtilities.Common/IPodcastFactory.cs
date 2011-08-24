namespace PodcastUtilities.Common
{
    ///<summary>
    /// Factory interface for creating podcast objects
    ///</summary>
    public interface IPodcastFactory
    {
        ///<summary>
        /// Create a new podcast
        ///</summary>
        ///<returns></returns>
        PodcastInfo CreatePodcast();
    }
}
namespace PodcastUtilities.Common.Configuration
{
    ///<summary>
    /// Factory for creating podcast objects
    ///</summary>
    public class PodcastFactory : IPodcastFactory
    {
        ///<summary>
        /// Create a new podcast
        ///</summary>
        ///<param name="controlFile"></param>
        ///<returns></returns>
        public IPodcastInfo CreatePodcast(IControlFileGlobalDefaults controlFile)
        {
            var podcast = new PodcastInfo (controlFile)
                       {
                           Feed = new FeedInfo (controlFile)
                       };

            return podcast;
        }
    }
}
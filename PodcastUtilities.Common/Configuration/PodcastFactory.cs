namespace PodcastUtilities.Common.Configuration
{
    ///<summary>
    /// Factory for creating podcast objects
    ///</summary>
    public class PodcastFactory : IPodcastFactory
    {
        private readonly IPodcastDefaultsProvider _podcastDefaultsProvider;

        ///<summary>
        /// Factory ctor
        ///</summary>
        ///<param name="podcastDefaultsProvider"></param>
        public PodcastFactory(IPodcastDefaultsProvider podcastDefaultsProvider)
        {
            _podcastDefaultsProvider = podcastDefaultsProvider;
        }

        ///<summary>
        /// Create a new podcast
        ///</summary>
        ///<param name="controlFile"></param>
        ///<returns></returns>
        public IPodcastInfo CreatePodcast(IControlFileGlobalDefaults controlFile)
        {
            var podcast = new PodcastInfo (controlFile)
                       {
                           Pattern = _podcastDefaultsProvider.Pattern,
                           Feed = new FeedInfo (controlFile)
                       };

            return podcast;
        }
    }
}
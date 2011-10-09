namespace PodcastUtilities.Common.Configuration
{
    ///<summary>
    /// Simple, hard-coded podcast defaults
    ///</summary>
    public class HardcodedPodcastDefaultsProvider : IPodcastDefaultsProvider
    {
        ///<summary>
        /// Default file pattern
        ///</summary>
        public string Pattern
        {
            get { return "*.mp3"; }
        }
    }
}
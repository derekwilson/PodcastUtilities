namespace PodcastUtilities.Common.Configuration
{
    ///<summary>
    /// Supplies default values when creating new podcasts
    ///</summary>
    public interface IPodcastDefaultsProvider
    {
        ///<summary>
        /// Default file pattern
        ///</summary>
        string Pattern { get; }
    }
}
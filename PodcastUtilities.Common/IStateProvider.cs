namespace PodcastUtilities.Common
{
    /// <summary>
    /// provides access to the state
    /// </summary>
    public interface IStateProvider
    {
        /// <summary>
        /// get the state identified by the filename
        /// </summary>
        IState GetState(string podcastRoot);
    }
}
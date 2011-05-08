namespace PodcastUtilities.Common
{
	/// <summary>
	/// supports the factory pattern for playlists
	/// </summary>
    public interface IPlaylistFactory
	{
        /// <summary>
        /// create the correct playlist format
        /// </summary>
        /// <param name="playlistFormat">the playlist format required</param>
        /// <param name="filename">filename to use for the playlist</param>
        /// <returns></returns>
        IPlaylist CreatePlaylist(PlaylistFormat playlistFormat, string filename);
	}
}

namespace PodcastUtilities.Common
{
	public interface IPlaylistFactory
	{
		IPlaylist CreatePlaylist(PlaylistFormat playlistFormat, string filename);
	}
}

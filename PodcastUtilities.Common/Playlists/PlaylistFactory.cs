using System;

namespace PodcastUtilities.Common.Playlists
{
	/// <summary>
	/// factory to generate the correct playlist object
	/// </summary>
    public class PlaylistFactory : IPlaylistFactory
	{
		#region Implementation of IPlaylistFactory

		/// <summary>
		/// create the correct playlist format
		/// </summary>
		/// <param name="playlistFormat">the playlist format required</param>
		/// <param name="fileName">filename to use for the playlist</param>
		/// <returns></returns>
        public IPlaylist CreatePlaylist(PlaylistFormat playlistFormat, string fileName)
		{
			switch (playlistFormat)
			{
				case PlaylistFormat.ASX:
					return new PlaylistAsx(fileName, true);
				case PlaylistFormat.WPL:
					return new PlaylistWpl(fileName, true);
			}
            throw new ArgumentOutOfRangeException("playlistFormat");
		}

		#endregion
	}
}
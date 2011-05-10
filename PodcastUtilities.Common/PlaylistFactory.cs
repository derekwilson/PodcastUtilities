using System;

namespace PodcastUtilities.Common
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
		/// <param name="filename">filename to use for the playlist</param>
		/// <returns></returns>
        public IPlaylist CreatePlaylist(PlaylistFormat playlistFormat, string filename)
		{
			switch (playlistFormat)
			{
				case PlaylistFormat.ASX:
					return new PlaylistAsx(filename, true);
				case PlaylistFormat.WPL:
					return new PlaylistWpl(filename, true);
			}
            throw new ArgumentOutOfRangeException("playlistFormat");
		}

		#endregion
	}
}
using System;

namespace PodcastUtilities.Common
{
	public class PlaylistFactory : IPlaylistFactory
	{
		#region Implementation of IPlaylistFactory

		public IPlaylist CreatePlaylist(PlaylistFormat playlistFormat, string filename)
		{
			switch (playlistFormat)
			{
				case PlaylistFormat.ASX:
					return new PlaylistAsx(filename, true);
				case PlaylistFormat.WPL:
					return new PlaylistWpl(filename, true);
			}
			throw new IndexOutOfRangeException("Unknown playlist format");
		}

		#endregion
	}
}
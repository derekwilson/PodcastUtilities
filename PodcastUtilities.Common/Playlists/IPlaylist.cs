namespace PodcastUtilities.Common.Playlists
{
    /// <summary>
    /// supports the functionality needed to manage a playlist
    /// </summary>
    public interface IPlaylist
    {
        /// <summary>
        /// Add a track to the playlist
        /// </summary>
        /// <param name="filePath">pathname to add, can be relative or absolute</param>
        /// <returns>true if the file was added false if the track was already present</returns>
        bool AddTrack(string filePath);

        /// <summary>
        /// filename to use when saving the playlist file
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// number of tracks in the playlist
        /// </summary>
        int NumberOfTracks { get; }

        /// <summary>
        /// persist the playlist to disk
        /// </summary>
        void SaveFile();

        /// <summary>
        /// the title of the playlist
        /// </summary>
        string Title { get; set; }
    }
}

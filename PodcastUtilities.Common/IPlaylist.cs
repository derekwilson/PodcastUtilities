using System;

namespace PodcastUtilities.Common
{
    public interface IPlaylist
    {
        bool AddTrack(string filepath);
        string Filename { get; }
        int NumberOfTracks { get; }
        void SaveFile();
        string Title { get; set; }
    }
}

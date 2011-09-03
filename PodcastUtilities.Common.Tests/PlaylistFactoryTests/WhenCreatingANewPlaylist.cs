using System;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.PlaylistFactoryTests
{
    public abstract class WhenCreatingANewPlaylist
        : WhenTestingBehaviour
    {
        protected PlaylistFormat PlayListFormat { get; set; }
        protected IPlaylist Playlist { get; set; }
        protected IPlaylistFactory PlaylistFactory { get; set; }
        protected string PlayllistFilename { get; set; }
        protected Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();
            PlaylistFactory = new PlaylistFactory();
            PlayllistFilename = "myplaylist.ext";
        }

        protected override void When()
        {
            ThrownException = null;
            try
            {
                Playlist = PlaylistFactory.CreatePlaylist(PlayListFormat, PlayllistFilename);
            }
            catch (Exception e)
            {
                ThrownException = e;
            }
        }
    }
}

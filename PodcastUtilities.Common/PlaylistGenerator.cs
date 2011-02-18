using System;
using System.Linq;
using System.IO;
using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common
{
    public class PlaylistGenerator
    {
    	public PlaylistGenerator(
			IFileFinder fileFinder,
			IFileUtilities fileUtilities,
			IPlaylistFactory playlistFactory)
    	{
    		FileFinder = fileFinder;
    		FileUtilities = fileUtilities;
    		PlaylistFactory = playlistFactory;
    	}

    	public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

    	private IFileFinder FileFinder { get; set; }
    	private IFileUtilities FileUtilities { get; set; }
    	private IPlaylistFactory PlaylistFactory { get; set; }

    	private void OnStatusUpdate(string message)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Status, message));
        }

        private void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            if (StatusUpdate != null)
                StatusUpdate(this, e);
        }

        public void GeneratePlaylist(IControlFile control, bool copyToDestination)
        {
			var allDestFiles = control.Podcasts.SelectMany(
        		podcast => FileFinder.GetFiles(Path.Combine(control.DestinationRoot, podcast.Folder), podcast.Pattern));

			IPlaylist p = PlaylistFactory.CreatePlaylist(control.PlaylistFormat, control.PlaylistFilename);

            foreach (IFileInfo thisFile in allDestFiles)
            {
                string thisRelativeFile = thisFile.FullName;
                string absRoot = Path.GetFullPath(control.DestinationRoot);
                if (thisRelativeFile.StartsWith(absRoot))
                {
                    thisRelativeFile = thisRelativeFile.Substring(absRoot.Length);
                }
                p.AddTrack("." + thisRelativeFile);
            }

            p.SaveFile();

            if (copyToDestination)
            {
                string destPlaylist = Path.Combine(control.DestinationRoot, control.PlaylistFilename);
                OnStatusUpdate(string.Format("Copying Playlist with {0} items to {1}", p.NumberOfTracks, destPlaylist));
				FileUtilities.FileCopy(control.PlaylistFilename, destPlaylist, true);
            }
            else
            {
                OnStatusUpdate(string.Format("Playlist with {0} items generated: {1}", p.NumberOfTracks, control.PlaylistFilename));
            }
        }
    }
}

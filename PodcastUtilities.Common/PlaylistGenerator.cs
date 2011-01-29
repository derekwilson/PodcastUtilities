using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common
{
    public class PlaylistGenerator
    {
    	public PlaylistGenerator(IFileFinder fileFinder)
    	{
    		FileFinder = fileFinder;
    	}

    	public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

    	private IFileFinder FileFinder { get; set; }

		private void OnStatusUpdate(string message)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Status, message));
        }

        private void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            if (StatusUpdate != null)
                StatusUpdate(this, e);
        }

        static private IPlaylist PlaylistFactory(IControlFile control)
        {
            switch (control.PlaylistFormat)
            {
                case PlaylistFormat.ASX:
                    return new PlaylistAsx(control.PlaylistFilename, true);
                case PlaylistFormat.WPL:
                    return new PlaylistWpl(control.PlaylistFilename, true);
            }
            throw new IndexOutOfRangeException("Unknown playlist format");
        }

        public void GeneratePlaylist(IControlFile control, bool copyToDestination)
        {
			var allDestFiles = control.Podcasts.SelectMany(
        		podcast => FileFinder.GetFiles(Path.Combine(control.DestinationRoot, podcast.Folder), podcast.Pattern));

            IPlaylist p = PlaylistFactory(control);
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

            p.SavePlaylist();

            if (copyToDestination)
            {
                string destPlaylist = Path.Combine(control.DestinationRoot, control.PlaylistFilename);
                OnStatusUpdate(string.Format("Copying Playlist with {0} items to {1}", p.NumberOfTracks, destPlaylist));
                File.Copy(control.PlaylistFilename, destPlaylist, true);
            }
            else
            {
                OnStatusUpdate(string.Format("Playlist with {0} items generated: {1}", p.NumberOfTracks, control.PlaylistFilename));
            }
        }

        void finder_StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            // pass it on
            OnStatusUpdate(e);
        }
    }
}

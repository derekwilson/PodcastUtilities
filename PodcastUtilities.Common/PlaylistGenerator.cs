using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PodcastUtilities.Common
{
    public class PlaylistGenerator
    {
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        private void OnStatusUpdate(string message)
        {
            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Status, message));
        }

        private void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            if (StatusUpdate != null)
                StatusUpdate(this, e);
        }

        static private IPlaylist PlaylistFactory(ControlFile control)
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

        public void GeneratePlaylist(ControlFile control, bool copyToDestination)
        {
            FileFinder finder = new FileFinder();
            finder.StatusUpdate += new EventHandler<StatusUpdateEventArgs>(finder_StatusUpdate);
            List<FileInfo> allDestFiles = finder.GetAllFilesInTarget(control);

            IPlaylist p = PlaylistFactory(control);
            foreach (FileInfo thisFile in allDestFiles)
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

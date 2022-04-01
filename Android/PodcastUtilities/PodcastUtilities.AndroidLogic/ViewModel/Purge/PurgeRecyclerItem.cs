namespace PodcastUtilities.AndroidLogic.ViewModel.Purge
{
    public class PurgeRecyclerItem
    {
        // we need to store different types in one list - either a file or a directory
        public dynamic FileOrDirectoryItem { get; set; }
        public bool Selected { get; set; }
    }
}
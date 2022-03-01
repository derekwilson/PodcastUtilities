using Android.OS;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IFileSystemHelper
    {
        long GetAvailableFileSystemSizeInBytes(string path);
        long GetTotalFileSystemSizeInBytes(string path);
    }

    public class FileSystemHelper : IFileSystemHelper
    {
        public long GetAvailableFileSystemSizeInBytes(string path)
        {
            StatFs stat = new StatFs(path);
            long blockSize = stat.BlockSizeLong;
            long availableBlocks = stat.AvailableBlocksLong;
            return availableBlocks * blockSize;
        }

        public long GetTotalFileSystemSizeInBytes(string path)
        {
            StatFs stat = new StatFs(path);
            long blockSize = stat.BlockSizeLong;
            long blocks = stat.BlockCountLong;
            return blocks * blockSize;
        }
    }
}
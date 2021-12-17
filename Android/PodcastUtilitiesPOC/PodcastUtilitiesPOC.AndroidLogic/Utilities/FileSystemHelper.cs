using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilitiesPOC.AndroidLogic.Utilities
{
    public interface IFileSystemHelper
    {
        long GetAvailableMemorySizeInBytes(string rootPath);
        long GetAvailableMemorySizeInMb(string rootPath);
    }

    public class FileSystemHelper : IFileSystemHelper
    {
        public long GetAvailableMemorySizeInBytes(string rootPath)
        {
            StatFs stat = new StatFs(rootPath);
            long blockSize = stat.BlockSizeLong;
            long availableBlocks = stat.AvailableBlocksLong;
            return availableBlocks * blockSize;
        }

        public long GetAvailableMemorySizeInMb(string rootPath)
        {
            long freeBytes = GetAvailableMemorySizeInBytes(rootPath);
            long freeKb = 0;
            long freeMb = 0;
            if (freeBytes > 0)
                freeKb = (freeBytes / 1024);
            if (freeKb > 0)
                freeMb = (freeKb / 1024);

            return freeMb;
        }
    }
}
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
        long GetAvailableFileSystemSizeInBytes(string rootPath);
        long GetTotalFileSystemSizeInBytes(string rootPath);
    }

    public class FileSystemHelper : IFileSystemHelper
    {
        public long GetAvailableFileSystemSizeInBytes(string rootPath)
        {
            StatFs stat = new StatFs(rootPath);
            long blockSize = stat.BlockSizeLong;
            long availableBlocks = stat.AvailableBlocksLong;
            return availableBlocks * blockSize;
        }

        public long GetTotalFileSystemSizeInBytes(string rootPath)
        {
            StatFs stat = new StatFs(rootPath);
            long blockSize = stat.BlockSizeLong;
            long blocks = stat.BlockCountLong;
            return blocks * blockSize;
        }
    }
}
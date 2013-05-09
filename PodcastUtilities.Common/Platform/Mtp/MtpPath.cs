using System;
using System.IO;

namespace PodcastUtilities.Common.Platform.Mtp
{
    internal static class MtpPath
    {
        private const string MtpPrefix = @"MTP:\";

        public static bool IsMtpPath(string path)
        {
            return HasMtpPrefix(path);
        }

        public static string StripMtpPrefix(string path)
        {
            if (HasMtpPrefix(path))
            {
                return path.Substring(MtpPrefix.Length);
            }

            return path;
        }

        public static string MakeFullPath(string path)
        {
            if (HasMtpPrefix(path))
            {
                return path;
            }

            return MtpPrefix + path;
        }

        public static string GetDeviceName(string path)
        {
            var separator = path.IndexOf(Path.DirectorySeparatorChar);

            return ((separator < 0) ? path : path.Substring(0, separator));
        }

        public static string GetPathWithoutDeviceName(string path)
        {
            var separator = path.IndexOf(Path.DirectorySeparatorChar);

            return ((separator < 0) ? "" : path.Substring(separator + 1, path.Length - (separator + 1)));
        }

        public static MtpPathInfo GetPathInfo(string path)
        {
            var pathInfo = new MtpPathInfo
                               {
                                   IsMtpPath = IsMtpPath(path)
                               };
            if (pathInfo.IsMtpPath)
            {
                var strippedPath = StripMtpPrefix(path);

                pathInfo.DeviceName = GetDeviceName(strippedPath);
                pathInfo.RelativePathOnDevice = GetPathWithoutDeviceName(strippedPath);
            }

            return pathInfo;
        }

        public static string Combine(string path1, string path2)
        {
            return String.Format("{0}{1}{2}", path1, Path.DirectorySeparatorChar, path2);
        }

        private static bool HasMtpPrefix(string path)
        {
            return path.ToUpperInvariant().StartsWith(MtpPrefix);
        }
    }
}
#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
using System;
using System.IO;

namespace PodcastUtilities.Common.Platform.Mtp
{
    public static class MtpPath
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
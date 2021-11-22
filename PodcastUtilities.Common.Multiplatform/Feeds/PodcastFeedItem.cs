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

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// An item in a podcast feed
    /// </summary>
    public class PodcastFeedItem : IPodcastFeedItem
    {
        // not invalid in file system filenames - however they cannot be put into XML so make playlists difficult
        private static char[] xml_invalid_chars = { '\'', '"' };
        // these are chars that are invalid in the file system but not in Path.GetInvalidFileNameChars()
        // for example the ? and : on Android scoped file storage (MediaStore)
        // to be fair to Mono its not possible to get Path.GetInvalidFileNameChars() correct as the rules change depending on the folder (thanks google)
        private static char[] additional_invalid_chars = { '?', ':' };

        /// <summary>
        /// title of the item
        /// </summary>
        public string EpisodeTitle  { get; set; }

        /// <summary>
        /// address of the attachment to download
        /// </summary>
        public Uri Address { get; set; }

        /// <summary>
        /// the date the episode was published
        /// </summary>
        public DateTime Published { get; set; }

        /// <summary>
        /// filename to use when saving the podcast file
        /// </summary>
        public string FileName
        {
            get
            {
                string filename = Address.Segments[Address.Segments.Length - 1];

                filename = ProcessFilenameForInvalidChars(filename);

                return filename;
            }
        }

        /// <summary>
        /// get the episode title in a form that can be used as a filename
        /// </summary>
        public string TitleAsFileName
        {
            get
            {
                var sanitizedTitle = ProcessFilenameForInvalidChars(EpisodeTitle);

                return Path.ChangeExtension(sanitizedTitle, Path.GetExtension(FileName));
            }
        }

        private static string ProcessFilenameForInvalidChars(string filename)
        {
            if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                filename = RemoveInvalidChars(filename, Path.GetInvalidFileNameChars());
            }
            if (filename.IndexOfAny(xml_invalid_chars) != -1)
            {
                filename = RemoveInvalidChars(filename, xml_invalid_chars);
            }
            if (filename.IndexOfAny(additional_invalid_chars) != -1)
            {
                filename = RemoveInvalidChars(filename, additional_invalid_chars);
            }
            return filename;
        }

        private static string RemoveInvalidChars(string filename, char[] invalid)
        {
            foreach (char invalidFileNameChar in invalid)
            {
                filename = filename.Replace(invalidFileNameChar, '_');
            }
            return filename;
        }
    }
}
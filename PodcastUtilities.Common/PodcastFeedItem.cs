using System;
using System.IO;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// An item in a podcast feed
    /// </summary>
    public class PodcastFeedItem : IPodcastFeedItem
    {
        private static char[] xml_invalid_chars = {'\'','"'};

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
        /// <returns></returns>
        public string GetFileName()
        {
            string filename = Address.Segments[Address.Segments.Length - 1];

            filename = ProcessFilenameForInvalidChars(filename);

            return filename;
        }

        /// <summary>
        /// get the episode title in a form that can be used as a filename
        /// </summary>
        /// <returns></returns>
        public string GetTitleAsFileName()
        {
        	var sanitizedTitle = ProcessFilenameForInvalidChars(EpisodeTitle);

            return Path.ChangeExtension(sanitizedTitle, Path.GetExtension(GetFileName()));
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
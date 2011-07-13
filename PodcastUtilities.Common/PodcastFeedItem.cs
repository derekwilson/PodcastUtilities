using System;
using System.IO;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// An item in a podcast feed
    /// </summary>
    public class PodcastFeedItem : IPodcastFeedItem
    {
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
        public string GetFilename()
        {
            string filename = Address.Segments[Address.Segments.Length - 1];
            
            if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                filename = RemoveInvalidChars(filename);
            }
            return filename;
        }

        private static string RemoveInvalidChars(string filename)
        {
            foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(invalidFileNameChar, '_');
            }
            return filename;
        }
    }
}
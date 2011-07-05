using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// render data for display
    /// </summary>
    public class DisplayFormatter
    {
        /// <summary>
        /// render a file size into KB or MB or GB as appropriate
        /// </summary>
        /// <param name="numberOfBytes"></param>
        /// <returns></returns>
        static public string RenderFileSize(long numberOfBytes)
        {
            long kb = 0;
            double mb = 0;
            double gb = 0;

            if (numberOfBytes > 0)
            {
                kb = (numberOfBytes / 1024);
            }
            if (kb > 0)
            {
                mb = (kb / 1024);
            }
            if (mb > 0)
            {
                gb = (mb / 1024);
            }

            if (gb > 1)
            {
                return string.Format("{0:#,0.##} GB", gb);
            }
            if (mb > 1)
            {
                return string.Format("{0:#,0.##} MB", mb);
            }
            if (kb > 1)
            {
                return string.Format("{0:#,0.##} KB", kb);
            }
            return string.Format("{0:#,0.##} bytes", numberOfBytes);
        }
    }
}

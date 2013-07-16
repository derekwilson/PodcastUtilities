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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// render data for display
    /// </summary>
    public static class DisplayFormatter
    {
        // For FxCop rule: StaticHolderTypesShouldNotHaveConstructors

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
                return string.Format(CultureInfo.InvariantCulture, "{0:#,0.##} GB", gb);
            }
            if (mb > 1)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0:#,0.##} MB", mb);
            }
            if (kb > 1)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0:#,0.##} KB", kb);
            }
            return string.Format(CultureInfo.InvariantCulture, "{0:#,0.##} bytes", numberOfBytes);
        }
    }
}

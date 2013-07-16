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
using System.Linq;
using System.Text;
using System.Windows;

namespace PodcastUtilities.Presentation
{
    public class DataObjectUriExtractor : IDataObjectUriExtractor
    {
        #region Implementation of IDataObjectUriExtractor

        public bool ContainsUri(IDataObject dataObject)
        {
            return (GetUri(dataObject) != null);
        }

        public string GetUri(IDataObject dataObject)
        {
            var uriData = GetDataInUrlFormat(dataObject);

            return uriData ?? GetDataInTextFormat(dataObject);
        }

        #endregion

        private static string GetDataInUrlFormat(IDataObject dataObject)
        {
            var stream = (MemoryStream)dataObject.GetData("UniformResourceLocator");
            if (stream == null)
            {
                return null;
            }

            var terminatedStringBytes = stream.ToArray().TakeWhile(b => b != 0);

            return Encoding.ASCII.GetString(terminatedStringBytes.ToArray());
        }

        private static string GetDataInTextFormat(IDataObject dataObject)
        {
            var text = dataObject.GetData("Text") as string;

            return (IsValidUri(text) ? text : null);
        }

        private static bool IsValidUri(string address)
        {
            return ((address != null) && Uri.IsWellFormedUriString(address, UriKind.Absolute));
        }
    }
}
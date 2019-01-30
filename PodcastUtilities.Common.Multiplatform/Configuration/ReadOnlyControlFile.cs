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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;

namespace PodcastUtilities.Common.Configuration
{
	/// <summary>
	/// this object represents the xml control file
	/// </summary>
	public class ReadOnlyControlFile : BaseControlFile, IReadOnlyControlFile, IControlFileGlobalDefaults
	{
        /// <summary>
        /// create an empty control file
        /// </summary>
        public ReadOnlyControlFile() : base()
        {
        }

		/// <summary>
		/// create the object and read the control file from the specified filename
		/// </summary>
		/// <param name="fileName">pathname to the control file xml</param>
        public ReadOnlyControlFile(string fileName)
		{
            XmlReaderSettings readSettings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };

			using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
			{
				ReadXml(XmlReader.Create(fileStream, readSettings));
			};
        }

        /// <summary>
        /// only used for unit testing
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        public ReadOnlyControlFile(XmlDocument document) : base(document)
        {
        }
    }
}

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

using System.Net;
using PodcastUtilities.Common.Platform;

using System.Security.Authentication;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// factory class to generate a new web client
    /// </summary>
    public class WebClientFactory : IWebClientFactory
    {
        static WebClientFactory()
        {
            FixSecurityProtocol();
        }

        /// <summary>
        /// generate a new web client - do not forget to dispose it
        /// </summary>
        /// <returns>a web client</returns>
        public IWebClient CreateWebClient()
        {
            return new SystemNetWebClient();
        }

        private static void FixSecurityProtocol()
        {
			try
			{
				// Our version of .net only has protocol types defined for SSL3 and TLS1.0
				// However, some of the servers are no longer supporting 1.0 so we need to tell .Net to also use
				// TLS1.1 and 1.2 - these are the equivalent values from later .net versions
				const int ssl3 = (int)SecurityProtocolType.Ssl3;
				const int tls10 = (int)SecurityProtocolType.Tls;
				const int tls11 = 768;
				const int tls12 = 3072;
				ServicePointManager.SecurityProtocol = (SecurityProtocolType)(ssl3 + tls10 + tls11 + tls12);
			}
			catch
			{
				// we need to swallow this if the OS does not supprt the protocols

				// for example on Windows7 you will get this error
				// Unhandled Exception: System.TypeInitializationException: The type initializer for 'PodcastUtilities.Common.WebClientFactory' threw an exception.
				// ---> System.NotSupportedException: The requested security protocol is not supported.

				// unless you install the following 
				// Support for TLS System Default Versions included in the .NET Framework 3.5.1 on Windows 7 SP1 and Server 2008 R2 SP1
				// https://support.microsoft.com/en-us/help/3154518/support-for-tls-system-default-versions-included-in-the-net-framework
			}
		}
	}
}

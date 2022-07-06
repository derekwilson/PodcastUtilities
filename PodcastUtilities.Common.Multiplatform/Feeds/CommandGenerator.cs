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
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// generates full commands from the provided podcast and command template
    /// </summary>
    public class CommandGenerator : ICommandGenerator
    {
        private readonly IEnvironmentInformationProvider _environmentInformationProvider;

        /// <summary>
        /// create the generator
        /// </summary>
        public CommandGenerator(IEnvironmentInformationProvider environmentInformationProvider)
        {
            _environmentInformationProvider = environmentInformationProvider;
        }

        private string ReplaceTokensInString(string input, string rootFolder, string destinationPathname, IPodcastInfo podcast)
        {
            if (input == null)
                return null;

            string returnValue = input.Replace("{downloadfullpath}", destinationPathname);
            returnValue = returnValue.Replace("{downloadroot}", rootFolder);
            returnValue = returnValue.Replace("{downloadfolder}", podcast.Folder);
            returnValue = returnValue.Replace("{exefolder}", GetExeFolder());

            return returnValue;
        }

        private string GetExeFolder()
        {
            return _environmentInformationProvider.GetCurrentApplicationDirectory().FullName;
        }

        /// <summary>
        /// replace any token elements in a command line and return a full command ready to be executed
        /// </summary>
        /// <param name="tokenisedCommand">tokenised command</param>
        /// <param name="rootFolder">the root folder for all downloads</param>
        /// <param name="destinationPathname">the full pathname for the download</param>
        /// <param name="podcast">the podcast, used as a source of tokens</param>
        /// <returns>a full command line, NULL if there is no command to execute</returns>
        public IExternalCommand ReplaceTokensInCommand(ITokenisedCommand tokenisedCommand, string rootFolder, string destinationPathname, IPodcastInfo podcast)
        {
            if (tokenisedCommand == null)
            {
                return null;
            }

            var command = new ExternalCommand();

            command.Command = ReplaceTokensInString(tokenisedCommand.Command.Value, rootFolder, destinationPathname, podcast);
            command.Arguments = ReplaceTokensInString(tokenisedCommand.Arguments.Value, rootFolder, destinationPathname, podcast);
            command.WorkingDirectory = ReplaceTokensInString(tokenisedCommand.WorkingDirectory.Value, rootFolder, destinationPathname, podcast);

            return command;
        }
    }
}
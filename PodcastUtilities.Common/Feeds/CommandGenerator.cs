using System;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// generates full commands from the provided podcast and command template
    /// </summary>
    public class CommandGenerator : ICommandGenerator
    {
        private string ReplaceTokensInString(string input, string rootFolder, string destinationPathname, PodcastInfo podcast)
        {
            if (input == null)
                return null;

            string returnValue = input.Replace("{downloadfullpath}", destinationPathname);
            returnValue = returnValue.Replace("{downloadroot}", rootFolder);
            returnValue = returnValue.Replace("{downloadfolder}", podcast.Folder);

            return returnValue;
        }
        
        /// <summary>
        /// replace any token elements in a command line and return a full command ready to be executed
        /// </summary>
        /// <param name="tokenisedCommand">tokenised command</param>
        /// <param name="rootFolder">the root folder for all downloads</param>
        /// <param name="destinationPathname">the full pathname for the download</param>
        /// <param name="podcast">the podcast, used as a source of tokens</param>
        /// <returns>a full command line, NULL if there is no command to execute</returns>
        public IExternalCommand ReplaceTokensInCommand(ITokenisedCommand tokenisedCommand, string rootFolder, string destinationPathname, PodcastInfo podcast)
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
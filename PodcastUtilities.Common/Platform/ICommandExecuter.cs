using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// execute a command
    /// </summary>
    public interface ICommandExecuter
    {
        /// <summary>
        /// execute a command
        /// </summary>
        /// <param name="command">command</param>
        /// <param name="arguments">args</param>
        /// <param name="workingDirectory">cwd - optional</param>
        /// <returns>std output</returns>
        string ExecuteCommand(string command, string arguments, string workingDirectory);
    }
}

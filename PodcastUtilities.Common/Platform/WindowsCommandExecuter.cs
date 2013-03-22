namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// command executer implementation for windows
    /// </summary>
    public class WindowsCommandExecuter : ICommandExecuter
    {
        /// <summary>
        /// execute a command
        /// </summary>
        /// <param name="command">command</param>
        /// <param name="arguments">args</param>
        /// <param name="workingDirectory">cwd - optional</param>
        /// <returns>std output</returns>
        public string ExecuteCommand(string command, string arguments, string workingDirectory)
        {
            //Create process
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();

            //strCommand is path and file name of command to run
            pProcess.StartInfo.FileName = command;

            //strCommandParameters are parameters to pass to program
            pProcess.StartInfo.Arguments = arguments;

            pProcess.StartInfo.UseShellExecute = false;

            //Set output of program to be written to process output stream
            pProcess.StartInfo.RedirectStandardOutput = true;

            //Optional
            if (!string.IsNullOrEmpty(workingDirectory))
            {
                pProcess.StartInfo.WorkingDirectory = workingDirectory;
            }

            //Start the process
            pProcess.Start();

            //Get program output
            string strOutput = pProcess.StandardOutput.ReadToEnd();

            //Wait for process to finish
            pProcess.WaitForExit();

            return strOutput;
        }
    }
}
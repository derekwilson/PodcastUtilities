using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Integration.Tests.ControlFile
{
    class Runner : RunnerBase
    {
        public Runner(string controlFilename) : base(controlFilename)
        {
        }

        public override void RunAllTests()
        {
            DisplayMessage("ControlFile Tests:", DisplayLevel.Title);
            if (_controlFile == null)
            {
                DisplayMessage("No control file specified, tests skipped");
            }
        }
    }
}

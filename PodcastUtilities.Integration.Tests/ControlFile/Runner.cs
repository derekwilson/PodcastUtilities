using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Integration.Tests.ControlFile
{
    class Runner : RunnerBase
    {
        private const string _filename = "podcastutilities.integrationtest.contolfile.xml";

        public Runner(string testToRun)
            : base(testToRun)
        {
        }

        public override void RunAllTests()
        {
            DisplayMessage("ControlFile Tests:", DisplayLevel.Title);
            if (!ShouldRunTests("control"))
            {
                DisplayMessage(" tests skipped");
                return;
            }

            RunOneTest(CreateControlFile);
        }

        private void CreateControlFile()
        {
            DisplayMessage(string.Format("Creating a blank control file: {0}",_filename));
            ReadWriteControlFile controlFile = new ReadWriteControlFile();
            controlFile.SaveToFile(_filename);
        }
    }
}

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

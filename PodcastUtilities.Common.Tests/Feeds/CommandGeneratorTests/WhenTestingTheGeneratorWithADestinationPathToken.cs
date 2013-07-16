﻿#region License
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
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.Feeds.CommandGeneratorTests
{
    public class WhenTestingTheGeneratorWithADestinationPathToken
        : WhenTestingTheGenerator
    {
        protected override void SetupData()
        {
            base.SetupData();
            _command.Command.Value = "COMMAND {downloadfullpath}";
            _command.Arguments.Value = "ARGS {downloadfullpath}";
            _command.WorkingDirectory.Value = "CWD {downloadfullpath}";
        }

        /// <summary>
        /// Invoke the action being tested.
        /// </summary>
        protected override void When()
        {
            _generatedCommand = _generator.ReplaceTokensInCommand(_command, _rootFolder, _destinationPathname, _podcastInfo);
        }

        [Test]
        public void ItShouldReturnTheCommand()
        {
            Assert.That(_generatedCommand.Command, Is.EqualTo(@"COMMAND c:\root\TestFolder\destination.xyz"));
        }
        [Test]
        public void ItShouldReturnTheArguments()
        {
            Assert.That(_generatedCommand.Arguments, Is.EqualTo(@"ARGS c:\root\TestFolder\destination.xyz"));
        }
        [Test]
        public void ItShouldReturnTheWorkingDirectory()
        {
            Assert.That(_generatedCommand.WorkingDirectory, Is.EqualTo(@"CWD c:\root\TestFolder\destination.xyz"));
        }
    }
}
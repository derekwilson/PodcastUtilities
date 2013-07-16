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
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.PodcastInfoTests.Clone
{
    public class WhenCloningAPodcastInfoWithAPostDownloadCommand : WhenCloningAPodcastInfo
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _pocastInfo.AscendingSort.Value = true;
            _pocastInfo.Folder = "FOLDER";
            _pocastInfo.MaximumNumberOfFiles.Value = 123;
            _pocastInfo.Pattern.Value = "PATTERN";
            _pocastInfo.SortField.Value = PodcastFileSortField.FileName;

            _pocastInfo.CreatePostDownloadCommand();
            _pocastInfo.PostDownloadCommand.Command.Value = "CMD";
            _pocastInfo.PostDownloadCommand.Arguments.Value = "ARGS";
            _pocastInfo.PostDownloadCommand.WorkingDirectory.Value = "CWD";
        }

        protected override void When()
        {
            _clonedPodcast = _pocastInfo.Clone() as PodcastInfo;
        }

        [Test]
        public void ItShouldCloneThePodcastAscendingSort()
        {
            Assert.That(_clonedPodcast.AscendingSort.Value, Is.EqualTo(true));
        }

        [Test]
        public void ItShouldCloneThePodcastFolder()
        {
            Assert.That(_clonedPodcast.Folder, Is.EqualTo("FOLDER"));
        }

        [Test]
        public void ItShouldCloneThePodcastMaximumNumberOfFiles()
        {
            Assert.That(_clonedPodcast.MaximumNumberOfFiles.Value, Is.EqualTo(123));
        }

        [Test]
        public void ItShouldCloneThePodcastSortField()
        {
            Assert.That(_clonedPodcast.SortField.Value, Is.EqualTo(PodcastFileSortField.FileName));
        }

        [Test]
        public void ItShouldCloneThePostDownloadCommand()
        {
            Assert.That(_clonedPodcast.PostDownloadCommand.Command.Value, Is.EqualTo("CMD"));
        }

        [Test]
        public void ItShouldCloneThePostDownloadCommandArgs()
        {
            Assert.That(_clonedPodcast.PostDownloadCommand.Arguments.Value, Is.EqualTo("ARGS"));
        }

        [Test]
        public void ItShouldCloneThePostDownloadCommandWorkingDir()
        {
            Assert.That(_clonedPodcast.PostDownloadCommand.WorkingDirectory.Value, Is.EqualTo("CWD"));
        }

    }
}

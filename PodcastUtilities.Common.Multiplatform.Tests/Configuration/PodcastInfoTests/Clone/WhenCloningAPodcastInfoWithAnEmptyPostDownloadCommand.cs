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
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Multiplatform.Tests.Configuration.PodcastInfoTests.Clone
{
    public class WhenCloningAPodcastInfoWithAnEmptyPostDownloadCommand : WhenCloningAPodcastInfo
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _podcastInfo.AscendingSort.Value = true;
            _podcastInfo.Folder = "FOLDER";
            _podcastInfo.MaximumNumberOfFiles.Value = 123;
            _podcastInfo.Pattern.Value = "PATTERN";
            _podcastInfo.SortField.Value = PodcastFileSortField.FileName;

            _podcastInfo.CreatePostDownloadCommand();
        }

        protected override void When()
        {
            _clonedPodcast = _podcastInfo.Clone() as PodcastInfo;
        }

        [Test]
        public void ItShouldCloneThePostDownloadCommand()
        {
            Assert.That(_clonedPodcast.PostDownloadCommand, Is.Not.Null);
        }

        [Test]
        public void ItShouldCloneThePostDownloadCommandCommand()
        {
            Assert.That(_clonedPodcast.PostDownloadCommand.Command.IsSet, Is.EqualTo(false));
        }

        [Test]
        public void ItShouldCloneThePostDownloadCommandArguments()
        {
            Assert.That(_clonedPodcast.PostDownloadCommand.Arguments.IsSet, Is.EqualTo(false));
        }

        [Test]
        public void ItShouldCloneThePostDownloadCommandWorkingDirectory()
        {
            Assert.That(_clonedPodcast.PostDownloadCommand.WorkingDirectory.IsSet, Is.EqualTo(false));
        }
    }
}
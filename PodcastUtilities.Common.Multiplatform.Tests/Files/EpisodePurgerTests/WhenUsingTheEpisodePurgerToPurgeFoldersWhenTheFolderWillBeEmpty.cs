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

using System.IO;
using NUnit.Framework;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Multiplatform.Tests.Files.EpisodePurgerTests
{
    public class WhenUsingTheEpisodePurgerToPurgeFoldersWhenTheFolderWillBeEmpty : WhenUsingTheEpisodePurger
    {
        protected override void SetupData()
        {
            base.SetupData();
            _episodesToDelete = new IFileInfo[5];
            _episodesToDelete[0] = TestFileInfo.GenerateFile(Path.Combine(_rootFolder, _podcastInfo.Folder), "2010_04_30_1611_title_.mp3");
            _episodesToDelete[1] = TestFileInfo.GenerateFile(Path.Combine(_rootFolder, _podcastInfo.Folder), "2010_04_26_1611_title_.mp3");
            _episodesToDelete[2] = TestFileInfo.GenerateFile(Path.Combine(_rootFolder, _podcastInfo.Folder), "2010_04_26_1609_title_.mp3");
            _episodesToDelete[3] = TestFileInfo.GenerateFile(Path.Combine(_rootFolder, _podcastInfo.Folder), "2010_04_20_1611_title_.mp3");
            _episodesToDelete[4] = TestFileInfo.GenerateFile(Path.Combine(_rootFolder, _podcastInfo.Folder), "state.xml");
            // note - we do not mark the state.xml or thumbs.db file as being one to delete
        }

        protected override void When()
        {
            _foldersToDelete = _episodePurger.FindEmptyFoldersToDelete(_rootFolder, _podcastInfo, _episodesToDelete);
        }

        protected override void SetupStubs()
        {
            StubFiles();

            base.SetupStubs();
        }

        [Test]
        public void ItShouldReturnTheCorrectFolders()
        {
            Assert.AreEqual(1, _foldersToDelete.Count);
        }
    }
}
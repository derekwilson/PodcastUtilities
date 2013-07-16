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
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Files.UnwantedFileRemoverTests
{
	public class WhenThereAreNoFilesToKeep : WhenTestingUnwantedFileRemover
	{
		protected override void GivenThat()
		{
			base.GivenThat();

			DirectoryInfo.Stub(d => d.Exists)
				.Return(true);
		}

		protected override void When()
		{
			FileRemover.RemoveUnwantedFiles(FilesToKeep, @"c:\x\y\z", "*.mp3", false);
		}

		[Test]
		public void ItShouldGetTheDirectoryFromTheProvider()
		{
			DirectoryInfoProvider.AssertWasCalled(d => d.GetDirectoryInfo(@"c:\x\y\z"));
		}

		[Test]
		public void ItShouldGetFilesInTheDirectory()
		{
			DirectoryInfo.AssertWasCalled(d => d.GetFiles("*.mp3"));
		}

		[Test]
		public void ItShouldDeleteAllTheFiles()
		{
			FileUtilities.AssertWasCalled(u => u.FileDelete(@"c:\a\b\c\1.mp3"));
			FileUtilities.AssertWasCalled(u => u.FileDelete(@"c:\a\b\c\2.mp3"));
			FileUtilities.AssertWasCalled(u => u.FileDelete(@"c:\a\b\c\3.mp3"));
		}

		[Test]
		public void ItShouldSendStatusUpdatesForEachDeletedFile()
		{
			Assert.AreEqual(3, StatusUpdates.Count);

			Assert.That(StatusUpdates[0].Message.Contains(FilesInDirectory[0].FullName));
			Assert.That(StatusUpdates[1].Message.Contains(FilesInDirectory[1].FullName));
			Assert.That(StatusUpdates[2].Message.Contains(FilesInDirectory[2].FullName));
		}
	}
}
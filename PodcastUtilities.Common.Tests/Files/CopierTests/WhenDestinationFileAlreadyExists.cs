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

namespace PodcastUtilities.Common.Tests.Files.CopierTests
{
	public class WhenDestinationFileAlreadyExists
		: WhenTestingCopier
	{
		protected override void GivenThat()
		{
			base.GivenThat();

			DestinationDriveInfo.Stub(i => i.AvailableFreeSpace).Return(1001 * 1024 * 1024);

			FileUtilities.Stub(u => u.FileExists(@"d:\Dest\A"))
				.Return(true);

			FileUtilities.Stub(u => u.FileExists(@"d:\Dest\B"))
				.Return(false);
		}

		[Test]
		public void ItShouldNotCopyTheFileThatExists()
		{
			FileUtilities.AssertWasNotCalled(u => u.FileCopy(@"c:\Source\A", @"d:\Dest\A"));
		}

		[Test]
		public void ItShouldCopyTheOtherFiles()
		{
			FileUtilities.AssertWasCalled(u => u.FileCopy(@"c:\Source\B", @"d:\Dest\B"));
		}

		[Test]
		public void ItShouldOnlySendStatusUpdatesForFilesThatWereCopied()
		{
			Assert.AreEqual(1, StatusUpdates.Count);

			Assert.AreEqual(StatusUpdateLevel.Status, StatusUpdates[0].MessageLevel);
			Assert.AreEqual(@"Copying to: d:\Dest\B", StatusUpdates[0].Message);
		}

	}
}
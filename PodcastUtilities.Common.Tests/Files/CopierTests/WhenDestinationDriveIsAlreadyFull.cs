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
	public class WhenDestinationDriveIsAlreadyFull
		: WhenTestingCopier
	{
		protected override void GivenThat()
		{
			base.GivenThat();

			DestinationDriveInfo.Stub(i => i.AvailableFreeSpace).Return(999 * 1024 * 1024);
		}

		[Test]
		public void ItShouldNotCopyAnyFiles()
		{
			FileUtilities.AssertWasNotCalled(
				f => f.FileCopy(null, null),
				o => o.IgnoreArguments());
		}

		[Test]
		public void ItShouldReportDriveFullStatusUpdates()
		{
			Assert.AreEqual(3, StatusUpdates.Count);

			Assert.AreEqual(StatusUpdateLevel.Status, StatusUpdates[1].MessageLevel);
			Assert.AreEqual("Destination drive is full leaving 1,000 MB free", StatusUpdates[1].Message);
			Assert.AreEqual(StatusUpdateLevel.Status, StatusUpdates[2].MessageLevel);
			Assert.AreEqual("Free Space on drive D is 1,022,976 KB, 999 MB, 0.98 GB", StatusUpdates[2].Message);
		}
	}
}

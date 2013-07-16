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
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Files.FinderTests
{
	public class WhenGettingLessThanAllFilesInAFolder : WhenTestingTheFinder
	{
		protected override void When()
		{
            FoundFiles = FileFinder.GetFiles(@"c:\blah", "*.mp3", 2, PodcastFileSortField.FileName, true);
		}

		[Test]
		public void ItShouldGetTheDirectoryInfoFromTheProvider()
		{
			DirectoryInfoProvider.AssertWasCalled(d => d.GetDirectoryInfo(@"c:\blah"));
		}

		[Test]
		public void ItShouldGetTheFilesFromTheDirectoryInfo()
		{
			DirectoryInfo.AssertWasCalled(d => d.GetFiles("*.mp3"));
		}

		[Test]
		public void ItShouldSortTheFiles()
		{
			FileSorter.AssertWasCalled(
                s => s.Sort(null, PodcastFileSortField.FileName, true),
                o => o.Constraints(Rhino.Mocks.Constraints.Property.Value("Count", 3), Rhino.Mocks.Constraints.Is.Equal(PodcastFileSortField.FileName), Rhino.Mocks.Constraints.Is.Equal(true)));
		}

		[Test]
		public void ItShouldReturnTheCorrectSubsetOfFiles()
		{
			Assert.AreEqual(2, FoundFiles.Count);
			Assert.AreEqual(FilesInDirectory[0], FoundFiles[0]);
			Assert.AreEqual(FilesInDirectory[1], FoundFiles[1]);
		}
	}
}
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
using Moq;
using NUnit.Framework;

namespace PodcastUtilities.Common.Multiplatform.Tests.Files.CopierTests
{
    public class WhenCopyingThrowsAnException
        : WhenTestingCopier
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            DestinationDriveInfo.Setup(i => i.AvailableFreeSpace).Returns(1001 * 1024 * 1024);

            FileUtilities.Setup(u => u.FileCopy(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new FileNotFoundException("blah"));
        }

        [Test]
        public void ItShouldReportErrorStatusUpdate()
        {
            Assert.AreEqual(2, StatusUpdates.Count);

            Assert.AreEqual(StatusUpdateLevel.Error, StatusUpdates[1].MessageLevel);
            Assert.AreEqual("Error writing file: blah", StatusUpdates[1].Message);
        }

    }
}
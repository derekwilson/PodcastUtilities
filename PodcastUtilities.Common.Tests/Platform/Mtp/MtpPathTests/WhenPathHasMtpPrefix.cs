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
using PodcastUtilities.Common.Platform.Mtp;

namespace PodcastUtilities.Common.Tests.Platform.Mtp.MtpPathTests
{
    public class WhenPathHasMtpPrefix
        : WhenTestingBehaviour
    {
        private string _pathToTest;

        protected override void GivenThat()
        {
            base.GivenThat();

            _pathToTest = @"mtp:\my device\path";
        }

        protected override void When()
        {
        }

        [Test]
        public void IsMtpPathShouldReturnTrue()
        {
            Assert.That(MtpPath.IsMtpPath(_pathToTest), Is.True);
        }

        [Test]
        public void StripMtpPrefixShouldRemoveMtpPrefix()
        {
            Assert.That(MtpPath.StripMtpPrefix(_pathToTest), Is.EqualTo(@"my device\path"));
        }

        [Test]
        public void MakeFullPathShouldReturnOriginalPath()
        {
            Assert.That(MtpPath.MakeFullPath(_pathToTest), Is.EqualTo(@"mtp:\my device\path"));
        }

        [Test]
        public void GetMtpPathInfoShouldSetIsMtpPathToTrue()
        {
            Assert.That(MtpPath.GetPathInfo(_pathToTest).IsMtpPath, Is.True);
        }

        [Test]
        public void GetMtpPathInfoShouldCorrectlySetDeviceName()
        {
            Assert.That(MtpPath.GetPathInfo(_pathToTest).DeviceName, Is.EqualTo("my device"));
        }

        [Test]
        public void GetMtpPathInfoShouldCorrectlySetPath()
        {
            Assert.That(MtpPath.GetPathInfo(_pathToTest).RelativePathOnDevice, Is.EqualTo("path"));
        }
    }
}
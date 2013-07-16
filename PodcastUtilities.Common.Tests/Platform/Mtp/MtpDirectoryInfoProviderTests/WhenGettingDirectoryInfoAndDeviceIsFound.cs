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
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Common.Platform.Mtp;
using PodcastUtilities.PortableDevices;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Platform.Mtp.MtpDirectoryInfoProviderTests
{
    public class WhenGettingDirectoryInfoAndDeviceIsFound : WhenTestingMtpDirectoryInfoProvider
    {
        protected IDevice Device { get; set; }
        protected IDirectoryInfo DirectoryInfo { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Device = GenerateMock<IDevice>();
            Device.Stub(device => device.Name)
                .Return("test device");

            DeviceManager.Stub(manager => manager.GetDevice("test device"))
                .Return(Device);
        }

        protected override void When()
        {
            DirectoryInfo = DirectoryInfoProvider.GetDirectoryInfo(@"mtp:\test device\a\b\c");
        }

        [Test]
        public void ItShouldCreateCorrectTypeOfDirectoryInfo()
        {
            Assert.That(DirectoryInfo, Is.InstanceOf<DirectoryInfo>());
        }

        [Test]
        public void ItShouldCreateDirectoryInfoCorrectly()
        {
            Assert.That(DirectoryInfo.FullName, Is.EqualTo(@"MTP:\test device\a\b\c"));
        }
    }
}
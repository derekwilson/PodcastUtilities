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
using PodcastUtilities.PortableDevices;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Platform.Mtp.MtpDriveInfoProviderTests
{
    public class WhenGettingDriveInfoAndDeviceIsFoundAndHasStorageObject : WhenGettingDriveInfoAndDeviceIsFound
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            var storageObject = GenerateMock<IDeviceObject>();
            storageObject.Stub(storage => storage.Name)
                .Return("storage");

            Device.Stub(device => device.GetRootStorageObjectFromPath(@"storage\b\c"))
                .Return(storageObject);
        }

        protected override void When()
        {
            DriveInfo = DriveInfoProvider.GetDriveInfoForPath(@"mtp:\test device\storage\b\c");
        }

        [Test]
        public void ItShouldCreateCorrectTypeOfDriveInfo()
        {
            Assert.That(DriveInfo, Is.InstanceOf<DriveInfo>());
        }

        [Test]
        public void ItShouldCreateDriveInfoCorrectly()
        {
            Assert.That(DriveInfo.Name, Is.EqualTo(@"storage"));
        }
    }
}
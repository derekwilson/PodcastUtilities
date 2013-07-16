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
using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceTests
{
    public class WhenGettingRootStorageObjectFromFullPath : WhenTestingDevice
    {
        private IDeviceObject DeviceObject { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            PortableDeviceHelper.Stub(
                helper => helper.GetChildObjectIds(PortableDeviceContent, PortableDeviceConstants.WPD_DEVICE_OBJECT_ID))
                .Return(new[] {"Dummy1", "xId", "Dummy2"});

            PortableDeviceHelper.Stub(
                helper => helper.GetChildObjectIds(PortableDeviceContent, "xId"))
                .Return(new[] {"Dummy3", "yId", "Dummy4"});

            PortableDeviceHelper.Stub(
                helper => helper.GetChildObjectIds(PortableDeviceContent, "yId"))
                .Return(new[] { "Dummy5", "InternalStorageId", "Dummy6" });

            StubObjectFilenameAndContentType("xId", "x", PortableDeviceConstants.WPD_CONTENT_TYPE_FOLDER);
            StubObjectFilenameAndContentType("yId", "y", PortableDeviceConstants.WPD_CONTENT_TYPE_FOLDER);
            StubObjectFilenameAndContentType("InternalStorageId", "Internal Storage", PortableDeviceConstants.WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT);

            PortableDeviceHelper.Stub(
                propertyHelper => propertyHelper.GetGuidProperty(
                    PortableDeviceContent, 
                    "InternalStorageId",
                    PortableDevicePropertyKeys.WPD_FUNCTIONAL_OBJECT_CATEGORY))
                .Return(PortableDeviceConstants.WPD_FUNCTIONAL_CATEGORY_STORAGE);
        }

        private void StubObjectFilenameAndContentType(string objectId, string fileName, Guid contentType)
        {
            PortableDeviceHelper
                .Stub(propertyHelper => propertyHelper.GetObjectFileName(
                    PortableDeviceContent,
                    objectId))
                .Return(fileName);

            PortableDeviceHelper.Stub(
                propertyHelper => propertyHelper.GetObjectContentType(PortableDeviceContent, objectId))
                .Return(contentType);
        }

        protected override void When()
        {
            // The storage object will normally be at the root, but we need to allow for it to be lower in the tree
            DeviceObject = Device.GetRootStorageObjectFromPath(@"x\y\Internal Storage\Foo\Bar");
        }

        [Test]
        public void ItShouldReturnTheCorrectObject()
        {
            Assert.That(DeviceObject.Id, Is.EqualTo("InternalStorageId"));
            Assert.That(DeviceObject.Name, Is.EqualTo("Internal Storage"));
        }
    }
}
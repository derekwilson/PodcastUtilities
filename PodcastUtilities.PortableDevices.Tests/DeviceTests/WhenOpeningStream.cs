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
using System.IO;
using PortableDeviceApiLib;
using Rhino.Mocks;
using R = Rhino.Mocks.Constraints;

namespace PodcastUtilities.PortableDevices.Tests.DeviceTests
{
    public abstract class WhenOpeningStream : WhenTestingDevice
    {
        protected Stream OpenedStream { get; set; }
        protected Exception ThrownException { get; set; }
        protected IStream UnderlyingStream { get; set; }
        protected Stream DeviceStream { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            PortableDeviceHelper.Stub(
                helper => helper.GetChildObjectIds(PortableDeviceContent, PortableDeviceConstants.WPD_DEVICE_OBJECT_ID))
                .Return(new[] { "InternalStorageID" });

            PortableDeviceHelper.Stub(
                helper => helper.GetChildObjectIds(PortableDeviceContent, "InternalStorageID"))
                .Return(new[] { "fooId", "xId" });

            PortableDeviceHelper
                .Stub(propertyHelper => propertyHelper.GetObjectFileName(
                    PortableDeviceContent,
                    "InternalStorageID"))
                .Return("Internal Storage");

            PortableDeviceHelper
                .Stub(propertyHelper => propertyHelper.GetObjectFileName(
                    PortableDeviceContent,
                    "xId"))
                .Return("x");

            PortableDeviceHelper
                .Stub(propertyHelper => propertyHelper.GetObjectFileName(
                    PortableDeviceContent,
                    "fooId"))
                .Return("foo.mp3");

            UnderlyingStream = GenerateMock<IStream>();

            DeviceStream = GeneratePartialMock<Stream>();
        }

        protected override void When()
        {
            try
            {
                DoWhen();
            }
            catch (Exception exception)
            {
                ThrownException = exception;
            }
        }

        protected abstract void DoWhen();
    }
}
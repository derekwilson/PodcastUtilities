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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using PodcastUtilities.Common.Tests;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceStreamTests
{
    public abstract class WhenTestingDeviceStream
        : WhenTestingBehaviour
    {
        protected DeviceStream DeviceStream { get; set; }
        protected IStream Stream { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Stream = GenerateMock<IStream>();

            // Make seek return a non-zero position
            Stream.Stub(stream => stream.Seek(0, 0, IntPtr.Zero))
                .IgnoreArguments()
                .WhenCalled(invocation => Marshal.WriteInt64((IntPtr) invocation.Arguments[2], 1234));

            DeviceStream = new DeviceStream(Stream);
        }
    }
}
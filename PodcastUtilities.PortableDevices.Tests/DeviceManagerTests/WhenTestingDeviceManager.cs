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
using PodcastUtilities.Common.Tests;
using PortableDeviceApiLib;
using Rhino.Mocks;
using System.Runtime.InteropServices;

namespace PodcastUtilities.PortableDevices.Tests.DeviceManagerTests
{
    public abstract class WhenTestingDeviceManager
        : WhenTestingBehaviour
    {
        protected DeviceManager DeviceManager { get; set; }
        protected IPortableDeviceManager PortableDeviceManager { get; set; }
        protected IPortableDevice PortableDevice { get; set; }
        protected IDeviceFactory DeviceFactory { get; set; }
        protected IDevice Device1 { get; set; }
        protected IDevice Device2 { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();
            // genius: .NET4 doesnt work properly with Rhino
            // https://stackoverflow.com/questions/3444581/mocking-com-interfaces-using-rhino-mocks
            // if we need to use .NET4 then uncomment this line
            //Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add(typeof(TypeIdentifierAttribute));

            PortableDeviceManager = new MockPortableDeviceManager();

            PortableDevice = GenerateMock<IPortableDevice>();

            Device1 = GenerateMock<IDevice>();
            Device1.Stub(device => device.Name)
                .Return("Device 1");
            Device1.Stub(device => device.Id)
                .Return("Device_Id_1");

            Device2 = GenerateMock<IDevice>();
            Device2.Stub(device => device.Name)
                .Return("Device 2");
            Device2.Stub(device => device.Id)
                .Return("Device_Id_2");

            DeviceFactory = GenerateMock<IDeviceFactory>();
            DeviceFactory.Stub(factory => factory.CreateDevice("Device_Id_1"))
                .Return(Device1);
            DeviceFactory.Stub(factory => factory.CreateDevice("Device_Id_2"))
                .Return(Device2);

            DeviceManager = new DeviceManager(PortableDeviceManager, DeviceFactory);
        }
    }
}
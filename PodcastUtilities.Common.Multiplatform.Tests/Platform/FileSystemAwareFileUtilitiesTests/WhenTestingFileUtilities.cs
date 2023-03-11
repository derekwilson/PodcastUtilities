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
using Moq;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Common.Multiplatform.Tests.Platform.FileSystemAwareFileUtilitiesTests
{
    public abstract class WhenTestingFileUtilities
        : WhenTestingBehaviour
    {
        protected Mock<IFileUtilities> RegularFileUtilities { get; set; }
        protected Mock<IDeviceManager> DeviceManager { get; set; }
        protected Mock<IStreamHelper> StreamHelper { get; set; }
        protected Mock<IFileInfoProvider> FileInfoProvider { get; set; }

        protected Mock<Stream> SourceStream { get; set; }
        protected Mock<Stream> DestinationStream { get; set; }

        protected Mock<IDevice> Device { get; set; }

        protected FileSystemAwareFileUtilities Utilities { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            RegularFileUtilities = GenerateMock<IFileUtilities>();
            DeviceManager = GenerateMock<IDeviceManager>();
            StreamHelper = GenerateMock<IStreamHelper>();
            FileInfoProvider = GenerateMock<IFileInfoProvider>();

            SourceStream = GeneratePartialMock<Stream>();
            DestinationStream = GeneratePartialMock<Stream>();

            Device = GenerateMock<IDevice>();

            Utilities = FileSystemAwareFileUtilities.getUtilitiesForUnitTests(RegularFileUtilities.Object, DeviceManager.Object, StreamHelper.Object, FileInfoProvider.Object);
        }
    }
}
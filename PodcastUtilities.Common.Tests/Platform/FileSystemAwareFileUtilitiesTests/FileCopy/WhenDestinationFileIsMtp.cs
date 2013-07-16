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
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Platform.FileSystemAwareFileUtilitiesTests.FileCopy
{
    public class WhenDestinationFileIsMtp
        : WhenTestingFileUtilities
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            DeviceManager.Stub(manager => manager.GetDevice("test device"))
                .Return(Device);

            StreamHelper.Stub(helper => helper.OpenRead(@"D:\foo2\bar.abc"))
                .Return(SourceStream);

            var fileInfo = GenerateMock<IFileInfo>();
            fileInfo.Stub(info => info.Length)
                .Return(1234);
            FileInfoProvider.Stub(provider => provider.GetFileInfo(@"D:\foo2\bar.abc"))
                .Return(fileInfo);

            Device.Stub(device => device.OpenWrite(@"path\file.xyz", 1234, true))
                .Return(DestinationStream);
        }

        protected override void When()
        {
            Utilities.FileCopy(@"D:\foo2\bar.abc", @"MTP:\test device\path\file.xyz", true);
        }

        [Test]
        public void ItShouldNotDelegateToRegularFileUtilities()
        {
            RegularFileUtilities.AssertWasNotCalled(
                utilities => utilities.FileCopy(null, null, true),
                options => options.IgnoreArguments());
        }

        [Test]
        public void ItShouldCopyFromSourceFileStreamToDestinationDeviceStream()
        {
            StreamHelper.AssertWasCalled(helper => helper.Copy(SourceStream, DestinationStream));
        }
    }
}
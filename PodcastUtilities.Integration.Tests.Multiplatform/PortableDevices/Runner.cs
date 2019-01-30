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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Integration.Tests.PortableDevices
{
    class Runner : RunnerBase
    {
        private IEnumerable<IDevice> _devices = null;

        public Runner(string testToRun)
            : base(testToRun)
        {
        }

        public override void RunAllTests()
        {
            DisplayMessage("PortableDevices Tests:",DisplayLevel.Title);
            if (!ShouldRunTests("mtp"))
            {
                DisplayMessage(" tests skipped");
                return;
            }

            _devices = EnumerateAllDevices();

            if (_devices.Count() > 0)
            {
                RunOneTest(CreateFolderInRoot);
                RunOneTest(DeleteFolderInRoot);
                RunOneTest(CreateFolderWithSubfolders);
                RunOneTest(DeleteFolderWithSubfolders);
                RunOneTest(CopyFile);
            }
        }

        private string GetRootFolder()
        {
            IDevice device = _devices.First();
            var objects = device.GetDeviceRootStorageObjects();

            return string.Format("MTP:\\{0}\\{1}\\podcastutilities.integration.test", _devices.First().Name, objects.First().Name);
        }

        private string GetDestinationPath()
        {
            return string.Format("{0}\\{1}", GetRootFolder(), GetSourcePath());
        }

        private string GetSourcePath()
        {
            return "srcfile.txt";
        }

        private void CreateFolderWithSubfolders()
        {
            string folder = string.Format("{0}\\{1}", GetRootFolder(), "folder1");

            TestCreateFolder(folder);
        }

        private void CreateFolderInRoot()
        {
            string folder = GetRootFolder();

            TestCreateFolder(folder);
        }

        private void TestCreateFolder(string folder)
        {
            DisplayMessage(string.Format("Creating {0}", folder));

            FileSystemAwareDirectoryInfoProvider dirInfoProvider = new FileSystemAwareDirectoryInfoProvider(new DeviceManager());

            IDirectoryInfo info = dirInfoProvider.GetDirectoryInfo(folder);
            info.Create();

            if (info.Exists)
            {
                DisplayMessage(string.Format("{0} Created OK", info.FullName));
            }
            else
            {
                DisplayMessage(string.Format("{0} Failed to create", info.FullName), DisplayLevel.Error);
            }
        }

        private void DeleteFolderInRoot()
        {
            string folder = GetRootFolder();

            DisplayMessage(string.Format("Deleting {0}", folder));

            FileSystemAwareDirectoryInfoProvider dirInfoProvider = new FileSystemAwareDirectoryInfoProvider(new DeviceManager());

            IDirectoryInfo info = dirInfoProvider.GetDirectoryInfo(folder);
            info.Delete();

            if (!info.Exists)
            {
                DisplayMessage(string.Format("{0} Deleted OK", info.FullName));
            }
            else
            {
                DisplayMessage(string.Format("{0} Failed to delete", info.FullName), DisplayLevel.Error);
            }
        }

        private void DeleteFolderWithSubfolders()
        {
            string folder = string.Format("{0}\\{1}", GetRootFolder(), "folder1");

            FileSystemAwareDirectoryInfoProvider dirInfoProvider = new FileSystemAwareDirectoryInfoProvider(new DeviceManager());

            DisplayMessage(string.Format("Deleting {0}", folder));

            IDirectoryInfo info1 = dirInfoProvider.GetDirectoryInfo(folder);
            info1.Delete();
            if (!info1.Exists)
            {
                DisplayMessage(string.Format("{0} Deleted OK", info1.FullName));
            }
            else
            {
                DisplayMessage(string.Format("{0} Failed to delete", info1.FullName), DisplayLevel.Error);
            }

            IDirectoryInfo info2 = dirInfoProvider.GetDirectoryInfo(GetRootFolder());
            info2.Delete();
            if (!info2.Exists)
            {
                DisplayMessage(string.Format("{0} Deleted OK", info2.FullName));
            }
            else
            {
                DisplayMessage(string.Format("{0} Failed to delete", info2.FullName), DisplayLevel.Error);
            }
        }

        private void CopyFile()
        {
            FileSystemAwareFileUtilities fileUtils = new FileSystemAwareFileUtilities(new DeviceManager(), new StreamHelper(), new FileSystemAwareFileInfoProvider(new DeviceManager()));

            if (!fileUtils.FileExists(GetSourcePath()))
            {
                DisplayMessage(string.Format("Cannot find source file {0}",GetSourcePath()),DisplayLevel.Error);
            }

            if (fileUtils.FileExists(GetDestinationPath()))
            {
                fileUtils.FileDelete(GetDestinationPath());
            }

            // put this section in the test preable
            string folder = GetRootFolder();
            FileSystemAwareDirectoryInfoProvider dirInfoProvider = new FileSystemAwareDirectoryInfoProvider(new DeviceManager());
            IDirectoryInfo info = dirInfoProvider.GetDirectoryInfo(folder);
            info.Create();

            fileUtils.FileCopy(GetSourcePath(),GetDestinationPath());

            if (fileUtils.FileExists(GetDestinationPath()))
            {
                DisplayMessage(string.Format("File copied to {0} OK", GetDestinationPath()));
            }
            else
            {
                DisplayMessage(string.Format("Cannot find destination file {0}", GetDestinationPath()), DisplayLevel.Error);
            }
        }

        private void ScrubOutAllTestData()
        {
            // note you may want to comment this out to see what the hell is going on

            // get rid of any file that we created
            FileSystemAwareFileUtilities fileUtils = new FileSystemAwareFileUtilities(new DeviceManager(), new StreamHelper(), new FileSystemAwareFileInfoProvider(new DeviceManager()));
            if (fileUtils.FileExists(GetDestinationPath()))
            {
                fileUtils.FileDelete(GetDestinationPath());
            }

            // get rid of any folders we created
            FileSystemAwareDirectoryInfoProvider dirInfoProvider = new FileSystemAwareDirectoryInfoProvider(new DeviceManager());
            IDirectoryInfo info = dirInfoProvider.GetDirectoryInfo(GetRootFolder());
            info.Delete();
        }

        protected override void TestPreamble()
        {
            ScrubOutAllTestData();
        }

        protected override void TestPostamble()
        {
            ScrubOutAllTestData();
        }

        private IEnumerable<IDevice> EnumerateAllDevices()
        {
            IDeviceManager manager = new DeviceManager();

            IEnumerable<IDevice> devices = manager.GetAllDevices();
            DisplayMessage(string.Format("{0} Devices Found", devices.Count()));

            foreach (var device in devices)
            {
                DisplayMessage(string.Format("Name: {0}, ID: {1}", device.Name, device.Id));
            }

            return devices;
        }
    }
}

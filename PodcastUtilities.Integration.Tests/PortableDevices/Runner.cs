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
            }
        }

        private string GetRootFolder()
        {
            return string.Format("MTP:\\{0}\\Internal Memory\\podcastutilities.integration.test", _devices.First().Name);
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

            DisplayMessage(string.Format("Deleteing {0}", folder));

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

            DisplayMessage(string.Format("Deleteing {0}", folder));

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

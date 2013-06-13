using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                RunOneTest(CreateFolder);
            }
        }

        private void CreateFolder()
        {
            throw new NotImplementedException();
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

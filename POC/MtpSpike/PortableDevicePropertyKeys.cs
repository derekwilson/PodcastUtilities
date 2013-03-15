using System;

namespace MtpSpike
{
    public static class PortableDevicePropertyKeys
    {
        // From PortableDevice.h in the windows SDK

        static PortableDevicePropertyKeys()
        {
            WPD_OBJECT_ID.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_ID.pid = 2;

            WPD_OBJECT_NAME.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_NAME.pid = 4;

            WPD_CLIENT_NAME.fmtid = new Guid(0x204D9F0C, 0x2292, 0x4080, 0x9F, 0x42, 0x40, 0x66, 0x4E, 0x70, 0xF8, 0x59);
            WPD_CLIENT_NAME.pid = 2;

            WPD_CLIENT_MAJOR_VERSION.fmtid = new Guid(0x204D9F0C, 0x2292, 0x4080, 0x9F, 0x42, 0x40, 0x66, 0x4E, 0x70, 0xF8, 0x59);
            WPD_CLIENT_MAJOR_VERSION.pid = 3;

            WPD_CLIENT_MINOR_VERSION.fmtid = new Guid(0x204D9F0C, 0x2292, 0x4080, 0x9F, 0x42, 0x40, 0x66, 0x4E, 0x70, 0xF8, 0x59);
            WPD_CLIENT_MINOR_VERSION.pid = 4;

            WPD_CLIENT_REVISION.fmtid = new Guid(0x204D9F0C, 0x2292, 0x4080, 0x9F, 0x42, 0x40, 0x66, 0x4E, 0x70, 0xF8, 0x59);
            WPD_CLIENT_REVISION.pid = 5;
        }

        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_ID;
        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_NAME;
        public static PortableDeviceApiLib._tagpropertykey WPD_CLIENT_NAME;
        public static PortableDeviceApiLib._tagpropertykey WPD_CLIENT_MAJOR_VERSION;
        public static PortableDeviceApiLib._tagpropertykey WPD_CLIENT_MINOR_VERSION;
        public static PortableDeviceApiLib._tagpropertykey WPD_CLIENT_REVISION;
    }
}
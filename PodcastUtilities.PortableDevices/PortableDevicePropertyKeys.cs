using System;

namespace PodcastUtilities.PortableDevices
{
    internal static class PortableDevicePropertyKeys
    {
        // From PortableDevice.h in the windows SDK

        static PortableDevicePropertyKeys()
        {
            WPD_OBJECT_ID.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_ID.pid = 2;

            WPD_OBJECT_PARENT_ID.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_PARENT_ID.pid = 3;

            WPD_OBJECT_NAME.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_NAME.pid = 4;

            WPD_OBJECT_CONTENT_TYPE.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_CONTENT_TYPE.pid = 7;

            WPD_OBJECT_ORIGINAL_FILE_NAME.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_ORIGINAL_FILE_NAME.pid = 12;

            WPD_OBJECT_DATE_CREATED.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_DATE_CREATED.pid = 18;

            WPD_OBJECT_DATE_MODIFIED.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_DATE_MODIFIED.pid = 19;

            WPD_CLIENT_NAME.fmtid = new Guid(0x204D9F0C, 0x2292, 0x4080, 0x9F, 0x42, 0x40, 0x66, 0x4E, 0x70, 0xF8, 0x59);
            WPD_CLIENT_NAME.pid = 2;

            WPD_CLIENT_MAJOR_VERSION.fmtid = new Guid(0x204D9F0C, 0x2292, 0x4080, 0x9F, 0x42, 0x40, 0x66, 0x4E, 0x70, 0xF8, 0x59);
            WPD_CLIENT_MAJOR_VERSION.pid = 3;

            WPD_CLIENT_MINOR_VERSION.fmtid = new Guid(0x204D9F0C, 0x2292, 0x4080, 0x9F, 0x42, 0x40, 0x66, 0x4E, 0x70, 0xF8, 0x59);
            WPD_CLIENT_MINOR_VERSION.pid = 4;

            WPD_CLIENT_REVISION.fmtid = new Guid(0x204D9F0C, 0x2292, 0x4080, 0x9F, 0x42, 0x40, 0x66, 0x4E, 0x70, 0xF8, 0x59);
            WPD_CLIENT_REVISION.pid = 5;

            WPD_RESOURCE_DEFAULT.fmtid = new Guid(0xE81E79BE, 0x34F0, 0x41BF, 0xB5, 0x3F, 0xF1, 0xA0, 0x6A, 0xE8, 0x78, 0x42);
            WPD_RESOURCE_DEFAULT.pid = 0;
        }

        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_ID;
        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_PARENT_ID;
        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_NAME;
        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_CONTENT_TYPE;
        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_ORIGINAL_FILE_NAME;
        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_DATE_CREATED;
        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_DATE_MODIFIED;
        public static PortableDeviceApiLib._tagpropertykey WPD_CLIENT_NAME;
        public static PortableDeviceApiLib._tagpropertykey WPD_CLIENT_MAJOR_VERSION;
        public static PortableDeviceApiLib._tagpropertykey WPD_CLIENT_MINOR_VERSION;
        public static PortableDeviceApiLib._tagpropertykey WPD_CLIENT_REVISION;
        public static PortableDeviceApiLib._tagpropertykey WPD_RESOURCE_DEFAULT;
    }
}
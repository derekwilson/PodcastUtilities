using System;

namespace PodcastUtilities.PortableDevices
{
    internal static class PortableDeviceConstants
    {
        // From PortableDevice.h in the windows SDK
        public const string WPD_DEVICE_OBJECT_ID = "DEVICE";

        public const int VT_LPWSTR = 31;
        public const int VT_CLSID = 72;
        public const int VT_DATE = 7;
        public const int VT_BOOL = 11;
        public const int VT_UI4 = 19;
        public const int VT_UI8 = 21;

        public const uint PORTABLE_DEVICE_DELETE_NO_RECURSION = 0;
        public const uint PORTABLE_DEVICE_DELETE_WITH_RECURSION = 1;

        public static Guid WPD_CONTENT_TYPE_FOLDER;
        public static Guid WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT;
        public static Guid WPD_FUNCTIONAL_CATEGORY_STORAGE;

        static PortableDeviceConstants()
        {
            WPD_CONTENT_TYPE_FOLDER = new Guid(0x27E2E392, 0xA111, 0x48E0, 0xAB, 0x0C, 0xE1, 0x77, 0x05, 0xA0, 0x5F, 0x85);
            WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT = new Guid(0x99ED0160, 0x17FF, 0x4C44, 0x9D, 0x98, 0x1D, 0x7A, 0x6F, 0x94, 0x19, 0x21);
            WPD_FUNCTIONAL_CATEGORY_STORAGE = new Guid(0x23F05BBC, 0x15DE, 0x4C2A, 0xA5, 0x5B, 0xA9, 0xAF, 0x5C, 0xE4, 0x12, 0xEF);
        }
    }
}
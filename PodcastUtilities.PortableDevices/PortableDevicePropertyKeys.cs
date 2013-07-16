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

            WPD_OBJECT_SIZE.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_SIZE.pid = 11;

            WPD_OBJECT_ORIGINAL_FILE_NAME.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_ORIGINAL_FILE_NAME.pid = 12;

            WPD_OBJECT_DATE_CREATED.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_DATE_CREATED.pid = 18;

            WPD_OBJECT_DATE_MODIFIED.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_DATE_MODIFIED.pid = 19;

            WPD_FUNCTIONAL_OBJECT_CATEGORY.fmtid = new Guid(0x8F052D93, 0xABCA, 0x4FC5, 0xA5, 0xAC, 0xB0, 0x1D, 0xF4, 0xDB, 0xE5, 0x98);
            WPD_FUNCTIONAL_OBJECT_CATEGORY.pid = 2;

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

            WPD_STORAGE_FREE_SPACE_IN_BYTES.fmtid = new Guid(0x01A3057A, 0x74D6, 0x4E80, 0xBE, 0xA7, 0xDC, 0x4C, 0x21, 0x2C, 0xE5, 0x0A);
            WPD_STORAGE_FREE_SPACE_IN_BYTES.pid = 5;
        }

        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_ID;
        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_PARENT_ID;
        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_NAME;
        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_CONTENT_TYPE;
        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_SIZE;
        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_ORIGINAL_FILE_NAME;
        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_DATE_CREATED;
        public static PortableDeviceApiLib._tagpropertykey WPD_OBJECT_DATE_MODIFIED;
        public static PortableDeviceApiLib._tagpropertykey WPD_FUNCTIONAL_OBJECT_CATEGORY;
        public static PortableDeviceApiLib._tagpropertykey WPD_CLIENT_NAME;
        public static PortableDeviceApiLib._tagpropertykey WPD_CLIENT_MAJOR_VERSION;
        public static PortableDeviceApiLib._tagpropertykey WPD_CLIENT_MINOR_VERSION;
        public static PortableDeviceApiLib._tagpropertykey WPD_CLIENT_REVISION;
        public static PortableDeviceApiLib._tagpropertykey WPD_RESOURCE_DEFAULT;
        public static PortableDeviceApiLib._tagpropertykey WPD_STORAGE_FREE_SPACE_IN_BYTES;
    }
}
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
        public static Guid WPD_CONTENT_TYPE_GENERIC_FILE;
        public static Guid WPD_FUNCTIONAL_CATEGORY_STORAGE;

        static PortableDeviceConstants()
        {
            WPD_CONTENT_TYPE_FOLDER = new Guid(0x27E2E392, 0xA111, 0x48E0, 0xAB, 0x0C, 0xE1, 0x77, 0x05, 0xA0, 0x5F, 0x85);
            WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT = new Guid(0x99ED0160, 0x17FF, 0x4C44, 0x9D, 0x98, 0x1D, 0x7A, 0x6F, 0x94, 0x19, 0x21);
            WPD_CONTENT_TYPE_GENERIC_FILE = new Guid(0x0085E0A6, 0x8D34, 0x45D7, 0xBC, 0x5C, 0x44, 0x7E, 0x59, 0xC7, 0x3D, 0x48);
            WPD_FUNCTIONAL_CATEGORY_STORAGE = new Guid(0x23F05BBC, 0x15DE, 0x4C2A, 0xA5, 0x5B, 0xA9, 0xAF, 0x5C, 0xE4, 0x12, 0xEF);
        }
    }
}
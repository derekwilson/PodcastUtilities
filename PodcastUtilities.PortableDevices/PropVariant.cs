using System;
using System.Runtime.InteropServices;
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    internal struct PropVariant
    {
        [FieldOffset(0)]
        public short variantType;
        [FieldOffset(8)]
        public IntPtr pointerValue;
        [FieldOffset(8)]
        public byte byteValue;
        [FieldOffset(8)]
        public long intValue;
        [FieldOffset(8)]
        public long longValue;
        [FieldOffset(8)]
        public double dateValue;
        [FieldOffset(8)]
        public short boolValue;

        public static PropVariant FromValue(tag_inner_PROPVARIANT value)
        {
            IntPtr ptrValue = Marshal.AllocHGlobal(Marshal.SizeOf(value));
            Marshal.StructureToPtr(value, ptrValue, false);

            //
            // Marshal the pointer into our C# object
            //
            return (PropVariant)Marshal.PtrToStructure(ptrValue, typeof(PropVariant));
        }

        public string AsString()
        {
            switch (variantType)
            {
                case PortableDeviceConstants.VT_LPWSTR:
                    return Marshal.PtrToStringUni(pointerValue);

                case PortableDeviceConstants.VT_CLSID:
                    return ToGuid().ToString();

                case PortableDeviceConstants.VT_DATE:
                    return ToDate().ToString();

                case PortableDeviceConstants.VT_BOOL:
                    return ToBool().ToString();

                case PortableDeviceConstants.VT_UI4:
                    return intValue.ToString();

                case PortableDeviceConstants.VT_UI8:
                    return longValue.ToString();
            }

            return variantType.ToString();
        }

        public Guid ToGuid()
        {
            return (Guid)Marshal.PtrToStructure(pointerValue, typeof(Guid));
        }

        public DateTime ToDate()
        {
            return DateTime.FromOADate(dateValue);
        }

        public bool ToBool()
        {
            return Convert.ToBoolean(boolValue);
        }

        public static tag_inner_PROPVARIANT StringToPropVariant(string value)
        {
            // Tried using the method suggested here:
            // http://blogs.msdn.com/b/dimeby8/archive/2007/01/08/creating-wpd-propvariants-in-c-without-using-interop.aspx
            // However, the GetValue fails (Element Not Found) even though we've just added it.
            // So, I use the alternative (and I think more "correct") approach below.

            var pvSet = new PropVariant
                            {
                                variantType = PortableDeviceConstants.VT_LPWSTR, 
                                pointerValue = Marshal.StringToCoTaskMemUni(value)
                            };

            // Marshal our definition into a pointer
            var ptrValue = Marshal.AllocHGlobal(Marshal.SizeOf(pvSet));
            Marshal.StructureToPtr(pvSet, ptrValue, false);

            // Marshal pointer into the interop PROPVARIANT 
            return (tag_inner_PROPVARIANT)Marshal.PtrToStructure(ptrValue, typeof(tag_inner_PROPVARIANT));
        }
    }
}
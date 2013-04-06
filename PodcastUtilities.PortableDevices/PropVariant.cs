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
        public long longValue;

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
            if (variantType != PortableDeviceConstants.VT_LPWSTR)
            {
                return variantType.ToString();
            }

            return Marshal.PtrToStringUni(pointerValue);
        }
    }
}
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
    }
}
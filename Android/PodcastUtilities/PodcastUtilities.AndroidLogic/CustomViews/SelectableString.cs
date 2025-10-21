using Android.OS;
using Android.Runtime;
using System;
using System.Collections.Generic;
using static Android.Provider.Settings;

namespace PodcastUtilities.AndroidLogic.CustomViews
{
    public class SelectableString : Java.Lang.Object, IParcelable
    {
        public static SelectableString GenerateOption<TYPE>(TYPE enumOption, TYPE currentValue)
        {
            return new SelectableString(Convert.ToInt32(enumOption), enumOption?.ToString()!, EqualityComparer<TYPE>.Default.Equals(enumOption, currentValue));
        }

        public static SelectableString GenerateOption<TYPE>(TYPE enumOption, bool selected)
        {
            return new SelectableString(Convert.ToInt32(enumOption), enumOption?.ToString()!, selected);
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }

        public SelectableString(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public SelectableString(int id, string name, bool selected)
        {
            Id = id;
            Name = name;
            Selected = selected;
        }

        public SelectableString(Parcel parcel)
        {
            Id = parcel.ReadInt();
            Name = parcel.ReadString()!;
            //Selected = parcel.ReadBoolean();
            Selected = parcel.ReadInt() == 1;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteInt(Id);
            dest.WriteString(Name);
            //dest.WriteBoolean(Selected);
            dest.WriteInt(Selected ? 1 : 0);
        }

        [Java.Interop.ExportField("CREATOR")]
        public static SelectableStringParcelableCreator InitializeCreator()
        {
            return new SelectableStringParcelableCreator();
        }

        public int DescribeContents()
        {
            return 0;
        }
    }

    public class SelectableStringParcelableCreator : Java.Lang.Object, IParcelableCreator
    {
        Java.Lang.Object IParcelableCreator.CreateFromParcel(Parcel? source)
        {
            if (source == null)
            {
                throw new InvalidOperationException("no parcel");
            }
            return new SelectableString(source);
        }

        Java.Lang.Object[] IParcelableCreator.NewArray(int size)
        {
            return new SelectableString[size];
        }
    }
}
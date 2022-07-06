using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.AndroidLogic.CustomViews
{
    public interface IDriveVolumeInfoViewFactory
    {
        IDriveVolumeInfoView GetNewView(Context context);
    }

    public class DriveVolumeInfoViewFactory : IDriveVolumeInfoViewFactory
    {
        public IDriveVolumeInfoView GetNewView(Context context)
        {
            return new DriveVolumeInfoView(context);
        }
    }
}
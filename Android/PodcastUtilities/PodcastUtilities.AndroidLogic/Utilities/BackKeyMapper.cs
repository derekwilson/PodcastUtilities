using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public class BackKeyMapper
    {
        public static bool HandleKeyEvent(AppCompatActivity activity, KeyEvent e)
        {
            if (e != null && e.Action == KeyEventActions.Up && e.KeyCode == Keycode.Escape)
            {
                activity.OnBackPressedDispatcher.OnBackPressed();
                return true;
            }
            return false;
        }
    }
}
using Android.Views;
using AndroidX.AppCompat.App;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public class BackKeyMapper
    {
        public static bool HandleKeyEvent(AppCompatActivity activity, KeyEvent? e)
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
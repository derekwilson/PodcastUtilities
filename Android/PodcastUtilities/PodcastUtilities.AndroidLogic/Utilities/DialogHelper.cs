using Android.App;
using Android.Views.InputMethods;
using Android.Views;
using Android.Content;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public class DialogHelper
    {
        public static void SetWidth(Dialog dialog, int widthPx)
        {
            if (dialog == null)
            {
                return;
            }
            var layoutParams = dialog.Window.Attributes;
            layoutParams.Width = widthPx;
            dialog.Window.Attributes = layoutParams;
        }

        public static void ShowSoftKeyboard(Context context, View view)
        {
            InputMethodManager inputMethodManager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
            view.RequestFocus();
            inputMethodManager.ShowSoftInput(view, 0);
            //inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }

        public static void HideSoftKeyboard(Context context, View view)
        {
            InputMethodManager inputMethodManager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(view.WindowToken, HideSoftInputFlags.None);
        }
    }
}
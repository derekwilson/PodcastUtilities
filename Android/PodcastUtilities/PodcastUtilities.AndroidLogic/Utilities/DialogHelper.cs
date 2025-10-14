using Android.App;
using Android.Views.InputMethods;
using Android.Content;
using Java.Lang;
using System;
using Android.Widget;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public class DialogHelper
    {
        /// <summary>
        /// set the width of a dialog as a percentage of the screen
        /// </summary>
        /// <param name="dialog">dialog to set</param>
        /// <param name="displayMetrics">screen metrics</param>
        /// <param name="percent">percentage to use for the dialog</param>
        public static void SetWidthByPercent(Dialog? dialog, Android.Util.DisplayMetrics? displayMetrics, int percent)
        {
            if (displayMetrics == null)
            {
                return;
            }
            DialogHelper.SetWidth(dialog, (displayMetrics.WidthPixels * 90) / 100);
        }

        /// <summary>
        /// set the width of a dialog
        /// </summary>
        /// <param name="dialog">dialog to set</param>
        /// <param name="widthPx">width in pixels, maybe work it out from the screen dimentions</param>
        public static void SetWidth(Dialog? dialog, int widthPx)
        {
            if (dialog == null || dialog.Window == null || dialog.Window.Attributes == null)
            {
                return;
            }
            var layoutParams = dialog.Window.Attributes;
            layoutParams.Width = widthPx;
            dialog.Window.Attributes = layoutParams;
        }

        /// <summary>
        /// attempt to force the soft keyboard to appear
        /// </summary>
        /// <param name="view">the EditText to take focus</param>
        public static void ShowSoftKeyboard(EditText? view)
        {
            if (view == null) 
            { 
                return;
            }

            // Show soft keyboard automatically and request focus to field (view)
            // I'm not very proud of this but it does have the virtue of working
            // see https://stackoverflow.com/questions/13694995/android-softkeyboard-showsoftinput-vs-togglesoftinput
            // see https://stackoverflow.com/questions/41725817/hide-to-show-and-hide-keyboard-in-dialogfragment
            // Note: it does not work sometimes on some phones, but it is a lot better than nothing

            var inputMethodManager = view.Context?.GetSystemService(Context.InputMethodService) as InputMethodManager;
            if (inputMethodManager == null)
            {
                return;
            }

            Action action = () =>
            {
                view.RequestFocus();
                view.SetSelection(view?.Text?.Length ?? 0);
                inputMethodManager.ShowSoftInput(view, ShowFlags.Implicit);
            };
            Runnable showKeyboard = new Runnable(action);
            view.PostDelayed(showKeyboard, 100);
        }
    }
}
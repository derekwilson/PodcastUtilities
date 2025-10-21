using Android.OS;
using AndroidX.Fragment.App;
using Google.Android.Material.Dialog;
using System;

namespace PodcastUtilities.AndroidLogic.CustomViews
{
    public class OkCancelDialogFragment : DialogFragment
    {
        private static string TITLE_KEY = "title_key";
        private static string MESSAGE_KEY = "message_key";
        private static string OK_KEY = "ok_key";
        private static string CANCEL_KEY = "cancel_key";
        private static string CUSTOM_KEY = "custom_key";

        public static OkCancelDialogFragment NewInstance(string title, string message, string ok, string cancel, string data)
        {
            var dialog = new OkCancelDialogFragment();
            var args = new Bundle();
            args.PutString(TITLE_KEY, title);
            args.PutString(MESSAGE_KEY, message);
            args.PutString(OK_KEY, ok);
            args.PutString(CANCEL_KEY, cancel);
            args.PutString(CUSTOM_KEY, data);
            dialog.Arguments = args;
            return dialog;
        }

        public EventHandler<Tuple<string, string>> OkSelected;
        public EventHandler<Tuple<string, string>> CancelSelected;

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var title = Arguments.GetString(TITLE_KEY);
            var message = Arguments.GetString(MESSAGE_KEY);
            var ok = Arguments.GetString(OK_KEY);
            var cancel = Arguments.GetString(CANCEL_KEY);
            var data = Arguments.GetString(CUSTOM_KEY);

            var builder = new MaterialAlertDialogBuilder(Activity)
                .SetMessage(message);
            if (!string.IsNullOrEmpty(title))
            {
                builder.SetTitle(title);
            }
            if (!string.IsNullOrEmpty(ok))
            {
                builder.SetPositiveButton("OK", (s, e) => OkSelected?.Invoke(this, Tuple.Create(Tag, data)));
            }
            if (!string.IsNullOrEmpty(cancel))
            {
                builder.SetNegativeButton(cancel, (sender, args) => CancelSelected?.Invoke(this, Tuple.Create(Tag, data)));
            }
            return builder.Create();
        }
    }
}
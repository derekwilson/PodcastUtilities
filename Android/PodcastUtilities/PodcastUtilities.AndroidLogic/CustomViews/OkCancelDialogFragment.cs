
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using System;

namespace PodcastUtilities.AndroidLogic.CustomViews
{
    public class OkCancelDialogFragment : DialogFragment
    {
        private static string MESSAGE_KEY = "message_key";
        private static string OK_KEY = "ok_key";
        private static string CANCEL_KEY = "cancel_key";
        private static string CUSTOM_KEY = "custom_key";

        public static OkCancelDialogFragment NewInstance(string message, string ok, string cancel, string data)
        {
            var dialog = new OkCancelDialogFragment();
            var args = new Bundle();
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
            var message = Arguments.GetString(MESSAGE_KEY);
            var ok = Arguments.GetString(OK_KEY);
            var cancel = Arguments.GetString(CANCEL_KEY);
            var data = Arguments.GetString(CUSTOM_KEY);

            return new AlertDialog.Builder(Activity)
                .SetMessage(message)
                .SetPositiveButton(ok, (sender, args) => OkSelected?.Invoke(this, Tuple.Create(Tag, data)))
                .SetNegativeButton(cancel, (sender, args) => CancelSelected?.Invoke(this, Tuple.Create(Tag, data)))
                .Create();
        }
    }
}
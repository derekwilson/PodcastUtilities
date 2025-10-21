using Android.OS;
using AndroidX.Fragment.App;
using Google.Android.Material.Dialog;
using System;

namespace PodcastUtilities.AndroidLogic.CustomViews
{
    public class OkCancelDialogFragment : AndroidX.Fragment.App.DialogFragment
    {
        private static string TITLE_KEY = "title_key";
        private static string MESSAGE_KEY = "message_key";
        private static string OK_KEY = "ok_key";
        private static string CANCEL_KEY = "cancel_key";
        private static string CUSTOM_KEY = "custom_key";

        public static OkCancelDialogFragment NewInstance(string title, string message, string ok, string cancel, string? data)
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

        public EventHandler<Tuple<string?, string?>>? OkSelected;
        public EventHandler<Tuple<string?, string?>>? CancelSelected;

        public override Android.App.Dialog OnCreateDialog(Bundle? savedInstanceState)
        {
            var args = RequireArguments();

            var title = args.GetString(TITLE_KEY);
            var message = args.GetString(MESSAGE_KEY);
            var ok = args.GetString(OK_KEY);
            var cancel = args.GetString(CANCEL_KEY);
            var data = args.GetString(CUSTOM_KEY);

            var activity = RequireActivity();
            var builder = new MaterialAlertDialogBuilder(activity)
                .SetMessage(message);
            if (!string.IsNullOrEmpty(title))
            {
                builder?.SetTitle(title);
            }
            if (!string.IsNullOrEmpty(ok))
            {
                builder?.SetPositiveButton("OK", (s, e) => OkSelected?.Invoke(this, Tuple.Create(Tag, data)));
            }
            if (!string.IsNullOrEmpty(cancel))
            {
                builder?.SetNegativeButton(cancel, (sender, args) => CancelSelected?.Invoke(this, Tuple.Create(Tag, data)));
            }
            return builder?.Create() ?? throw new InvalidOperationException("cannot build dialog");
        }
    }
}
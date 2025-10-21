using Android.Content.Res;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Google.Android.Material.TextField;
using PodcastUtilities.AndroidLogic.Utilities;
using System;

namespace PodcastUtilities.AndroidLogic.CustomViews
{
    public class ValuePromptDialogFragment : AndroidX.Fragment.App.DialogFragment
    {
        private static string TITLE_KEY = "title_key";
        private static string OK_KEY = "ok_key";
        private static string CANCEL_KEY = "cancel_key";
        private static string DATA_KEY = "data_key";
        private static string PROMPT_KEY = "prompt_key";
        private static string VALUE_KEY = "value_key";
        private static string NUMERIC_KEY = "numeric_key";

        public struct ValuePromptDialogFragmentParameters
        {
            public string Title;
            public string Ok;
            public string Cancel;
            public string Data;
            public string Prompt;
            public string Value;
            public bool IsNumeric;
        };

        public static ValuePromptDialogFragment NewInstance(ValuePromptDialogFragmentParameters parameters)
        {
            var dialog = new ValuePromptDialogFragment();
            var args = new Bundle();
            args.PutString(TITLE_KEY, parameters.Title);
            args.PutString(OK_KEY, parameters.Ok);
            args.PutString(CANCEL_KEY, parameters.Cancel);
            args.PutString(DATA_KEY, parameters.Data);
            args.PutString(PROMPT_KEY, parameters.Prompt);
            args.PutString(VALUE_KEY, parameters.Value);
            args.PutBoolean(NUMERIC_KEY, parameters.IsNumeric);
            dialog.Arguments = args;
            return dialog;
        }

        // tag, customData, value
        public EventHandler<Tuple<string?, string?, string>>? OkSelected;
        public EventHandler<Tuple<string?, string?, string>>? CancelSelected;

        private struct Controls
        {
            public TextView txtTitle;
            public TextInputLayout layValue;
            public EditText txtValue;
            public Button okButton;
            public Button cancelButton;
            public ImageView clearButton;
        }
        private Controls dialogControls;

        public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.dialog_fragment_value_prompt, container);
            dialogControls.txtTitle = ViewHelper.FindViewByIdOrThrow<TextView>("title", view, Resource.Id.value_prompt_title);
            dialogControls.layValue = ViewHelper.FindViewByIdOrThrow<TextInputLayout>("value layout", view, Resource.Id.value_prompt_value_layout);
            dialogControls.txtValue = ViewHelper.FindViewByIdOrThrow<EditText>("value text", view, Resource.Id.value_prompt_value);
            dialogControls.okButton = ViewHelper.FindViewByIdOrThrow<Button>("ok button", view, Resource.Id.value_prompt_ok);
            dialogControls.cancelButton = ViewHelper.FindViewByIdOrThrow<Button>("cancel button", view, Resource.Id.value_prompt_cancel);
            dialogControls.clearButton = ViewHelper.FindViewByIdOrThrow<ImageView>("clear button", view, Resource.Id.value_prompt_txt_clear);
            return view;
        }

        public override void OnViewCreated(View view, Bundle? savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            var args = RequireArguments();
            var title = args.GetString(TITLE_KEY);
            var ok = args.GetString(OK_KEY);
            var cancel = args.GetString(CANCEL_KEY);
            var customData = args.GetString(DATA_KEY);
            var prompt = args.GetString(PROMPT_KEY);
            var value = args.GetString(VALUE_KEY);
            var isNumeric = args.GetBoolean(NUMERIC_KEY);

            if (string.IsNullOrWhiteSpace(title)) {
                dialogControls.txtTitle.Visibility = ViewStates.Gone;
            } 
            else
            {
                dialogControls.txtTitle.Visibility = ViewStates.Visible;
                dialogControls.txtTitle.Text = title;
            }

            dialogControls.layValue.Hint = prompt;
            if (isNumeric)
            {
                dialogControls.txtValue.InputType = Android.Text.InputTypes.ClassNumber;
            }
            dialogControls.txtValue.EditorAction += (sender, args) => DoEditAction(Tag, customData, args);
            dialogControls.txtValue.Text = value;
            dialogControls.txtValue.SetSelection(dialogControls.txtValue?.Text?.Length ?? 0);

            dialogControls.clearButton.Click += (sender, e) => DoClear();

            dialogControls.okButton.Text = ok;
            dialogControls.okButton.Click += (sender, e) => DoOkAction(Tag, customData);

            // cancel is optional
            if (string.IsNullOrWhiteSpace(cancel))
            {
                dialogControls.cancelButton.Visibility = ViewStates.Gone;
            } else
            {
                dialogControls.cancelButton.Visibility = ViewStates.Visible;
                dialogControls.cancelButton.Text = cancel;
                dialogControls.cancelButton.Click += (sender, e) => DoCancelAction(Tag, customData);
            }

            DialogHelper.ShowSoftKeyboard(dialogControls.txtValue);
        }

        public override void OnResume()
        {
            // make the layout 90% of the width of the screen
            // because it looks good - and the OS default varies but is often very narrow
            DialogHelper.SetWidthByPercent(this.Dialog, Resources.DisplayMetrics, 90);

            base.OnResume();
        }

        private void DoClear()
        {
            dialogControls.txtValue.Text = "";
        }

        private void DoEditAction(string? tag, string? customData, TextView.EditorActionEventArgs args)
        {
            if (args.ActionId == ImeAction.Done)
            {
                DoOkAction(Tag, customData);
            }
        }

        private void DoCancelAction(string? tag, string? customData)
        {
            CancelSelected?.Invoke(this, Tuple.Create(tag, customData, dialogControls.txtValue.Text ?? ""));
            ExitDialog();
        }

        private void DoOkAction(string? tag, string? customData)
        {
            OkSelected?.Invoke(this, Tuple.Create(tag, customData, dialogControls.txtValue.Text ?? ""));
            ExitDialog();
        }

        private void ExitDialog()
        {
            Dismiss();
        }
    }
}
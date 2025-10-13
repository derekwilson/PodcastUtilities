using Android.Content.Res;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Google.Android.Material.TextField;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common.Configuration;
using System;

namespace PodcastUtilities.AndroidLogic.CustomViews
{
    public class DefaultableItemValuePromptDialogFragment : AndroidX.Fragment.App.DialogFragment
    {
        private static string TITLE_KEY = "title_key";
        private static string OK_KEY = "ok_key";
        private static string CANCEL_KEY = "cancel_key";
        private static string DATA_KEY = "data_key";
        private static string DEFAULT_PROMPT_KEY = "default_prompt_key";
        private static string NAMED_PROMPT_KEY = "named_prompt_key";
        private static string CUSTOM_PROMPT_KEY = "custom_prompt_key";
        private static string VALUE_TYPE_KEY = "value_type_key";
        private static string CURRENT_VALUE_KEY = "current_value_key";
        private static string DEFAULT_VALUE_KEY = "default_value_key";
        private static string NAMED_VALUE_KEY = "named_value_key";
        private static string NUMERIC_KEY = "numeric_key";

        public enum ItemValueType
        {
            Defaulted = 0,      // the value is defaulted from the global config
            Named = 1,          // the value is named, for example "Maximum"
            Custom = 2,         // custom value specified here
        }

        public struct DefaultableItemValuePromptDialogFragmentParameters
        {
            public string Title;
            public string Ok;
            public string Cancel;
            public string Data;
            public string DefaultPrompt;
            public string NamedPrompt;
            public string CustomPrompt;
            public ItemValueType ValueType;
            public string CurrentValue;
            public string DefaultValue;
            public string NamedValue;
            public bool IsNumeric;
        };

        public static ItemValueType GetIntItemType(IDefaultableItem<int> intValue, int namedValue)
        {
            if (intValue.IsSet)
            {
                if (intValue.Value == namedValue)
                {
                    return ItemValueType.Named;
                }
                else
                {
                    return ItemValueType.Custom;
                }
            }
            else
            {
                return ItemValueType.Defaulted;
            }
        }

        public static DefaultableItemValuePromptDialogFragment NewInstance(DefaultableItemValuePromptDialogFragmentParameters parameters)
        {
            var dialog = new DefaultableItemValuePromptDialogFragment();
            var args = new Bundle();
            args.PutString(TITLE_KEY, parameters.Title);
            args.PutString(OK_KEY, parameters.Ok);
            args.PutString(CANCEL_KEY, parameters.Cancel);
            args.PutString(DATA_KEY, parameters.Data);
            args.PutString(DEFAULT_PROMPT_KEY, parameters.DefaultPrompt);
            args.PutString(NAMED_PROMPT_KEY, parameters.NamedPrompt);
            args.PutString(CUSTOM_PROMPT_KEY, parameters.CustomPrompt);
            args.PutInt(VALUE_TYPE_KEY, ((int)parameters.ValueType));
            args.PutString(CURRENT_VALUE_KEY, parameters.CurrentValue);
            args.PutString(DEFAULT_VALUE_KEY, parameters.DefaultValue);
            args.PutString(NAMED_VALUE_KEY, parameters.NamedValue);
            args.PutBoolean(NUMERIC_KEY, parameters.IsNumeric);
            dialog.Arguments = args;
            return dialog;
        }

        // event callback: tag, customData, value
        public EventHandler<Tuple<string?, string?, string, ItemValueType>>? OkSelected;
        public EventHandler<Tuple<string?, string?>>? CancelSelected;

        // vars
        private ItemValueType valueType;

        private struct Controls
        {
            // controls
            public TextView txtTitle;
            public RadioGroup radioGroup;
            public RadioButton radioButtonDefault;
            public RadioButton radioButtonNamed;
            public RadioButton radioButtonCustom;
            // custom value
            public TextInputLayout layValue;
            public EditText txtValue;
            public ImageView clearButton;
            // ok / cancel
            public Button okButton;
            public Button cancelButton;
        }
        private Controls dialogControls;

        public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.dialog_fragment_defaultableitem_value_prompt, container);
            dialogControls.txtTitle = ViewHelper.FindViewByIdOrThrow<TextView>("title", view, Resource.Id.defaultableitem_value_prompt_title);
            dialogControls.radioGroup = ViewHelper.FindViewByIdOrThrow<RadioGroup>("radio group", view, Resource.Id.defaultableitem_value_prompt_radio_group);
            dialogControls.radioButtonDefault = ViewHelper.FindViewByIdOrThrow<RadioButton>("default button", view, Resource.Id.defaultableitem_value_prompt_radio_default);
            dialogControls.radioButtonNamed = ViewHelper.FindViewByIdOrThrow<RadioButton>("named button", view, Resource.Id.defaultableitem_value_prompt_radio_named);
            dialogControls.radioButtonCustom = ViewHelper.FindViewByIdOrThrow<RadioButton>("custom button", view, Resource.Id.defaultableitem_value_prompt_radio_custom);
            dialogControls.layValue = ViewHelper.FindViewByIdOrThrow<TextInputLayout>("value layout", view, Resource.Id.defaultableitem_value_prompt_value_layout);
            dialogControls.txtValue = ViewHelper.FindViewByIdOrThrow<EditText>("value text", view, Resource.Id.defaultableitem_value_prompt_value);
            dialogControls.clearButton = ViewHelper.FindViewByIdOrThrow<ImageView>("clear button", view, Resource.Id.defaultableitem_value_prompt_txt_clear);
            dialogControls.okButton = ViewHelper.FindViewByIdOrThrow<Button>("ok button", view, Resource.Id.defaultableitem_value_prompt_ok);
            dialogControls.cancelButton = ViewHelper.FindViewByIdOrThrow<Button>("cancel button", view, Resource.Id.defaultableitem_value_prompt_cancel);
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
            var defaultPrompt = args.GetString(DEFAULT_PROMPT_KEY);
            var namedPrompt = args.GetString(NAMED_PROMPT_KEY);
            var customPrompt = args.GetString(CUSTOM_PROMPT_KEY);
            valueType = (ItemValueType)args.GetInt(VALUE_TYPE_KEY);
            var currentValue = args.GetString(CURRENT_VALUE_KEY);
            var defaultValue = args.GetString(DEFAULT_VALUE_KEY);
            var namedValue = args.GetString(NAMED_VALUE_KEY);
            var isNumeric = args.GetBoolean(NUMERIC_KEY);

            // title is optional
            if (string.IsNullOrWhiteSpace(title)) {
                dialogControls.txtTitle.Visibility = ViewStates.Gone;
            } 
            else
            {
                dialogControls.txtTitle.Visibility = ViewStates.Visible;
                dialogControls.txtTitle.Text = title;
            }

            // default is optional
            if (string.IsNullOrWhiteSpace(defaultPrompt))
            {
                dialogControls.radioButtonDefault.Visibility = ViewStates.Gone;
            }
            else
            {
                dialogControls.radioButtonDefault.Visibility = ViewStates.Visible;
                dialogControls.radioButtonDefault.Text = defaultPrompt;
            }

            // named is optional
            if (string.IsNullOrWhiteSpace(namedPrompt))
            {
                dialogControls.radioButtonNamed.Visibility = ViewStates.Gone;
            }
            else
            {
                dialogControls.radioButtonNamed.Visibility = ViewStates.Visible;
                dialogControls.radioButtonNamed.Text = namedPrompt;
            }

            dialogControls.layValue.Hint = customPrompt;
            if (isNumeric)
            {
                dialogControls.txtValue.InputType = Android.Text.InputTypes.ClassNumber;
            }
            dialogControls.txtValue.EditorAction += (sender, args) => DoEditAction(Tag, customData, valueType, args);
            dialogControls.clearButton.Click += (sender, e) => DoClear();

            SetupControlsByType(valueType, currentValue);
            dialogControls.radioGroup.CheckedChange += (sender, e) => DoCheckChanged((View?) sender, e);

            dialogControls.okButton.Text = ok;
            dialogControls.okButton.Click += (sender, e) => DoOkAction(Tag, customData, valueType);

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

            if (valueType == ItemValueType.Custom)
            {
                DialogHelper.ShowSoftKeyboard(dialogControls.txtValue);
            }
        }

        private void DoCheckChanged(View? sender, RadioGroup.CheckedChangeEventArgs eventParams)
        {
            var button = sender?.FindViewById<RadioButton>(eventParams.CheckedId);
            if (button != null && button.Checked)
            {
                if (eventParams.CheckedId == Resource.Id.defaultableitem_value_prompt_radio_default)
                {
                    dialogControls.txtValue.Text = "";
                    dialogControls.txtValue.Enabled = false;
                    valueType = ItemValueType.Defaulted;
                }
                if (eventParams.CheckedId == Resource.Id.defaultableitem_value_prompt_radio_named)
                {
                    dialogControls.txtValue.Text = "";
                    dialogControls.txtValue.Enabled = false;
                    valueType = ItemValueType.Named;
                }
                if (eventParams.CheckedId == Resource.Id.defaultableitem_value_prompt_radio_custom)
                {
                    dialogControls.txtValue.Enabled = true;
                    DialogHelper.ShowSoftKeyboard(dialogControls.txtValue);
                    valueType = ItemValueType.Custom;
                }
            }
        }

        private void SetupControlsByType(ItemValueType valueType, string? currentValue)
        {
            switch (valueType)
            {
                case ItemValueType.Defaulted:
                    dialogControls.radioGroup.Check(Resource.Id.defaultableitem_value_prompt_radio_default);
                    dialogControls.txtValue.Text = "";
                    dialogControls.txtValue.Enabled = false;
                    break;
                case ItemValueType.Named:
                    dialogControls.radioGroup.Check(Resource.Id.defaultableitem_value_prompt_radio_named);
                    dialogControls.txtValue.Text = "";
                    dialogControls.txtValue.Enabled = false;
                    break;
                case ItemValueType.Custom:
                    dialogControls.radioGroup.Check(Resource.Id.defaultableitem_value_prompt_radio_custom);
                    dialogControls.txtValue.Enabled = true;
                    dialogControls.txtValue.Text = currentValue;
                    dialogControls.txtValue.SetSelection(dialogControls.txtValue?.Text?.Length ?? 0);
                    break;
            }
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

        private void DoEditAction(string? tag, string? customData, ItemValueType valueType, TextView.EditorActionEventArgs args)
        {
            if (args.ActionId == ImeAction.Done)
            {
                DoOkAction(Tag, customData, valueType);
            }
        }

        private void DoCancelAction(string? tag, string? customData)
        {
            CancelSelected?.Invoke(this, Tuple.Create(tag, customData));
            ExitDialog();
        }

        private void DoOkAction(string? tag, string? customData, ItemValueType valueType)
        {
            OkSelected?.Invoke(this, Tuple.Create(tag, customData, GetResult(), valueType));
            ExitDialog();
        }

        private string GetResult()
        {
            return dialogControls.txtValue.Text ?? "";
        }

        private void ExitDialog()
        {
            Dismiss();
        }
    }
}
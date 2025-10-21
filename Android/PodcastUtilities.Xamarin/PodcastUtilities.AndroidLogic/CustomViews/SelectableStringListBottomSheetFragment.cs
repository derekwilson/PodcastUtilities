using Android.OS;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomSheet;
using Java.Lang;
using System.Collections.Generic;
using System.Linq;

namespace PodcastUtilities.AndroidLogic.CustomViews
{
    public class SelectableStringListBottomSheetFragment : BottomSheetDialogFragment
    {
        private static string TITLE_KEY = "title_key";
        private static string DIVIDER_KEY = "divider_key";
        private static string OPTIONS_KEY = "options_key";

        public static SelectableStringListBottomSheetFragment NewInstance(bool divider, string title, List<string> options)
        {
            var parcelableOptions = options.Select((option, index) => new SelectableString(index, option)).ToArray();
            return NewInstance(divider, title, parcelableOptions);
        }

        public static SelectableStringListBottomSheetFragment NewInstance(bool divider, string title, List<SelectableString> options)
        {
            return NewInstance(divider, title, options.ToArray());
        }

        public static SelectableStringListBottomSheetFragment NewInstance(bool divider, string title, SelectableString[] options)
        {
            var fragment = new SelectableStringListBottomSheetFragment();
            var args = new Bundle();
            args.PutBoolean(DIVIDER_KEY, divider);
            args.PutString(TITLE_KEY, title);
            AddOptionsToArgs(args, options);
            fragment.Arguments = args;
            return fragment;
        }

        private static void AddOptionsToArgs(Bundle args, SelectableString[] parcelableOptions)
        {
            args.PutParcelableArray(OPTIONS_KEY, parcelableOptions);
        }

        private static List<SelectableString> GetOptionsFromArgs(Bundle args)
        {
            List<SelectableString> items = null;

            // We need to set the class loader:
            // https://developer.android.com/reference/android/os/Bundle#getParcelableArrayList(java.lang.String,%20java.lang.Class%3C?%20extends%20T%3E)
            var originalClassLoader = args.ClassLoader;
            args.ClassLoader = Java.Lang.Class.FromType(typeof(SelectableString)).ClassLoader;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                items = args.GetParcelableArray(OPTIONS_KEY, Java.Lang.Class.FromType(typeof(SelectableString))).Cast<SelectableString>().ToList();
            } else
            {
                // excellent work google - way to break an API - there really should be a compat version - but there isnt
#pragma warning disable CS0618 // Type or member is obsolete
                items = args.GetParcelableArray(OPTIONS_KEY).Cast<SelectableString>().ToList();
#pragma warning restore CS0618 // Type or member is obsolete
            }
            args.ClassLoader = originalClassLoader;
            return items;
        }

        public interface IListener
        {
            void BottomsheetItemSelected(string tag, int position, SelectableString item);
        }

        private TextView titleView;
        private ListView listView;

        private List<SelectableString> options;
        private IListener listener;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_bottomsheet_selectable_string_list, container, false);
            titleView = view.FindViewById<TextView>(Resource.Id.bottomsheet_slectable_string_list_title);
            listView = view.FindViewById<ListView>(Resource.Id.bottomsheet_slectable_string_list);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            listener = this.Activity as IListener;
            if (listener == null)
            {
                throw new ClassCastException(this.Activity.ToString() + " must implement SelectableStringListBottomSheetFragment.IListener");
            }

            var args = RequireArguments();

            titleView.Text = args.GetString(TITLE_KEY);
            options = GetOptionsFromArgs(args);

            if (!args.GetBoolean(DIVIDER_KEY, false))
            {
                listView.DividerHeight = 0;
            }

            var adapter = new SelectableStringArrayAdapter(RequireContext(), options);
            listView.Adapter = adapter;
            listView.ItemClick += ListView_ItemClick;
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            listener.BottomsheetItemSelected(this.Tag, e.Position, options[e.Position]);
            Dismiss();
        }

        public override void OnStart()
        {
            base.OnStart();
            // this forces the sheet to appear at max height even on landscape
            var behavior = BottomSheetBehavior.From(RequireView().Parent as View);
            behavior.State = BottomSheetBehavior.StateExpanded;
        }
    }
}

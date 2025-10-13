using Android.Content;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using PodcastUtilities.AndroidLogic.Utilities;

namespace PodcastUtilities.AndroidLogic.CustomViews
{
    public interface IDriveVolumeInfoView
    {
        View GetView();

        string Title { get; set; }

        void SetSpace(int usedMb, int totalMb, string free, string freeUnits, string total, string totalUnits);
    }

    public class DriveVolumeInfoView : LinearLayout, IDriveVolumeInfoView
    {
        private struct Controls
        {
            public TextView titleView;
            public TextView freeSpaceView;
            public TextView totalSpaceView;
            public ProgressBar steppedBarView;
        }
        private Controls viewControls;

        public DriveVolumeInfoView(Context context) : base(context)
        {
            Init(context, null, 0);
        }

        public DriveVolumeInfoView(Context context, IAttributeSet? attrs) : base(context, attrs)
        {
            Init(context, attrs, 0);
        }

        public DriveVolumeInfoView(Context context, IAttributeSet? attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init(context, attrs, defStyleAttr);
        }

        public string Title
        {
            get
            {
                return viewControls.titleView.Text ?? "";
            }
            set
            {
                viewControls.titleView.Text = value;
            }
        }

        public void SetSpace(int usedMb, int totalMb, string free, string freeUnits, string total, string totalUnits)
        {
            viewControls.steppedBarView.Max = totalMb;
            viewControls.steppedBarView.Progress = usedMb;
            SetValue(viewControls.freeSpaceView, free, freeUnits);
            SetValue(viewControls.totalSpaceView, total, totalUnits);
        }

        private void SetValue(TextView view, string value, string unit)
        {
            // I think this is still OK in RTL languages, ie. <value><unit>
            var valueLength = value.Length;
            var unitLength = unit.Length;
            var totalLength = valueLength + unitLength;

            var valueSpan = new SpannableString(value + unit);
            valueSpan.SetSpan(new RelativeSizeSpan(0.5F), valueLength, totalLength, 0);

            view.TextFormatted = valueSpan;
        }

        private void Init(Context context, IAttributeSet? attrs, int defStyle)
        {
            var view = InflateView(context);
            viewControls.titleView = ViewHelper.FindViewByIdOrThrow<TextView>("title", view, Resource.Id.drive_volume_info_title);
            viewControls.freeSpaceView = ViewHelper.FindViewByIdOrThrow<TextView>("free space", view, Resource.Id.drive_volume_info_free);
            viewControls.totalSpaceView = ViewHelper.FindViewByIdOrThrow<TextView>("total space", view, Resource.Id.drive_volume_info_total);
            viewControls.steppedBarView = ViewHelper.FindViewByIdOrThrow<ProgressBar>("stepped bar", view, Resource.Id.drive_volume_stepped_bar);

            LoadAttributes(attrs, defStyle);
        }

        private void LoadAttributes(IAttributeSet? attrs, int defStyle)
        {
            var a = Context?.ObtainStyledAttributes(attrs, Resource.Styleable.DriveVolumeInfoView, defStyle, 0);

            Title = a?.GetString(Resource.Styleable.DriveVolumeInfoView_title) ?? "NO TITLE";

            a?.Recycle();
        }

        private View? InflateView(Context context)
        {
            var inflater = context?.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
            return inflater?.Inflate(Resource.Layout.view_drive_volume_info, this, true);
        }

        public View GetView()
        {
            return this;
        }
    }
}
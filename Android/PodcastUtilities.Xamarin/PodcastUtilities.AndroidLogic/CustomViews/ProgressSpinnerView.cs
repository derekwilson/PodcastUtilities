using Android.Content;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace PodcastUtilities.AndroidLogic.CustomViews
{
    /// <summary>
    /// A progress bar with animations
    /// </summary>
    public class ProgressSpinnerView : LinearLayout
    {
        private TextView messageView;
        private ProgressBar indeterminateBar;
        private ProgressBar steppedBar;

        public ProgressSpinnerView(Context context) : base(context)
        {
            Init(context, null, 0);
        }

        public ProgressSpinnerView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(context, attrs, 0);
        }

        public ProgressSpinnerView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init(context, attrs, defStyleAttr);
        }

        public string Message
        {
            set
            {
                messageView.Text = value;
            }
        }

        public int Max
        {
            set
            {
                steppedBar.Max = value;
            }
        }

        public int Progress
        {
            set
            {
                steppedBar.Progress = value;
            }
        }

        private void Init(Context context, IAttributeSet attrs, int defStyle)
        {
            var view = InflateView(context);
            messageView = FindViewById<TextView>(Resource.Id.progress_bar_message);
            indeterminateBar = FindViewById<ProgressBar>(Resource.Id.indeterminateBar);
            steppedBar = FindViewById<ProgressBar>(Resource.Id.steppedBar);

            LoadAttributes(attrs, defStyle);
        }

        private void LoadAttributes(IAttributeSet attrs, int defStyle)
        {
            var a = Context.ObtainStyledAttributes(attrs, Resource.Styleable.ProgressSpinnerView, defStyle, 0);

            Message = a.GetString(Resource.Styleable.ProgressSpinnerView_message);

            a.Recycle();
        }

        private View InflateView(Context context)
        {
            LayoutInflater inflater = context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
            return inflater.Inflate(Resource.Layout.view_progress_spinner, this, true);
        }

        public void SlideDown(bool indeterminateProgress)
        {
            indeterminateBar.Visibility = indeterminateProgress ? ViewStates.Visible : ViewStates.Gone;
            steppedBar.Visibility = indeterminateProgress ? ViewStates.Gone : ViewStates.Visible;
            this.Visibility = ViewStates.Visible;
            var animate = new TranslateAnimation(
                0f,             // fromXDelta
                0f,             // toXDelta
                -this.Height,   // fromYDelta
                0f);            // toYDelta
            animate.Duration = 500;
            ClearAnimation();
            StartAnimation(animate);
        }

        public void SlideUp()
        {
            this.Visibility = ViewStates.Gone;
            var animate = new TranslateAnimation(
                0f,             // fromXDelta
                0f,             // toXDelta
                0f,             // fromYDelta
                -this.Height);  // toYDelta
            animate.Duration = 500;
            ClearAnimation();
            StartAnimation(animate);
        }

    }
}
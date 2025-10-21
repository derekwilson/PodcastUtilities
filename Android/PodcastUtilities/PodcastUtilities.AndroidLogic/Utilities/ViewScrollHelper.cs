using Android.OS;
using Android.Views;
using AndroidX.Core.Widget;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public class ViewScrollHelper
    {
        public static void ScrollToTop(NestedScrollView scrollView)
        {
            scrollView.FullScroll(((int)FocusSearchDirection.Up));
            if (Build.VERSION.SdkInt <= BuildVersionCodes.P)
            {
                // scroll to the top of the page
                // weird shit for old versions of OS
                // see https://stackoverflow.com/questions/31014409/programmatically-scroll-to-the-top-of-a-nestedscrollview
                scrollView.Parent?.RequestChildFocus(scrollView, scrollView);
            }
        }
    }
}
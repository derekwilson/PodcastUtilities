using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PodcastUtilitiesPOC.CustomViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilitiesPOC.Utilities
{
    class ProgressViewHelper
    {
        // specify a dynamic message to display in the progress view and use a stepped progress
        public static void StartProgress(ProgressSpinnerView progressBar, Window window , int messageId, Context context, int max)
        {
            progressBar.Message = context.GetString(messageId);
            progressBar.Max = max;
            StartProgress(progressBar, window, false);
        }

        // specify a dynamic message to display in the progress view
        public static void StartProgress(ProgressSpinnerView progressBar, Window window, int messageId, Context context)
        {
            progressBar.Message = context.GetString(messageId);
            StartProgress(progressBar, window);
        }

        // use the message specified in the layout XML and use a stepped progress
        public static void StartProgress(ProgressSpinnerView progressBar, Window window, int max)
        {
            progressBar.Max = max;
            StartProgress(progressBar, window, false);
        }

        // use the message specified in the layout XML
        public static void StartProgress(ProgressSpinnerView progressBar, Window window, bool indeterminateProgress = true)
        {
            progressBar.Progress = 0;
            progressBar.SlideDown(indeterminateProgress);

            // disable the UI
            window.SetFlags(WindowManagerFlags.NotTouchable, WindowManagerFlags.NotTouchable);
        }

        public static void CompleteProgress(ProgressSpinnerView progressBar, Window window)
        {
            progressBar.SlideUp();

            // enable the UI
            window.ClearFlags(WindowManagerFlags.NotTouchable);
        }
    }
}
using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Java.Util.Jar.Attributes;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    class ViewHelper
    {
        /// <summary>
        /// gets the android view or throws if it cannot be found, so the return is never null
        /// </summary>
        /// <typeparam name="T">the android view type</typeparam>
        /// <param name="displayName">name for use in any exception</param>
        /// <param name="view">the view to search</param>
        /// <param name="id">the id to look for</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">thrown if we cannot find the target view</exception>
        public static T FindViewByIdOrThrow<T>(string displayName, View? view, int id) where T : View
        {
            return view?.FindViewById<T>(id) ?? throw new ArgumentNullException($"{displayName}, {id.ToString()}", "cannot find ID in resources");
        }
    }
}

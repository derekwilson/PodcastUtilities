using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilitiesPOC.AndroidLogic.Utilities
{
    public interface IResourceProvider
    {
        string GetString(int id);
    };

    public class AndroidResourceProvider : IResourceProvider
    {
        private Context ResourceContext;

        public AndroidResourceProvider(Context context)
        {
            ResourceContext = context;
        }

        public string GetString(int id)
        {
            return ResourceContext.GetString(id);
        }
    }
}
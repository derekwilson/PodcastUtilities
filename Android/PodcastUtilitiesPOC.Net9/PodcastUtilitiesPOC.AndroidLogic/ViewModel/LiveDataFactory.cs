using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilitiesPOC.AndroidLogic.ViewModel
{
    public interface ILiveDataFactory
    {
        MutableLiveData CreateMutableLiveData();
    }

    public class LiveDataFactory : ILiveDataFactory
    {
        public MutableLiveData CreateMutableLiveData()
        {
            return new MutableLiveData();
        }
    }
}
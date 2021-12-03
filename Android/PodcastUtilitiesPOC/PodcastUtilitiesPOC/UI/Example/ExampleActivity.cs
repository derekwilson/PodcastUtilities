using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using PodcastUtilitiesPOC.AndroidLogic.ViewModel.Example;
using PodcastUtilitiesPOC.UI.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilitiesPOC.UI.Example
{
    [Activity(Label = "Example Activity", ParentActivity = typeof(MainActivity))]
    public class ExampleActivity : AppCompatActivity
    {
        private ExampleViewModel ViewModel;
        private AndroidApplication AndroidApplication;

        private class ExampleTitleObserver : Java.Lang.Object, IObserver
        {
            ExampleActivity Activity;

            public ExampleTitleObserver(ExampleActivity downloadActivity)
            {
                Activity = downloadActivity;
            }

            public void OnChanged(Java.Lang.Object o)
            {
                string value = (string)o;
                Activity.Title = value;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"ExampleActivity:OnCreate");

            base.OnCreate(savedInstanceState);

            // Set our view from the layout resource
            SetContentView(Resource.Layout.activity_example);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(ExampleViewModel))) as ExampleViewModel;
            Lifecycle.AddObserver(ViewModel);
            SetupLiveDataViewModelObservers();
            SetupViewModelObservers();

            ViewModel.Initialise();

            AndroidApplication.Logger.Debug(() => $"ExampleActivity:OnCreate - end");
        }

        protected override void OnStop()
        {
            base.OnStop();
            KillObservers();
            // the LiveData observers are automatically removed at this point because of the androidx lifecycle
        }

        private void SetupLiveDataViewModelObservers()
        {
            ViewModel.LiveDataObservables.Title.Observe(this, new ExampleTitleObserver(this));
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.Title += SetTitle;
        }

        private void KillObservers()
        {
            ViewModel.Observables.Title -= SetTitle;
        }

        private void SetTitle(object sender, string title)
        {
            Title = title;
        }
    }
}
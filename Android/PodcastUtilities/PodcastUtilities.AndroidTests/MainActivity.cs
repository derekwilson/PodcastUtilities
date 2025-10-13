using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content.PM;
using PodcastUtilities.Common.Platform;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Android.NUnitLite;

namespace PodcastUtilities.AndroidTests
{
    [Activity(Label = "UnitTests PodcastUtilities", MainLauncher = true)]
    public class MainActivity : TestSuiteActivity
    {
        public static Context MainContext;

        protected override void OnCreate(Bundle bundle)
        {
            // tests can be inside the main assembly
            this.AddTest(Assembly.GetExecutingAssembly());
            // or in any reference assemblies
            // AddTest (typeof (Your.Library.TestClass).Assembly);

            // Once you called base.OnCreate(), you cannot add more assemblies.
            base.OnCreate(bundle);

            MainContext = this;
            // add in our extra stuff
            View runButton = FindViewById<View>(Resource.Id.RunTestsButton);
            if (runButton != null)
            {
                var linearLayout = runButton.Parent;
                if (linearLayout is LinearLayout)
                {
                    (linearLayout as LinearLayout).AddView(GenerateDiagnosticInfo());
                }
            }
        }

        private TextView GenerateDiagnosticInfo()
        {
            TextView view = new TextView(this);
            List<string> environment = WindowsEnvironmentInformationProvider.GetEnvironmentRuntimeDisplayInformation();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(GetVersionDisplay());
            foreach (string line in environment)
            {
                builder.AppendLine(line);
            }
            view.Text = builder.ToString();

            view.SetSingleLine(false);
            return view;
        }

        private string GetVersionDisplay()
        {
            PackageInfo package = PackageManager.GetPackageInfo(PackageName, 0);
            long longVersionCode = PackageInfoCompat.GetLongVersionCode(package);
            return $"v{package.VersionName}, ({longVersionCode})";
        }
    }

}
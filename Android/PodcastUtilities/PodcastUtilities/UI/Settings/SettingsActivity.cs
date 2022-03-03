using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;

namespace PodcastUtilities.UI.Settings
{
    [Activity(Label = "@string/settings_activity", ParentActivity = typeof(MainActivity))]
    public class SettingsActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SupportFragmentManager.BeginTransaction()
                    .Replace(Android.Resource.Id.Content, new SettingsFragment())
                    .Commit();

            InitActionBar();
        }

        private void InitActionBar()
        {
            var actionBar = SupportActionBar;
            actionBar?.SetDisplayHomeAsUpEnabled(true);
        }

    }
}
using Android.App;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using PodcastUtilities.AndroidLogic.Utilities;

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

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (BackKeyMapper.HandleKeyEvent(this, e))
            {
                return true;
            }
            return base.DispatchKeyEvent(e);
        }

    }
}
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Share;

namespace PodcastUtilities.UI.Share
{
    [Activity(Label = "@string/share_episode_activity_title", ParentActivity = typeof(MainActivity))]
    internal class ShareEpisodeActivity : AppCompatActivity
    {
        private const string ACTIVITY_PARAM_ID = "ShareEpisodeActivity:Param:Id";

        public static Intent CreateIntent(Context context, string id)
        {
            Intent intent = new Intent(context, typeof(ShareEpisodeActivity));
            intent.PutExtra(ACTIVITY_PARAM_ID, id);
            return intent;
        }

        private AndroidApplication AndroidApplication = null!;
        private ShareEpisodeViewModel ViewModel = null!;

        // controls
        private TextView ErrorMessage = null!;
        private EmptyRecyclerView RvEpisodes = null!;
        private ShareEpisodeRecyclerItemAdapter Adapter = null;
        private LinearLayout NoDataView = null!;
        private TextView NoDataText = null!;
        private ProgressSpinnerView ProgressSpinner = null!;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            AndroidApplication = (AndroidApplication)Application!;
            AndroidApplication.Logger.Debug(() => $"ShareEpisodeActivity:OnCreate");

            base.OnCreate(savedInstanceState);

            // Set our view from the layout resource
            SetContentView(Resource.Layout.activity_share_episode);

            ErrorMessage = FindViewById<TextView>(Resource.Id.txtErrorMessage)!;
            RvEpisodes = FindViewById<EmptyRecyclerView>(Resource.Id.rvEpisodes)!;
            NoDataView = FindViewById<LinearLayout>(Resource.Id.layNoData)!;
            NoDataText = FindViewById<TextView>(Resource.Id.txtNoData)!;
            ProgressSpinner = FindViewById<ProgressSpinnerView>(Resource.Id.progressBar)!;

            RvEpisodes.SetLayoutManager(new LinearLayoutManager(this));
            RvEpisodes.AddItemDecoration(new DividerItemDecoration(this, DividerItemDecoration.Vertical));
            RvEpisodes.SetEmptyView(NoDataView);

            var factory = AndroidApplication.IocContainer?.Resolve<ViewModelFactory>() ?? throw new MissingMemberException("ViewModelFactory");
            ViewModel = (ShareEpisodeViewModel)new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(ShareEpisodeViewModel)));

            Adapter = new ShareEpisodeRecyclerItemAdapter(this, ViewModel, AndroidApplication.Logger);
            RvEpisodes.SetAdapter(Adapter);

            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            var id = Intent?.GetStringExtra(ACTIVITY_PARAM_ID);

            ViewModel.Initialise(id);
            Task.Run(() => ViewModel.FindItemsInFeed());
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"ShareEpisodeActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"ShareEpisodeActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            if (OperatingSystem.IsAndroidVersionAtLeast(23))
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        public override bool DispatchKeyEvent(KeyEvent? e)
        {
            if (BackKeyMapper.HandleKeyEvent(this, e))
            {
                AndroidApplication.Logger.Debug(() => $"ShareEpisodeActivity:DispatchKeyEvent - handled");
                return true;
            }
            return base.DispatchKeyEvent(e);
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.DisplayMessage += ToastMessage;
            ViewModel.Observables.StartProgress += StartProgress;
            ViewModel.Observables.EndProgress += EndProgress;
            ViewModel.Observables.SetItems += SetItems;
            ViewModel.Observables.SetEmptyText += SetEmptyText;
            ViewModel.Observables.DisplayChooser += DisplayChooser;
            ViewModel.Observables.DisplayErrorMessage += DisplayErrorMessage;
            ViewModel.Observables.HideErrorMessage += HideErrorMessage;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.Title -= SetTitle;
            ViewModel.Observables.DisplayMessage -= ToastMessage;
            ViewModel.Observables.StartProgress -= StartProgress;
            ViewModel.Observables.EndProgress -= EndProgress;
            ViewModel.Observables.SetItems -= SetItems;
            ViewModel.Observables.SetEmptyText -= SetEmptyText;
            ViewModel.Observables.DisplayChooser -= DisplayChooser;
            ViewModel.Observables.DisplayErrorMessage -= DisplayErrorMessage;
            ViewModel.Observables.HideErrorMessage -= HideErrorMessage;
        }

        private void DisplayChooser(object? sender, Tuple<string, Intent> args)
        {
            AndroidApplication.Logger.Debug(() => $"ShareEpisodeActivity: DisplayChooser");
            (string title, Intent intent) = args;
            StartActivity(Intent.CreateChooser(intent, title));
        }

        private void SetItems(object? sender, List<ShareEpisodeRecyclerItem> items)
        {
            RunOnUiThread(() =>
            {
                Adapter.SetItems(items);
                Adapter.NotifyDataSetChanged();
            });
        }

        private void SetEmptyText(object? sender, string message)
        {
            RunOnUiThread(() =>
            {
                NoDataText.Text = message;
            });
        }

        private void SetTitle(object? sender, string title)
        {
            RunOnUiThread(() =>
            {
                Title = title;
            });
        }

        private void ToastMessage(object? sender, string message)
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(Application.Context, message, ToastLength.Short)?.Show();
            });
        }

        private void StartProgress(object? sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                // indeterminate
                ProgressViewHelper.StartProgress(ProgressSpinner, Window);
            });
        }

        private void EndProgress(object? sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                ProgressViewHelper.CompleteProgress(ProgressSpinner, Window);
            });
        }

        private void DisplayErrorMessage(object? sender, string? message)
        {
            RunOnUiThread(() =>
            {
                if (!String.IsNullOrEmpty(message))
                {
                    ErrorMessage.Text = message;
                }
                ErrorMessage.Visibility = ViewStates.Visible;
            });
        }

        private void HideErrorMessage(object? sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                ErrorMessage.Visibility = ViewStates.Gone;
            });
        }

    }
}

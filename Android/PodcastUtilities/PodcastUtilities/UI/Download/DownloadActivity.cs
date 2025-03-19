using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.FloatingActionButton;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Download;
using PodcastUtilities.Common.Feeds;
using PodcastUtilities.UI.Messages;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.UI.Download
{
    // Title is set dynamically
    [Activity(ParentActivity = typeof(MainActivity), LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class DownloadActivity : AppCompatActivity
    {
        private const string ACTIVITY_PARAM_FOLDER = "DownloadActivity:Param:Folder";
        private const string ACTIVITY_PARAM_TEST = "DownloadActivity:Param:Test";
        private const string ACTIVITY_PARAM_HUD = "DownloadActivity:Param:Hud";

        public static Intent CreateIntent(Context context, string folder)
        {
            Intent intent = new Intent(context, typeof(DownloadActivity));
            if (!string.IsNullOrEmpty(folder))
            {
                intent.PutExtra(ACTIVITY_PARAM_FOLDER, folder);
            }
            return intent;
        }

        public static Intent CreateIntentToTestFeed(Context context, string folder)
        {
            Intent intent = new Intent(context, typeof(DownloadActivity));
            if (string.IsNullOrEmpty(folder))
            {
                return null;
            }
            intent.PutExtra(ACTIVITY_PARAM_FOLDER, folder);
            intent.PutExtra(ACTIVITY_PARAM_TEST, true);
            return intent;
        }
        public static Intent CreateIntentFromHud(Context context)
        {
            Intent intent = new Intent(context, typeof(DownloadActivity));
            intent.PutExtra(ACTIVITY_PARAM_HUD, true);
            return intent;
        }

        private const string KILL_PROMPT_TAG = "kill_prompt_tag";
        private const string NETWORK_PROMPT_TAG = "network_prompt_tag";

        private DownloadViewModel ViewModel;

        private AndroidApplication AndroidApplication;

        private TextView ErrorMessage;
        private EmptyRecyclerView RvDownloads;
        private LinearLayout NoDataView;
        private TextView NoDataText;
        private ProgressSpinnerView ProgressSpinner;
        private FloatingActionButton DownloadButton;
        private FloatingActionButton CancelButton;
        private DownloadRecyclerItemAdapter Adapter;
        private OkCancelDialogFragment KillPromptDialogFragment;
        private OkCancelDialogFragment NetworkPromptDialogFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => $"DownloadActivity:OnCreate");

            base.OnCreate(savedInstanceState);

            // Set our view from the layout resource
            SetContentView(Resource.Layout.activity_download);

            ErrorMessage = FindViewById<TextView>(Resource.Id.txtErrorMessage);
            RvDownloads = FindViewById<EmptyRecyclerView>(Resource.Id.rvDownloads);
            NoDataView = FindViewById<LinearLayout>(Resource.Id.layNoData);
            NoDataText = FindViewById<TextView>(Resource.Id.txtNoData);
            ProgressSpinner = FindViewById<ProgressSpinnerView>(Resource.Id.progressBar);
            DownloadButton = FindViewById<FloatingActionButton>(Resource.Id.fab_download);
            CancelButton = FindViewById<FloatingActionButton>(Resource.Id.fab_cancel);

            RvDownloads.SetLayoutManager(new LinearLayoutManager(this));
            RvDownloads.AddItemDecoration(new DividerItemDecoration(this, DividerItemDecoration.Vertical));
            RvDownloads.SetEmptyView(NoDataView);

            var factory = AndroidApplication.IocContainer.Resolve<ViewModelFactory>();
            ViewModel = new ViewModelProvider(this, factory).Get(Java.Lang.Class.FromType(typeof(DownloadViewModel))) as DownloadViewModel;

            Adapter = new DownloadRecyclerItemAdapter(this, ViewModel);
            RvDownloads.SetAdapter(Adapter);

            Lifecycle.AddObserver(ViewModel);
            SetupViewModelObservers();

            var folder = Intent?.GetStringExtra(ACTIVITY_PARAM_FOLDER);
            bool test = Intent?.GetBooleanExtra(ACTIVITY_PARAM_TEST, false) ?? false;
            bool fromHud = Intent?.GetBooleanExtra(ACTIVITY_PARAM_HUD, false) ?? false;
            ViewModel.Initialise(false, test, folder, fromHud);

            DownloadButton.Click += (sender, e) => ViewModel.DownloadAllPodcastsWithNetworkCheck();
            CancelButton.Click += (sender, e) => ViewModel.RequestKillDownloads();
            ErrorMessage.Click += (sender, e) => ViewModel.ActionSelected(Resource.Id.action_display_logs);

            KillPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(KILL_PROMPT_TAG) as OkCancelDialogFragment;
            SetupFragmentObservers(KillPromptDialogFragment);
            NetworkPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(NETWORK_PROMPT_TAG) as OkCancelDialogFragment;
            SetupFragmentObservers(NetworkPromptDialogFragment);

            AndroidApplication.Logger.Debug(() => $"DownloadActivity:OnCreate - end");
        }

        protected override void OnNewIntent(Intent intent)
        {
            // we are SingleTask launchmode so if a new activity is needed then this gets called
            AndroidApplication.Logger.Debug(() => $"DownloadActivity:OnNewIntent");
            base.OnNewIntent(intent);
            if (intent != null)
            {
                var folder = Intent?.GetStringExtra(ACTIVITY_PARAM_FOLDER);
                bool test = Intent?.GetBooleanExtra(ACTIVITY_PARAM_TEST, false) ?? false;
                bool fromHud = Intent?.GetBooleanExtra(ACTIVITY_PARAM_HUD, false) ?? false;
                ViewModel.Initialise(true, test, folder, fromHud);
            }
            AndroidApplication.Logger.Debug(() => $"DownloadActivity:OnNewIntent - end");
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"DownloadActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
            KillFragmentObservers(KillPromptDialogFragment);
            KillFragmentObservers(NetworkPromptDialogFragment);
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (BackKeyMapper.HandleKeyEvent(this, e))
            {
                AndroidApplication.Logger.Debug(() => $"DownloadActivity:DispatchKeyEvent - handled");
                return true;
            }
            return base.DispatchKeyEvent(e);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_download, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            EnableMenuItemIfAvailable(menu, Resource.Id.action_display_logs);
            return true;
        }

        private void EnableMenuItemIfAvailable(IMenu menu, int itemId)
        {
            menu.FindItem(itemId)?.SetEnabled(ViewModel.IsActionAvailable(itemId));
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (ViewModel.ActionSelected(item.ItemId))
            {
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void SetupViewModelObservers()
        {
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.TestMode += TestMode;
            ViewModel.Observables.StartProgress += StartProgress;
            ViewModel.Observables.UpdateProgress += UpdateProgress;
            ViewModel.Observables.EndProgress += EndProgress;
            ViewModel.Observables.SetSyncItems += SetSyncItems;
            ViewModel.Observables.UpdateItemProgress += UpdateItemProgress;
            ViewModel.Observables.UpdateItemStatus += UpdateItemStatus;
            ViewModel.Observables.DisplayMessage += ToastMessage;
            ViewModel.Observables.StartDownloading += StartDownloading;
            ViewModel.Observables.EndDownloading += EndDownloading;
            ViewModel.Observables.NavigateToDisplayLogs += NavigateToDisplayLogs;
            ViewModel.Observables.KillPrompt += KillPrompt;
            ViewModel.Observables.SetEmptyText += SetEmptyText;
            ViewModel.Observables.CellularPrompt += CellularPrompt;
            ViewModel.Observables.DisplayErrorMessage += DisplayErrorMessage;
            ViewModel.Observables.HideErrorMessage += HideErrorMessage;
        }

        private void KillViewModelObservers()
        {
            ViewModel.Observables.Title -= SetTitle;
            ViewModel.Observables.TestMode -= TestMode;
            ViewModel.Observables.StartProgress -= StartProgress;
            ViewModel.Observables.UpdateProgress -= UpdateProgress;
            ViewModel.Observables.EndProgress -= EndProgress;
            ViewModel.Observables.SetSyncItems -= SetSyncItems;
            ViewModel.Observables.UpdateItemProgress -= UpdateItemProgress;
            ViewModel.Observables.UpdateItemStatus += UpdateItemStatus;
            ViewModel.Observables.DisplayMessage -= ToastMessage;
            ViewModel.Observables.StartDownloading -= StartDownloading;
            ViewModel.Observables.EndDownloading -= EndDownloading;
            ViewModel.Observables.NavigateToDisplayLogs -= NavigateToDisplayLogs;
            ViewModel.Observables.KillPrompt -= KillPrompt;
            ViewModel.Observables.SetEmptyText -= SetEmptyText;
            ViewModel.Observables.CellularPrompt -= CellularPrompt;
            ViewModel.Observables.DisplayErrorMessage -= DisplayErrorMessage;
            ViewModel.Observables.HideErrorMessage -= HideErrorMessage;
        }

        private void SetupFragmentObservers(OkCancelDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"DownloadActivity: SetupFragmentObservers - {fragment.Tag}");
                fragment.OkSelected += OkSelected;
                fragment.CancelSelected += CancelSelected;
            }
        }

        private void KillFragmentObservers(OkCancelDialogFragment fragment)
        {
            if (fragment != null)
            {
                AndroidApplication.Logger.Debug(() => $"DownloadActivity: KillFragmentObservers - {fragment.Tag}");
                fragment.OkSelected -= OkSelected;
                fragment.CancelSelected -= CancelSelected;
            }
        }

        private void CancelSelected(object sender, Tuple<string, string> parameters)
        {
            (string tag, string data) = parameters;
            AndroidApplication.Logger.Debug(() => $"CancelSelected: {tag}");
            switch (tag)
            {
                case KILL_PROMPT_TAG:
                    KillFragmentObservers(KillPromptDialogFragment);
                    break;
                case NETWORK_PROMPT_TAG:
                    KillFragmentObservers(NetworkPromptDialogFragment);
                    break;
            }
        }

        private void OkSelected(object sender, Tuple<string, string> parameters)
        {
            (string tag, string data) = parameters;
            AndroidApplication.Logger.Debug(() => $"OkSelected: {tag}");
            RunOnUiThread(() =>
            {
                switch (tag)
                {
                    case KILL_PROMPT_TAG:
                        KillFragmentObservers(KillPromptDialogFragment);
                        ViewModel.CancelAllDownloads();
                        break;
                    case NETWORK_PROMPT_TAG:
                        KillFragmentObservers(NetworkPromptDialogFragment);
                        ViewModel.DownloadAllPodcastsWithoutNetworkCheck();
                        break;
                }
            });
        }

        private void DisplayErrorMessage(object sender, string message)
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

        private void HideErrorMessage(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                ErrorMessage.Visibility = ViewStates.Gone;
            });
        }

        private void SetSyncItems(object sender, List<DownloadRecyclerItem> items)
        {
            RunOnUiThread(() =>
            {
                Adapter.SetItems(items);
                Adapter.NotifyDataSetChanged();
            });
        }

        private void EndProgress(object sender, EventArgs e)
        {
            AndroidApplication.Logger.Debug(() => $"DownloadActivity:EndProgress");
            RunOnUiThread(() =>
            {
                ProgressViewHelper.CompleteProgress(ProgressSpinner, Window);
                DownloadButton.Enabled = true;
            });
        }

        private void UpdateProgress(object sender, int position)
        {
            AndroidApplication.Logger.Debug(() => $"DownloadActivity:UpdateProgress {position}");
            RunOnUiThread(() =>
            {
                ProgressSpinner.Progress = position;
            });
        }

        private void StartProgress(object sender, int max)
        {
            AndroidApplication.Logger.Debug(() => $"DownloadActivity:StartProgress {max}");
            RunOnUiThread(() =>
            {
                ProgressViewHelper.StartProgress(ProgressSpinner, Window, max);
                DownloadButton.Enabled = false;
            });
        }
        private void TestMode(object sender, bool testMode)
        {
            RunOnUiThread(() =>
            {
                DownloadButton.Visibility = testMode ? ViewStates.Gone : ViewStates.Visible;
                DownloadButton.Enabled = false;
                CancelButton.Visibility = ViewStates.Gone;
            });
        }

        private void SetTitle(object sender, string title)
        {
            RunOnUiThread(() =>
            {
                Title = title;
            });
        }

        private void UpdateItemProgress(object sender, Tuple<ISyncItem, int> updateItem)
        {
            RunOnUiThread(() =>
            {
                (ISyncItem item, int progress) = updateItem;
                var position = Adapter.SetItemProgress(item.Id, progress);
                Adapter.NotifyItemChanged(position);
            });
        }

        private void UpdateItemStatus(object sender, Tuple<ISyncItem, Status, string> updateItem)
        {
            RunOnUiThread(() =>
            {
                (ISyncItem item, Status status, string message) = updateItem;
                var position = Adapter.SetItemStatus(item.Id, status, message);
                Adapter.NotifyItemChanged(position);
            });
        }

        private void ToastMessage(object sender, string message)
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
            });
        }

        private void StartDownloading(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                Adapter.SetReadOnly(true);
                DownloadButton.Enabled = false;
                DownloadButton.Visibility = ViewStates.Gone;
                CancelButton.Visibility = ViewStates.Visible;
            });
        }

        private void EndDownloading(object sender, string message)
        {
            RunOnUiThread(() =>
            {
                Adapter.SetReadOnly(false);
                DownloadButton.Enabled = true;
                DownloadButton.Visibility = ViewStates.Visible;
                CancelButton.Visibility = ViewStates.Gone;
                KillPromptDialogFragment?.Dismiss();
                Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
            });
        }

        private void KillPrompt(object sender, Tuple<string, string, string, string> parameters)
        {
            RunOnUiThread(() =>
            {
                (string title, string message, string ok, string cancel) = parameters;
                KillPromptDialogFragment = OkCancelDialogFragment.NewInstance(title, message, ok, cancel, null);
                SetupFragmentObservers(KillPromptDialogFragment);
                KillPromptDialogFragment.Show(SupportFragmentManager, KILL_PROMPT_TAG);
            });
        }

        private void CellularPrompt(object sender, Tuple<string, string, string, string> parameters)
        {
            RunOnUiThread(() =>
            {
                (string title, string message, string ok, string cancel) = parameters;
                NetworkPromptDialogFragment = OkCancelDialogFragment.NewInstance(title, message, ok, cancel, null);
                SetupFragmentObservers(NetworkPromptDialogFragment);
                NetworkPromptDialogFragment.Show(SupportFragmentManager, NETWORK_PROMPT_TAG);
            });
        }

        private void NavigateToDisplayLogs(object sender, EventArgs e)
        {
            AndroidApplication.Logger.Debug(() => $"DownloadActivity: NavigateToDisplayLogs");
            RunOnUiThread(() =>
            {
                var intent = new Intent(this, typeof(MessagesActivity));
                StartActivity(intent);
            });
        }

        private void SetEmptyText(object sender, string message)
        {
            RunOnUiThread(() =>
            {
                NoDataText.Text = message;
            });
        }

    }
}
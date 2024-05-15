using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Activity;
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
using System.Threading.Tasks;

namespace PodcastUtilities.UI.Download
{
    // Title is set dynamically
    [Activity(ParentActivity = typeof(MainActivity))]
    public class DownloadActivity : AppCompatActivity
    {
        private const string ACTIVITY_PARAM_FOLDER = "DownloadActivity:Param:Folder";
        private const string ACTIVITY_PARAM_TEST = "DownloadActivity:Param:Test";

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

        private const string EXIT_PROMPT_TAG = "exit_prompt_tag";
        private const string NETWORK_PROMPT_TAG = "network_prompt_tag";

        private DownloadViewModel ViewModel;

        private AndroidApplication AndroidApplication;

        private TextView ErrorMessage;
        private EmptyRecyclerView RvDownloads;
        private LinearLayout NoDataView;
        private TextView NoDataText;
        private ProgressSpinnerView ProgressSpinner;
        private FloatingActionButton DownloadButton;
        private DownloadRecyclerItemAdapter Adapter;
        private OkCancelDialogFragment ExitPromptDialogFragment;
        private OkCancelDialogFragment NetworkPromptDialogFragment;

        private DownloadBackPressedCallback BackCallback;

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
            ViewModel.Initialise(test);
            Task.Run(() => ViewModel.FindEpisodesToDownload(folder));

            DownloadButton.Click += (sender, e) => ViewModel.DownloadAllPodcastsWithNetworkCheck();
            ErrorMessage.Click += (sender, e) => ViewModel.ActionSelected(Resource.Id.action_display_logs);

            ExitPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(EXIT_PROMPT_TAG) as OkCancelDialogFragment;
            SetupFragmentObservers(ExitPromptDialogFragment);
            NetworkPromptDialogFragment = SupportFragmentManager.FindFragmentByTag(NETWORK_PROMPT_TAG) as OkCancelDialogFragment;
            SetupFragmentObservers(NetworkPromptDialogFragment);

            BackCallback = new DownloadBackPressedCallback(this, ViewModel, AndroidApplication);
            this.OnBackPressedDispatcher.AddCallback(BackCallback);

            AndroidApplication.Logger.Debug(() => $"DownloadActivity:OnCreate - end");
        }

        private class DownloadBackPressedCallback : OnBackPressedCallback
        {
            private DownloadActivity Activity;
            private DownloadViewModel ViewModel;
            private AndroidApplication AndroidApplication;

            internal DownloadBackPressedCallback(DownloadActivity activity, DownloadViewModel model, AndroidApplication app) : base(true)
            {
                Activity = activity;
                ViewModel = model;
                AndroidApplication = app;
            }

            public override void HandleOnBackPressed()
            {
                if (ViewModel.RequestExit())
                {
                    AndroidApplication.Logger.Debug(() => $"DownloadBackPressedCallback:HandleOnBackPressed - exit");
                    Activity.Finish();
                    return;
                }
                AndroidApplication.Logger.Debug(() => $"DownloadBackPressedCallback:HandleOnBackPressed - exit not allowed");
            }
        }

        protected override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => $"DownloadActivity:OnDestroy");
            base.OnDestroy();
            KillViewModelObservers();
            KillFragmentObservers(ExitPromptDialogFragment);
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
            if (item.ItemId == Android.Resource.Id.Home)
            {
                BackCallback.HandleOnBackPressed();
                return true;
            }
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
            ViewModel.Observables.Exit += Exit;
            ViewModel.Observables.NavigateToDisplayLogs += NavigateToDisplayLogs;
            ViewModel.Observables.ExitPrompt += ExitPrompt;
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
            ViewModel.Observables.Exit -= Exit;
            ViewModel.Observables.NavigateToDisplayLogs -= NavigateToDisplayLogs;
            ViewModel.Observables.ExitPrompt -= ExitPrompt;
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
                case EXIT_PROMPT_TAG:
                    KillFragmentObservers(ExitPromptDialogFragment);
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
                    case EXIT_PROMPT_TAG:
                        KillFragmentObservers(ExitPromptDialogFragment);
                        ViewModel.CancelAllJobsAndExit();
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
            RunOnUiThread(() =>
            {
                ProgressViewHelper.CompleteProgress(ProgressSpinner, Window);
                DownloadButton.Enabled = true;
            });
        }

        private void UpdateProgress(object sender, int position)
        {
            RunOnUiThread(() =>
            {
                ProgressSpinner.Progress = position;
            });
        }

        private void StartProgress(object sender, int max)
        {
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
            });
        }

        private void EndDownloading(object sender, string message)
        {
            RunOnUiThread(() =>
            {
                Adapter.SetReadOnly(false);
                DownloadButton.Enabled = true;
                ExitPromptDialogFragment?.Dismiss();
                Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
            });
        }

        private void Exit(object sender, EventArgs e)
        {
            AndroidApplication.Logger.Debug(() => $"DownloadActivity: Exit");
            RunOnUiThread(() =>
            {
                Finish();
            });
        }

        private void ExitPrompt(object sender, Tuple<string, string, string, string> parameters)
        {
            RunOnUiThread(() =>
            {
                (string title, string message, string ok, string cancel) = parameters;
                ExitPromptDialogFragment = OkCancelDialogFragment.NewInstance(title, message, ok, cancel, null);
                SetupFragmentObservers(ExitPromptDialogFragment);
                ExitPromptDialogFragment.Show(SupportFragmentManager, EXIT_PROMPT_TAG);
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
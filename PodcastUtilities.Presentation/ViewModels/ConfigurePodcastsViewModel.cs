using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PodcastUtilities.Common;
using PodcastUtilities.Presentation.Services;

namespace PodcastUtilities.Presentation.ViewModels
{
    public class ConfigurePodcastsViewModel
        : ViewModel
    {
    	private readonly IApplicationService _applicationService;
    	private readonly IBrowseForFileService _browseForFileService;
    	private readonly IDialogService _dialogService;
    	private readonly IControlFileFactory _controlFileFactory;
    	private IControlFile _controlFile;
		private PodcastViewModel _selectedPodcast;
        private DelegateCommand _editPodcastCommand;

        private ObservableCollection<PodcastViewModel> _podcasts;

        public ConfigurePodcastsViewModel(
			IApplicationService applicationService,
			IBrowseForFileService browseForFileService,
			IDialogService dialogService,
			IControlFileFactory controlFileFactory)
        {
        	_applicationService = applicationService;
        	_browseForFileService = browseForFileService;
        	_dialogService = dialogService;
        	_controlFileFactory = controlFileFactory;

			OpenFileCommand = new DelegateCommand(ExecuteOpenFileCommand, CanExecuteOpenFileCommand);
			ExitCommand = new DelegateCommand(ExecuteExitCommand);
            _editPodcastCommand = new DelegateCommand(ExecuteEditPodcastCommand, CanExecuteEditPodcastCommand);

			_podcasts = new ObservableCollection<PodcastViewModel>();
        }

    	public ICommand OpenFileCommand { get; private set; }

    	public ICommand ExitCommand { get; private set; }

        public ICommand EditPodcastCommand
        {
            get { return _editPodcastCommand; }
        }

        public ObservableCollection<PodcastViewModel> Podcasts
        {
            get { return _podcasts; }
            private set { SetProperty(ref _podcasts, value, "Podcasts"); }
        }

    	public PodcastViewModel SelectedPodcast
    	{
    		get { return _selectedPodcast; }
    		set
    		{
    		    SetProperty(ref _selectedPodcast, value, "SelectedPodcast");
                _editPodcastCommand.RaiseCanExecuteChanged(this);
    		}
    	}

    	#region Command handling

		private void ExecuteOpenFileCommand(object parameter)
		{
			var selectedFile = _browseForFileService.BrowseForFileToOpen("Control Files|*.xml");

			if (selectedFile != null)
			{
				_controlFile = _controlFileFactory.OpenControlFile(selectedFile);

				Podcasts = new ObservableCollection<PodcastViewModel>(
					_controlFile.Podcasts.Select(p => new PodcastViewModel(p)));
			}
		}

		private static bool CanExecuteOpenFileCommand(object parameter)
		{
			return true;
		}

		private void ExecuteExitCommand(object parameter)
		{
			_applicationService.ShutdownApplication();
		}

		private void ExecuteEditPodcastCommand(object parameter)
		{
		    SelectedPodcast.StartEditing();

			if (_dialogService.ShowEditPodcastDialog(SelectedPodcast))
			{
			    SelectedPodcast.AcceptEdit();
			}
            else
			{
			    SelectedPodcast.CancelEdit();
			}
		}

        private bool CanExecuteEditPodcastCommand(object parameter)
        {
            return (SelectedPodcast != null);
        }

		#endregion
	}
}
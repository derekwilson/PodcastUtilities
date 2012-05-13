using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Presentation.Services;

namespace PodcastUtilities.Presentation.ViewModels
{
    public class ConfigurePodcastsViewModel
        : ViewModel
    {
        private const string TextDataFormat = "Text";

    	private readonly IApplicationService _applicationService;
    	private readonly IBrowseForFileService _browseForFileService;
    	private readonly IDialogService _dialogService;
    	private readonly IControlFileFactory _controlFileFactory;
        private readonly IPodcastFactory _podcastFactory;
        private readonly IClipboardService _clipboardService;
        private IReadWriteControlFile _controlFile;
		private PodcastViewModel _selectedPodcast;
        private readonly DelegateCommand _editPodcastCommand;

        private ObservableCollection<PodcastViewModel> _podcasts;

        public ConfigurePodcastsViewModel(
			IApplicationService applicationService,
			IBrowseForFileService browseForFileService,
			IDialogService dialogService,
			IControlFileFactory controlFileFactory,
            IPodcastFactory podcastFactory,
            IClipboardService clipboardService)
        {
        	_applicationService = applicationService;
        	_browseForFileService = browseForFileService;
        	_dialogService = dialogService;
        	_controlFileFactory = controlFileFactory;
            _podcastFactory = podcastFactory;
            _clipboardService = clipboardService;

            OpenFileCommand = new DelegateCommand(ExecuteOpenFileCommand, CanExecuteOpenFileCommand);
            SaveFileCommand = new DelegateCommand(ExecuteSaveFileCommand, CanExecuteSaveFileCommand);
			ExitCommand = new DelegateCommand(ExecuteExitCommand);
            AddPodcastCommand = new DelegateCommand(ExecuteAddPodcastCommand, CanExecuteAddPodcastCommand);
            _editPodcastCommand = new DelegateCommand(ExecuteEditPodcastCommand, CanExecuteEditPodcastCommand);

			_podcasts = new ObservableCollection<PodcastViewModel>();
        }

        public ICommand OpenFileCommand { get; private set; }

        public ICommand SaveFileCommand { get; private set; }

    	public ICommand ExitCommand { get; private set; }

        public ICommand EditPodcastCommand
        {
            get { return _editPodcastCommand; }
        }

        public ICommand AddPodcastCommand { get; private set; }

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
					_controlFile.GetPodcasts().Select(p => new PodcastViewModel(p)));
			}
		}

		private static bool CanExecuteOpenFileCommand(object parameter)
		{
			return true;
		}

        private bool CanExecuteSaveFileCommand(object obj)
        {
            return true;
        }

        private void ExecuteSaveFileCommand(object parameter)
        {
            var selectedFile = _browseForFileService.BrowseForFileToSave("Control Files|*.xml");

            if (selectedFile != null)
            {
                _controlFile.SaveToFile(selectedFile);
            }
        }

		private void ExecuteExitCommand(object parameter)
		{
			_applicationService.ShutdownApplication();
		}

		private void ExecuteEditPodcastCommand(object parameter)
		{
            EditPodcast(SelectedPodcast);
		}

        private bool CanExecuteEditPodcastCommand(object parameter)
        {
            return (SelectedPodcast != null);
        }

        private void ExecuteAddPodcastCommand(object parameter)
        {
            var dataObject = parameter as IDataObject;

            var newPodcast = (dataObject != null)
                                 ? CreateNewPodcast((string) dataObject.GetData(TextDataFormat))
                                 : CreateNewPodcast(_clipboardService.GetText());

            var newPodcastViewModel = new PodcastViewModel(newPodcast);

            if (EditPodcast(newPodcastViewModel))
            {
                Podcasts.Add(newPodcastViewModel);
            }
        }

        private static bool CanExecuteAddPodcastCommand(object parameter)
        {
            var dataObject = parameter as IDataObject;
            if (dataObject == null)
            {
                return true;
            }
            return IsValidUri((string)dataObject.GetData(TextDataFormat));
        }

		#endregion

        private bool EditPodcast(PodcastViewModel podcast)
        {
            podcast.StartEditing();

            var acceptedEdit = _dialogService.ShowEditPodcastDialog(podcast);

            if (acceptedEdit)
            {
                podcast.AcceptEdit();
            }
            else
            {
                podcast.CancelEdit();
            }

            return acceptedEdit;
        }

        private IPodcastInfo CreateNewPodcast(string possiblePodcastAddress)
        {
            var newPodcast = _podcastFactory.CreatePodcast(_controlFile);

            if (IsValidUri(possiblePodcastAddress))
            {
                newPodcast.Feed.Address = new Uri(possiblePodcastAddress);
            }

            return newPodcast;
        }

        private static bool IsValidUri(string address)
        {
            return ((address != null) && Uri.IsWellFormedUriString(address, UriKind.Absolute));
        }
    }
}
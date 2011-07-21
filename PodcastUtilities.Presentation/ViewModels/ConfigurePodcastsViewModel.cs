using System.Collections.ObjectModel;
using System.Windows.Input;
using PodcastUtilities.Common;
using PodcastUtilities.Presentation.Services;

namespace PodcastUtilities.Presentation.ViewModels
{
    public class ConfigurePodcastsViewModel
        : ViewModel
    {
    	private readonly IControlFileFactory _controlFileFactory;
    	private readonly IBrowseForFileService _browseForFileService;
    	private IControlFile _controlFile;

        private ObservableCollection<PodcastInfo> _podcasts;

        public ConfigurePodcastsViewModel(
			IControlFileFactory controlFileFactory,
			IBrowseForFileService browseForFileService)
        {
        	_controlFileFactory = controlFileFactory;
        	_browseForFileService = browseForFileService;

			OpenFileCommand = new DelegateCommand(ExecuteOpenFileCommand, CanExecuteOpenFileCommand);

        	_podcasts = new ObservableCollection<PodcastInfo>();
        }

    	public ICommand OpenFileCommand { get; private set; }

    	public ObservableCollection<PodcastInfo> Podcasts
        {
            get { return _podcasts; }
            private set { SetProperty(ref _podcasts, value, "Podcasts"); }
        }

		private void ExecuteOpenFileCommand(object parameter)
		{
			var selectedFile = _browseForFileService.BrowseForFileToOpen("Control Files|*.xml");

			if (selectedFile != null)
			{
				_controlFile = _controlFileFactory.OpenControlFile(selectedFile);

				Podcasts = new ObservableCollection<PodcastInfo>(_controlFile.Podcasts);
			}
		}

		private static bool CanExecuteOpenFileCommand(object parameter)
		{
			return true;
		}
	}
}
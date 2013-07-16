#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PodcastUtilities.Common.Configuration;
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
        private readonly IPodcastFactory _podcastFactory;
        private readonly IClipboardService _clipboardService;
        private readonly IDataObjectUriExtractor _dataObjectUriExtractor;
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
            IClipboardService clipboardService,
            IDataObjectUriExtractor dataObjectUriExtractor)
        {
        	_applicationService = applicationService;
        	_browseForFileService = browseForFileService;
        	_dialogService = dialogService;
        	_controlFileFactory = controlFileFactory;
            _podcastFactory = podcastFactory;
            _clipboardService = clipboardService;
            _dataObjectUriExtractor = dataObjectUriExtractor;

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
                                 ? CreateNewPodcast(_dataObjectUriExtractor.GetUri(dataObject))
                                 : CreateNewPodcast(_clipboardService.GetText());

            var newPodcastViewModel = new PodcastViewModel(newPodcast);

            if (EditPodcast(newPodcastViewModel))
            {
                Podcasts.Add(newPodcastViewModel);
            }
        }

        private bool CanExecuteAddPodcastCommand(object parameter)
        {
            var dataObject = parameter as IDataObject;
            if (dataObject == null)
            {
                return true;
            }
            return _dataObjectUriExtractor.ContainsUri(dataObject);
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
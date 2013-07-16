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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Presentation.ViewModels
{
	public class EditPodcastViewModel
		: ViewModel
	{
		private bool? _dialogResult;

		private readonly DelegateCommand _acceptCommand;

		public EditPodcastViewModel(PodcastViewModel podcast)
		{
			Podcast = podcast;

			_dialogResult = null;

			_acceptCommand = new DelegateCommand(ExecuteAcceptCommand, CanExecuteAcceptCommand);

            NamingStyles = CollectionHelper.CreateForDefaultableEnum<PodcastEpisodeNamingStyle>();

            DownloadStrategies = CollectionHelper.CreateForDefaultableEnum<PodcastEpisodeDownloadStrategy>();

			Podcast.PropertyChanged += PodcastPropertyChanged;
		}

		public PodcastViewModel Podcast { get; private set; }

        public IEnumerable<DefaultableValueTypeItem<PodcastEpisodeNamingStyle>> NamingStyles { get; private set; }

        public IEnumerable<DefaultableValueTypeItem<PodcastEpisodeDownloadStrategy>> DownloadStrategies { get; private set; }

        public IDefaultableItem<PodcastEpisodeNamingStyle> SelectedNamingStyle
        {
            get
            {
                return NamingStyles.First(s => s.Equals(Podcast.Podcast.Feed.NamingStyle));
            }
            set
            {
                if (!Podcast.Podcast.Feed.NamingStyle.Equals(value))
                {
                    Podcast.Podcast.Feed.NamingStyle.Copy(value);
                    OnPropertyChanged("SelectedNamingStyle");
                }
            }
        }

        public IDefaultableItem<PodcastEpisodeDownloadStrategy> SelectedDownloadStrategy
        {
            get
            {
                return DownloadStrategies.First(s => s.Equals(Podcast.Podcast.Feed.DownloadStrategy));
            }
            set
            {
                if (!Podcast.Podcast.Feed.DownloadStrategy.Equals(value))
                {
                    Podcast.Podcast.Feed.DownloadStrategy.Copy(value);
                    OnPropertyChanged("SelectedDownloadStrategy");
                }
            }
        }

		public ICommand AcceptCommand { get { return _acceptCommand; } }

		public bool? DialogResult
		{
			get { return _dialogResult; }
			set { SetProperty(ref _dialogResult, value, "DialogResult"); }
		}

		private void ExecuteAcceptCommand(object parameter)
		{
			DialogResult = true;
		}

		private bool CanExecuteAcceptCommand(object obj)
		{
		    return Podcast.IsValid;
		}

		private void PodcastPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			_acceptCommand.RaiseCanExecuteChanged(this);
		}
	}
}
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
                    if (value.IsSet)
                    {
                        Podcast.Podcast.Feed.NamingStyle.Value = value.Value;
                    }
                    else
                    {
                        Podcast.Podcast.Feed.NamingStyle.RevertToDefault();
                    }
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
                    if (value.IsSet)
                    {
                        Podcast.Podcast.Feed.DownloadStrategy.Value = value.Value;
                    }
                    else
                    {
                        Podcast.Podcast.Feed.DownloadStrategy.RevertToDefault();
                    }
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
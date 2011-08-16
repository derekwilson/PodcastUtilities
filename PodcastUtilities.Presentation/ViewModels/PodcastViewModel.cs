using System;
using PodcastUtilities.Common;

namespace PodcastUtilities.Presentation.ViewModels
{
	public class PodcastViewModel
		: ViewModel
	{
		private readonly PodcastInfo _podcast;
		private PodcastInfo _backupPodcastInfo;

		public PodcastViewModel(PodcastInfo podcast)
		{
			_podcast = podcast;
		}

		public PodcastInfo Podcast
		{
			get { return _podcast; }
		}

		public string Name
		{
			get { return _podcast.Folder; }
			set
			{
				if (_podcast.Folder != value)
				{
					_podcast.Folder = value;
					OnPropertyChanged("Name");
				}
			}
		}

		public Uri Address
		{
			get { return _podcast.Feed.Address; }
			set
			{
				if (_podcast.Feed.Address != value)
				{
					_podcast.Feed.Address = value;
					OnPropertyChanged("Address");
				}
			}
		}

	    public virtual void StartEditing()
	    {
	        _backupPodcastInfo = new PodcastInfo
	                                   {
	                                       Folder = _podcast.Folder,
                                           Feed = new FeedInfo { Address = new Uri(_podcast.Feed.Address.AbsoluteUri )}
	                                   };
	    }

	    public virtual void AcceptEdit()
	    {
	        _backupPodcastInfo = null;
	    }

	    public virtual void CancelEdit()
	    {
	        Name = _backupPodcastInfo.Folder;
	        Address = _backupPodcastInfo.Feed.Address;

	        _backupPodcastInfo = null;
	    }
	}
}
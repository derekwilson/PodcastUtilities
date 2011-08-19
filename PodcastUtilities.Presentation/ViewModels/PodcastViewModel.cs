using System;
using System.ComponentModel;
using PodcastUtilities.Common;

namespace PodcastUtilities.Presentation.ViewModels
{
	public class PodcastViewModel
		: ViewModel, IDataErrorInfo
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

	    #region Implementation of IDataErrorInfo

	    public string this[string columnName]
	    {
	        get
	        {
	            if ((columnName == "Name") && String.IsNullOrEmpty(Name))
	            {
	                return "Please enter a name";
	            }
	            else if ((columnName == "Address") && ((Address == null) || !Address.IsAbsoluteUri))
	            {
	                return "Please enter a valid address for the podcast";
	            }
	            return null;
	        }
	    }

	    public string Error
	    {
	        get { throw new NotImplementedException(); }
	    }

	    #endregion
	}
}
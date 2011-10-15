using System;
using System.ComponentModel;
using System.Diagnostics;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Presentation.ViewModels
{
	public class PodcastViewModel
		: ViewModel, IDataErrorInfo
	{
		private readonly IPodcastInfo _podcast;
		private IPodcastInfo _backupPodcastInfo;

		public PodcastViewModel(IPodcastInfo podcast)
		{
			_podcast = podcast;
		}

		public IPodcastInfo Podcast
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

        public IDefaultableItem<PodcastEpisodeNamingStyle> NamingStyle
        {
            get { return _podcast.Feed.NamingStyle; }
            set
            {
                if (!_podcast.Feed.NamingStyle.Equals(value))
                {
                    if (value.IsSet)
                    {
                        _podcast.Feed.NamingStyle.Value = value.Value;
                    }
                    else
                    {
                        _podcast.Feed.NamingStyle.RevertToDefault();
                    }
                    OnPropertyChanged("NamingStyle");
                }
            }
        }

        public bool IsEditing
	    {
            get { return (_backupPodcastInfo != null); }
	    }

        public bool IsValid
        {
            get { return ((this["Name"] == null) && (this["Address"] == null)); }
        }

	    public virtual void StartEditing()
	    {
	        _backupPodcastInfo = _podcast.Clone() as PodcastInfo;
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
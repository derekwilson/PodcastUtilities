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
using System.ComponentModel;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Presentation.ViewModels
{
	public class PodcastViewModel
		: ViewModel, IDataErrorInfo
	{
		private IPodcastInfo _podcast;
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
	        _podcast = _backupPodcastInfo;

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
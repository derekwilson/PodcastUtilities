using System.Collections.Generic;
using System.ComponentModel;

namespace PodcastUtilities.Presentation.ViewModels
{
    public abstract class ViewModel
        : INotifyPropertyChanged
    {
        protected bool SetProperty<TProperty>(
            ref TProperty property,
            TProperty newValue,
            string propertyName)
        {
            if (EqualityComparer<TProperty>.Default.Equals(property, newValue))
            {
                return false;
            }

            property = newValue;

            OnPropertyChanged(propertyName);

            return true;
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
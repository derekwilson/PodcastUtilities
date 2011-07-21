using System;
using System.Windows.Input;

namespace PodcastUtilities.Presentation
{
    public class DelegateCommand
        : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public DelegateCommand(
            Action<object> execute,
            Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        #region Implementation of ICommand

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add {  }
            remove {  }
        }

        #endregion
    }
}
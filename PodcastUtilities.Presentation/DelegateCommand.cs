using System;
using System.Windows.Input;

namespace PodcastUtilities.Presentation
{
    public class DelegateCommand
        : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public DelegateCommand(Action<object> execute)
			:this(execute, null)
        {
        }

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
            return ((_canExecute != null) ? _canExecute(parameter) : true);
        }

    	public event EventHandler CanExecuteChanged;

		public void RaiseCanExecuteChanged(object sender)
		{
			if (CanExecuteChanged != null)
			{
				CanExecuteChanged(sender, EventArgs.Empty);
			}
		}

    	#endregion
    }
}
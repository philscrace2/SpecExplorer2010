using System;
using System.Windows.Input;

namespace Microsoft.SpecExplorer
{
	public class DelegateCommand : ICommand
	{
		public Func<bool> CanExecuteMethod { get; private set; }

		public Action<object> ExecutedMethod { get; private set; }

		public event EventHandler CanExecuteChanged;

		public DelegateCommand(Action<object> executedMethod)
		{
			ExecutedMethod = executedMethod;
		}

		public DelegateCommand(Action<object> executedMethod, Func<bool> canExecuteMethod)
			: this(executedMethod)
		{
			CanExecuteMethod = canExecuteMethod;
		}

		public void InvokeCanExecuteChangedEvent()
		{
			if (this.CanExecuteChanged != null)
			{
				this.CanExecuteChanged(this, null);
			}
		}

		public bool CanExecute(object parameter)
		{
			if (CanExecuteMethod == null)
			{
				return true;
			}
			return CanExecuteMethod();
		}

		public void Execute(object parameter)
		{
			if (ExecutedMethod != null)
			{
				ExecutedMethod(parameter);
			}
		}
	}
}

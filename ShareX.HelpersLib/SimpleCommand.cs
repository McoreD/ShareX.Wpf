using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HelpersLib
{
    public class SimpleCommand : ICommand
    {
        private readonly Action<object> executeDelegate;
        private readonly Predicate<object> canExecuteDelegate;

        public SimpleCommand(Action<object> execute) : this(execute, null)
        {
        }

        public SimpleCommand(Action<object> execute, Predicate<object> canExecute)
        {
            executeDelegate = execute;
            canExecuteDelegate = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (canExecuteDelegate != null)
            {
                return canExecuteDelegate(parameter);
            }

            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            if (executeDelegate != null)
            {
                executeDelegate(parameter);
            }
        }
    }
}
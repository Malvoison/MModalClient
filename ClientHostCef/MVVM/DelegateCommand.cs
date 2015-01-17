using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

namespace ClientHostCef.MVVM
{
    public class DelegateCommand : ICommand
    {
        private readonly Action commandHandler;
        private readonly Func<bool> canExecuteHandler;

        public DelegateCommand(Action commandHandler, Func<bool> canExecuteHandler = null)
        {
            this.commandHandler = commandHandler;
            this.canExecuteHandler = canExecuteHandler;
        }

        public void Execute(object parameter)
        {
            commandHandler();
        }

        public bool CanExecute(object parameter)
        {
            return
                canExecuteHandler == null ||
                canExecuteHandler();
        }

#pragma warning disable 0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067
    }
}

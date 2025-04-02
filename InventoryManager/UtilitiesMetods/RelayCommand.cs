using System.Windows.Input;

namespace InventoryManager.UtilitiesMetods
{
    public class RelayCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove {  CommandManager.RequerySuggested -= value;}
        }
        private Action<object> execute;
        private Func<object,bool> canExecute;
        public RelayCommand(Action<object> _execute,Func<object,bool> _canExecute = null)
        {
            execute = _execute;
            canExecute = _canExecute;
        }
        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute(parameter);
        }
        public void Execute(object? parameter)
        {
            execute(parameter);
        }
    }
}

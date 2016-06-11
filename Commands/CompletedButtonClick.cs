using BucketListUWP.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BucketListUWP.Commands
{
    public class CompletedButtonClick : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            App.DataModel.CompleteGoalToday((BucketItem)parameter);
        }
    }
}

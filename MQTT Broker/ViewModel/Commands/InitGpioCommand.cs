using MQTT_Broker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace MQTT_Broker.ViewModel.Commands
{
    public class InitGpioCommand : ICommand
    {
        public ViewModelBase ViewModel { get; set; }
        public InitGpioCommand(ViewModelBase vm) 
        {
            this.ViewModel = vm;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if(parameter != null) 
            { 
                var s = parameter as DigitalControl;
                if (s == null) return false;
                return true;
            }

            return false;
        }

        public void Execute(object parameter)
        {
           this.ViewModel.InitGPIO(parameter as DigitalControl);
        }
    }
}

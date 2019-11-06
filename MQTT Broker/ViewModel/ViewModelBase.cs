using MQTT_Broker.Models;
using MQTT_Broker.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MQTT_Broker.ViewModel
{
    public class ViewModelBase
    {
        public InitGpioCommand InitGpiocommand { get; set; }
        public ViewModelBase() 
        {
            this.InitGpiocommand = new InitGpioCommand(this);
        }

        public void InitGPIO(DigitalControl dc)
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                dc.Status = "There is no GPIO controller on this device.";
                //dc.Status.Foreground = new SolidColorBrush(Colors.Red);
            }

            dc.Status = "GPIO controller initialized correctly.";
            //dc.Status.Foreground = new SolidColorBrush(Colors.Green);

            dc.DevicePin = gpio.OpenPin(dc.PinNumber);
            dc.PinValue = GpioPinValue.Low;
            dc.DevicePin.Write(dc.PinValue);
            dc.DevicePin.SetDriveMode(GpioPinDriveMode.Output);
        }
    }
}

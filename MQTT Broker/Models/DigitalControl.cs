using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml.Controls;

namespace MQTT_Broker.Models
{
    public class DigitalControl
    {
        public string Status { get; set; }
        public GpioPin DevicePin  { get; set; }
        public GpioPinValue PinValue { get; set; }
        public int PinNumber { get; set; }
    }
}

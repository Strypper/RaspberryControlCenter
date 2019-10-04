using MQTTnet;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MQTT_Broker
{

    public class TempDemo
    {
        public double Temp { get; set; }
        public int Time { get; set; }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //fixed pin
        private const int LED_PIN = 17, RELAY_PIN = 27;
        private GpioPin LEDpin, RELAYpin;
        private GpioPinValue LEDpinValue, RELAYpinValue;
        private string PayLoad;
        private bool LightDigital;
        private bool RelayDigital;
        Random rdm = new Random();

        public MainPage()
        {
            this.InitializeComponent();
            MQTTBrokerInit();
            List<TempDemo> demo = new List<TempDemo>()
            {
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)},
                new TempDemo(){ Temp = rdm.Next(), Time = rdm.Next(0, 24)}
            };
            (TempChart.Series[0] as LineSeries).ItemsSource = demo;
        }




        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                LEDpin = null;
                //StatusDigitalLed.Text = "There is no GPIO controller on this device.";
                return;
            }

            LEDpin = gpio.OpenPin(LED_PIN);
            RELAYpin = gpio.OpenPin(RELAY_PIN);
            LEDpinValue = GpioPinValue.Low;
            RELAYpinValue = GpioPinValue.High;
            LEDpin.Write(LEDpinValue);
            RELAYpin.Write(RELAYpinValue);
            LEDpin.SetDriveMode(GpioPinDriveMode.Output);
            RELAYpin.SetDriveMode(GpioPinDriveMode.Output);

            //StatusDigitalLed.Text = "GPIO pin initialized correctly.";

        }

        public async Task MQTTBrokerInit()
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionBacklog(100)
                .WithDefaultEndpointPort(1884);

            var mqttServer = new MqttFactory().CreateMqttServer();
            try
            {
                await mqttServer.StartAsync(optionsBuilder.Build());
                System.Diagnostics.Debug.WriteLine("Success");
            }
            catch (MQTTnet.Exceptions.MqttCommunicationException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            await mqttServer.GetClientStatusAsync();
            MqttApplicationMessage message;
            await Task.Run(async () =>
             {
                 while (true)
                 {
                     message = new MqttApplicationMessageBuilder()
                                .WithTopic("hello/world")
                                .WithPayload("Hello")
                                .WithExactlyOnceQoS()
                                .WithRetainFlag(true)
                                .Build();
                         await mqttServer.PublishAsync(message);
                     await Task.Delay(5000); // Every 10 seconds
                 }
             });
        }

        private void ToggleFirstLightSection1_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
                if (toggleSwitch.IsOn == true)
                {
                    Test.Background = new SolidColorBrush(Color.FromArgb(250, 120, 255, 214));
                    FirstLightBubSec1.Foreground = new SolidColorBrush(Colors.Yellow);
                    LEDpinValue = GpioPinValue.High;
                    LEDpin.Write(LEDpinValue);
                    LightDigital = true;
                    PayLoad = "Turn On Light";
            }
                else
                {
                    Test.Background = new SolidColorBrush(Color.FromArgb(200, 250, 250, 250));
                    FirstLightBubSec1.Foreground = new SolidColorBrush(Colors.Black);
                    LEDpinValue = GpioPinValue.Low;
                    LEDpin.Write(LEDpinValue);
                    LightDigital = false;
                    PayLoad = "Turn Off Light";
            }
        }

        private void ToggleRelaySec1_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
                if (toggleSwitch.IsOn == true)
                {
                    RELAYpinValue = GpioPinValue.Low;
                    RELAYpin.Write(RELAYpinValue);
                    RelayDigital = true;
                    PayLoad = "Turn On Relay";
                }
                else
                {
                    RELAYpinValue = GpioPinValue.High;
                    RELAYpin.Write(RELAYpinValue);
                    RelayDigital = false;
                    PayLoad = "Turn Off Relay";
            }
        }

        private void FirstLightTest_Click(object sender, RoutedEventArgs e)
        {
            InitGPIO();
        }
    }
}

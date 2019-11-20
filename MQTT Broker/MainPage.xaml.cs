using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using MQTT_Broker.PopUps;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MQTT_Broker
{

    public class TempDemo
    {
        public TempDemo(int temp, int time)
        {
            Temp = temp;
            Time = time;
        }

        public double Temp { get; set; }
        public int Time { get; set; }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Compositor compositor = Window.Current.Compositor;
        private Visual backvisual;
        private const int LED_PIN = 17, RELAY_PIN = 27;
        private GpioPin LEDpin, RELAYpin;
        private GpioPinValue LEDpinValue, RELAYpinValue;
        private string PayLoad;
        private bool  RelayDigital, IsConnected, _isSwiped, IsPublic;
        BitmapImage LED = new BitmapImage(new Uri("ms-appx:///Assets/Diagrams/LEDDigital.png"));
        BitmapImage RELAY = new BitmapImage(new Uri("ms-appx:///Assets/Diagrams/RelayDigital.png"));
        Random rdm = new Random();
        private ScalarKeyFrameAnimation rotate;
        MqttApplicationMessage message;
        MQTTSettings popup = new MQTTSettings();


        IMqttServer mqttServer = new MqttFactory().CreateMqttServer();
        public MainPage()
        {
            this.InitializeComponent();
            ToolBar.ToggleMQTT += new RoutedEventHandler(MqttToggled);
            //MQTTBrokerInit();
        }

        private async void MqttToggled(object sender, RoutedEventArgs e)
        {
            popup.Sub += new RoutedEventHandler(MQTTSubcribe);
            await popup.ShowAsync();
            await MQTTBrokerInit();
        }

        private void MQTTSubcribe(object sender, RoutedEventArgs e) 
        {
            ToggleFirstLightSection1.IsEnabled = true;
            Subcribe(popup.targetIP, popup.topic);
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Load Chart Data
            (TempChart.Series[0] as LineSeries).ItemsSource =
                        from i in Enumerable.Range(0, 12)
                        let temp = rdm.Next()
                        let time = rdm.Next(0, 24)
                        select new TempDemo(temp, time);
            //Create animation
            backvisual = ElementCompositionPreview.GetElementVisual(FanIcon);
            ElementCompositionPreview.SetIsTranslationEnabled(FanIcon, true);
            backvisual.Size = new Vector2(100, 100);
            backvisual.CenterPoint = new Vector3(backvisual.Size / 2, 0);

            rotate = compositor.CreateScalarKeyFrameAnimation();

            var linear = compositor.CreateLinearEasingFunction();

            var startingValue = ExpressionValues.StartingValue.CreateScalarStartingValue();

            rotate.InsertExpressionKeyFrame(0.0f, startingValue);
            rotate.InsertExpressionKeyFrame(1.0f, startingValue + 360f, linear);
            rotate.Duration = TimeSpan.FromMilliseconds(1000);
            rotate.IterationBehavior = AnimationIterationBehavior.Forever;
            backvisual.StartAnimation(nameof(Visual.RotationAngleInDegrees), rotate);

        }




        //private bool InitGPIO(TextBlock txt)
        //{
        //    var gpio = GpioController.GetDefault();

        //    // Show an error if there is no GPIO controller
        //    if (gpio == null)
        //    {
        //        txt.Text = "There is no GPIO controller on this device.";
        //        txt.Foreground = new SolidColorBrush(Colors.Red);
        //        return false;
        //    }

        //    txt.Text = "GPIO controller initialized correctly.";
        //    txt.Foreground = new SolidColorBrush(Colors.Green);

        //    if(txt == StatusDigitalRelay)
        //    {
        //        RELAYpin = gpio.OpenPin(RELAY_PIN);
        //        RELAYpinValue = GpioPinValue.High;
        //        RELAYpin.Write(RELAYpinValue);
        //        RELAYpin.SetDriveMode(GpioPinDriveMode.Output);
        //        return true;
        //    }

        //    LEDpin = gpio.OpenPin(LED_PIN);
        //    LEDpinValue = GpioPinValue.Low;
        //    LEDpin.Write(LEDpinValue);
        //    LEDpin.SetDriveMode(GpioPinDriveMode.Output);
        //    return true;
        //}

        public async Task MQTTBrokerInit()
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionBacklog(100)
                .WithDefaultEndpointPort(1884);
            try
            {
                await mqttServer.StartAsync(optionsBuilder.Build());
                System.Diagnostics.Debug.WriteLine("Success");
                IsPublic = true;
            }
            catch (MQTTnet.Exceptions.MqttCommunicationException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        public async Task SendPayload(string topic, string payload)
        {
            message = new MqttApplicationMessageBuilder()
              .WithTopic(topic)
              .WithPayload(payload)
              .WithExactlyOnceQoS()
              .WithRetainFlag(true)
              .Build();

            await mqttServer.PublishAsync(message);
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
                            if (IsPublic == true)
                            {
                                SendPayload("Digital/Light", "On");
                            }
                }
                else
                {
                    Test.Background = new SolidColorBrush(Color.FromArgb(200, 250, 250, 250));
                    FirstLightBubSec1.Foreground = new SolidColorBrush(Colors.Black);
                    LEDpinValue = GpioPinValue.Low;
                    LEDpin.Write(LEDpinValue);
                            if (IsPublic == true)
                            {
                                SendPayload("Digital/Light", "Off");
                            }
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
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                LEDpin = null;
                StatusDigitalLed.Text = "There is no GPIO controller on this device.";
                StatusDigitalLed.Foreground = new SolidColorBrush(Colors.Red);
                ToggleFirstLightSection1.IsEnabled = false;
                return;
            }
            else
            {
                LEDpin = gpio.OpenPin(LED_PIN);
                LEDpinValue = GpioPinValue.Low;
                LEDpin.Write(LEDpinValue);
                LEDpin.SetDriveMode(GpioPinDriveMode.Output);
                ToggleFirstLightSection1.IsEnabled = true;


                StatusDigitalLed.Text = "GPIO controller initialized correctly.";
                StatusDigitalLed.Foreground = new SolidColorBrush(Colors.Green);
            }

        }



        private void RelayTest_Click(object sender, RoutedEventArgs e)
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                RELAYpin = null;
                StatusDigitalRelay.Text = "There is no GPIO controller on this device.";
                StatusDigitalRelay.Foreground = new SolidColorBrush(Colors.Red);
                ToggleRelaySec1.IsEnabled = false;
                return;
            } else
            {
                RELAYpin = gpio.OpenPin(RELAY_PIN);
                RELAYpinValue = GpioPinValue.High;
                RELAYpin.Write(RELAYpinValue);
                RELAYpin.SetDriveMode(GpioPinDriveMode.Output);
                ToggleRelaySec1.IsEnabled = true;


                StatusDigitalRelay.Text = "GPIO controller initialized correctly.";
                StatusDigitalRelay.Foreground = new SolidColorBrush(Colors.Green);
            }

        }

        private void AddControl_Click(object sender, RoutedEventArgs e)
        {
            //Add Controls
        }



        private async void InstructionClick(object sender, RoutedEventArgs e)
        {
            if(sender == FirstLightInstruc) 
            {
                InstructionDiaglog diaglog = new InstructionDiaglog("27", "1 LED, 1 Capacitor", LED)
                {
                    Title = "Light Digital Example"
                };
                await diaglog.ShowAsync();
            }
            else if(sender == RelayInstruc) 
            {
                InstructionDiaglog diaglog = new InstructionDiaglog("17", "1 Relay with 1 Capacitor", RELAY)
                {
                    Title = "Relay Digital Example"
                };
                await diaglog.ShowAsync();
            }
        }

        private void SwipeableTextBlock_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial && !_isSwiped)
            {
                var swipedDistance = e.Cumulative.Translation.Y;

                if (Math.Abs(swipedDistance) <= 2) return;

                if (swipedDistance > 0)
                {
                    TheToolBar.Translation = new Vector3(0, 100, 0);
                }
                else
                {
                    TheToolBar.Translation = new Vector3(0, 0, 0);
                }
                _isSwiped = true;
            }
        }

        private void SwipeableTextBlock_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            _isSwiped = false;
        }

        private async Task<bool> Subcribe(string ipv4, string topic) 
        {
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                                    .WithClientId("Raspberry pi 3 Control Center")
                                    .WithTcpServer(ipv4, 1884)
                                    .Build();
            try
            {
                await mqttClient.ConnectAsync(options);
            }
            catch (MQTTnet.Exceptions.MqttCommunicationException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            await mqttClient.SubscribeAsync
                (new TopicFilterBuilder().WithTopic(topic).Build());
            IsConnected = mqttClient.IsConnected;
            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                if(e.ApplicationMessage.Topic == "Light/Digital" && Encoding.UTF8.GetString(e.ApplicationMessage.Payload) == "On") 
                {
                    ToggleFirstLightSection1.IsOn = true;
                } else if (e.ApplicationMessage.Topic == "Light/Digital" && Encoding.UTF8.GetString(e.ApplicationMessage.Payload) == "Off") 
                {
                    ToggleFirstLightSection1.IsOn = false;
                } else if (e.ApplicationMessage.Topic == "Relay/Digital" && Encoding.UTF8.GetString(e.ApplicationMessage.Payload) == "On") 
                {

                } else if (e.ApplicationMessage.Topic == "Relay/Digital" && Encoding.UTF8.GetString(e.ApplicationMessage.Payload) == "Off") 
                {

                }
                System.Diagnostics.Debug.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                System.Diagnostics.Debug.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                System.Diagnostics.Debug.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");

            });
            return false;
        }
    }
}

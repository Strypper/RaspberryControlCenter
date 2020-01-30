using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MQTT_Broker.PopUps
{
    public sealed partial class MQTTSettings : ContentDialog
    {
        public string targetIP { get { return TargetIp.Text; } }
        public string topic { get { return Topic.Text; } }
        public string message { get { return Message.Text; } }
        public event RoutedEventHandler Sub;

        public MQTTSettings()
        {
            this.InitializeComponent();
            IpV4.Text = GetLocalIp();
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            MQTTDialog.Hide();
        }

        protected void Subcribe_Click(object sender, RoutedEventArgs e)
        {
            if(Sub != null)
            {
                this.Sub(this, e);
            }
        }
        public string GetLocalIp(HostNameType hostNameType = HostNameType.Ipv4)
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            if (icp?.NetworkAdapter == null) return null;
            var hostname =
                NetworkInformation.GetHostNames()
                    .FirstOrDefault(
                        hn =>
                            hn.Type == hostNameType &&
                            hn.IPInformation?.NetworkAdapter != null &&
                            hn.IPInformation.NetworkAdapter.NetworkAdapterId == icp.NetworkAdapter.NetworkAdapterId);

            // the ip address
            return hostname?.CanonicalName;
        }

        private async void Find_Click(object sender, RoutedEventArgs e)
        {
            //var optionsBuilder = new MqttServerOptionsBuilder()
            //                        .WithConnectionBacklog(100)
            //                        .WithDefaultEndpointPort(1884);
            //try
            //{
            //    await mqttServer.StartAsync(optionsBuilder.Build());
            //    System.Diagnostics.Debug.WriteLine("Success");
            //    IsPublic = true;
            //}
            //catch (MQTTnet.Exceptions.MqttCommunicationException e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e);
            //}
        }
    }
}

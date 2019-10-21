using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    }
}

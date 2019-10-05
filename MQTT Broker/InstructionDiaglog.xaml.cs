using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MQTT_Broker
{
    public sealed partial class InstructionDiaglog : ContentDialog
    {
        public string Pin { get; set; }
        public string Components { get; set; }
        public ImageSource Image { get; set; }
        public InstructionDiaglog(string pin, string component, BitmapImage img)
        {
            this.InitializeComponent();
            Pin = pin;
            Components = component;
            Image = img;
            ComponentsUI.Text = component;
            PinUI.Text = pin;
            ArduinoSketch.Source = Image;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            dialog.Hide();
        }
    }
}

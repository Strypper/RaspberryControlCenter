using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using MQTT_Broker.PopUps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MQTT_Broker.ToolBar
{
    public sealed partial class ToolBar : UserControl
    {
        private Compositor compositor = Window.Current.Compositor;
        private Visual backVisual;
        private ScalarKeyFrameAnimation rotate;
        public event RoutedEventHandler ToggleMQTT;
        public ToolBar()
        {
            this.InitializeComponent();
        }

        private void Setting_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            backVisual.StartAnimation(nameof(Visual.RotationAngleInDegrees), rotate);
        }

        private void Setting_PointerExited(object sender, PointerRoutedEventArgs e)
        {

            //backVisual.StopAnimation(nameof(Visual.RotationAngleInDegrees));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            backVisual = ElementCompositionPreview.GetElementVisual(Setting);
            backVisual.Size = new System.Numerics.Vector2(40, 40);
            backVisual.CenterPoint = new System.Numerics.Vector3(backVisual.Size / 2, 0);

            rotate = compositor.CreateScalarKeyFrameAnimation();

            //var linear = compositor.CreateLinearEasingFunction();

            var startingValue = ExpressionValues.StartingValue.CreateScalarStartingValue();

            rotate.InsertExpressionKeyFrame(0.0f, startingValue);
            rotate.InsertExpressionKeyFrame(1.0f, startingValue + 360f);
            rotate.Duration = TimeSpan.FromMilliseconds(1000);
            //rotate.IterationBehavior = AnimationIterationBehavior.Forever;
        }

        protected async void MQTTToggle_Checked(object sender, RoutedEventArgs e)
        {
            if(ToggleMQTT != null)
            {
                this.ToggleMQTT(this, e);
            }
        }

        private void EButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
        }

        private void EButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            CoreApplication.Exit();
        }

        private async void RaspberryGPIO_Click(object sender, RoutedEventArgs e)
        {
            RaspberryPiMap m = new RaspberryPiMap();
            await m.ShowAsync();
        }
    }
}

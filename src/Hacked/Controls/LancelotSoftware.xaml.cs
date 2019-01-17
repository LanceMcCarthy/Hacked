using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Hacked.Controls
{
    public sealed partial class LancelotSoftware : UserControl
    {
        public static readonly DependencyProperty GearOneBrushProperty = DependencyProperty.Register(
            "GearOneBrush", typeof(SolidColorBrush), typeof(LancelotSoftware), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public SolidColorBrush GearOneBrush
        {
            get { return (SolidColorBrush) GetValue(GearOneBrushProperty); }
            set { SetValue(GearOneBrushProperty, value); }
        }

        public static readonly DependencyProperty GearTwoBrushProperty = DependencyProperty.Register(
            "GearTwoBrush", typeof(SolidColorBrush), typeof(LancelotSoftware), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public SolidColorBrush GearTwoBrush
        {
            get { return (SolidColorBrush) GetValue(GearTwoBrushProperty); }
            set { SetValue(GearTwoBrushProperty, value); }
        }

        public static readonly DependencyProperty TextBackgroundProperty = DependencyProperty.Register(
            "TextBackground", typeof(SolidColorBrush), typeof(LancelotSoftware), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush TextBackground
        {
            get { return (SolidColorBrush) GetValue(TextBackgroundProperty); }
            set { SetValue(TextBackgroundProperty, value); }
        }

        public LancelotSoftware()
        {
            this.InitializeComponent();
            GearOneBrush = new SolidColorBrush(Colors.Blue);
            GearTwoBrush = new SolidColorBrush(Colors.Red);
            this.Loaded += LancelotSoftware_Loaded;
        }

        void LancelotSoftware_Loaded(object sender, RoutedEventArgs e)
        {
            //SpinStory.RepeatBehavior = RepeatBehavior.Forever;
            //SpinStory.AutoReverse = true;
            //this.SpinStory.Begin();

            GearsStory.RepeatBehavior = RepeatBehavior.Forever;
            this.GearsStory.Begin();
        }
    }
}

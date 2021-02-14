using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Hacked.Wpf.Controls
{
    public partial class LancelotSoftware : UserControl
    {
        public LancelotSoftware()
        {
            GearOneBrush = new SolidColorBrush(Colors.Blue);
            GearTwoBrush = new SolidColorBrush(Colors.Red);
            this.InitializeComponent();
            this.Loaded += LancelotSoftware_Loaded;
        }

        void LancelotSoftware_Loaded(object sender, RoutedEventArgs e)
        {
            if (Resources["GearsStory"] is Storyboard gearsStoryboard)
            {
                gearsStoryboard.RepeatBehavior = RepeatBehavior.Forever;
                gearsStoryboard.Begin();
            }
        }
        
        public static readonly DependencyProperty GearOneBrushProperty = DependencyProperty.Register(
            "GearOneBrush", typeof(SolidColorBrush), typeof(LancelotSoftware), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public SolidColorBrush GearOneBrush
        {
            get => (SolidColorBrush)GetValue(GearOneBrushProperty);
            set => SetValue(GearOneBrushProperty, value);
        }

        public static readonly DependencyProperty GearTwoBrushProperty = DependencyProperty.Register(
            "GearTwoBrush", typeof(SolidColorBrush), typeof(LancelotSoftware), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public SolidColorBrush GearTwoBrush
        {
            get => (SolidColorBrush)GetValue(GearTwoBrushProperty);
            set => SetValue(GearTwoBrushProperty, value);
        }

        public static readonly DependencyProperty TextBackgroundProperty = DependencyProperty.Register(
            "TextBackground", typeof(SolidColorBrush), typeof(LancelotSoftware), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush TextBackground
        {
            get => (SolidColorBrush)GetValue(TextBackgroundProperty);
            set => SetValue(TextBackgroundProperty, value);
        }
    }
}

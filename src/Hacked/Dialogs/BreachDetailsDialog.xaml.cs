using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Hacked.Core.Models;

namespace Hacked.Dialogs
{
    public sealed partial class BreachDetailsDialog : ContentDialog
    {
        public static readonly DependencyProperty SelectedBreachProperty = DependencyProperty.Register(
            "SelectedBreach", typeof(Breach), typeof(BreachDetailsDialog), new PropertyMetadata(default(Breach)));

        public Breach SelectedBreach
        {
            get => (Breach)GetValue(SelectedBreachProperty);
            set => SetValue(SelectedBreachProperty, value);
        }

        public BreachDetailsDialog()
        {
            InitializeComponent();
            DataContext = SelectedBreach;
            Loaded += BreachDetailsDialog_Loaded;
        }

        public BreachDetailsDialog(Breach breach)
        {
            InitializeComponent();
            DataContext = SelectedBreach = breach;
            Loaded += BreachDetailsDialog_Loaded;
        }

        private void BreachDetailsDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedBreach?.Description))
            {
                this.Hide();
            }
            else
            {
                this.Title = this.SelectedBreach.Title;

                BreachDetailsWebView.NavigateToString(this.SelectedBreach.Description);
                
                //IconWebView.Source = new Uri($"https://az594751.vo.msecnd.net/cdn/{SelectedBreach.Name}.{SelectedBreach.LogoType}", UriKind.Absolute);
            }

        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }
    }
}

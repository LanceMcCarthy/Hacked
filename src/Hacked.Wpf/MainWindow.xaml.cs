using System.Windows;
using Hacked.Wpf.ViewModels;

namespace Hacked.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await (DataContext as MainWindowViewModel)?.InitializeApp();
        }
    }
}

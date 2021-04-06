using Hacked.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hacked.Wpf.Views
{
    public partial class AddAccountView : UserControl
    {
        private string enteredText;
        public AddAccountView()
        {
            InitializeComponent();
        }

        private void EmailInput_OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {

                if (e.Key == Key.Enter || e.SystemKey == Key.Enter)
                {
                    if (!string.IsNullOrEmpty(enteredText))
                    {
                        AddMonitoredAccount();
                    }
                }
            }
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            ResetAndHide();
        }

        private void OkayButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(enteredText))
            {
                AddMonitoredAccount();
            }
        }

        private void TxtChanged(object sender, TextChangedEventArgs e)
        {
            AddButton.IsEnabled = !string.IsNullOrEmpty(EmailInput.Text);

            if (AddButton.IsEnabled)
                enteredText = EmailInput.Text;
        }

        private async void AddMonitoredAccount()
        {
            if (string.IsNullOrEmpty(enteredText))
                return;

            var addAccount = (DataContext as MainWindowViewModel)?.AddAccount(enteredText);

            if (addAccount != null)
                await addAccount;

            ResetAndHide();
        }

        private void ResetAndHide()
        {
            EmailInput.Text = "";
            //Visibility = Visibility.Collapsed;
        }
    }
}

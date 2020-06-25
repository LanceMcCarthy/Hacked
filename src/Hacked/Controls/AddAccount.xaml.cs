using Hacked.ViewModels;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hacked.Controls
{
    public sealed partial class AddAccount : UserControl
    {
        private string enteredText;

        public AddAccount()
        {
            InitializeComponent();

            if(!DesignMode.DesignModeEnabled)
                CoreWindow.GetForCurrentThread().KeyDown += AddAccount_KeyDown;
        }

        private void AddAccount_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (Visibility == Visibility.Visible)
            {
                if (args.VirtualKey == VirtualKey.Enter)
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

            var addAccount = (DataContext as MainViewModel)?.AddAccount(enteredText);

            if (addAccount != null)
                await addAccount;

            ResetAndHide();
        }

        private void ResetAndHide()
        {
            EmailInput.Text = "";
            Visibility = Visibility.Collapsed;
        }
    }
}

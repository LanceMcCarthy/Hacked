using System;
using System.Threading.Tasks;
using Hacked.Maui.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Hacked.Maui.Views
{
    public partial class AddAccountPage : ContentPage
    {
        public AddAccountPage()
        {
            InitializeComponent();
        }

        private async void AddAccount_OnClicked(object sender, EventArgs e)
        {
            await AttemptEmailAddAsync(this.EmailEntry?.Text);
        }

        private async void EmailEntry_OnCompleted(object sender, EventArgs e)
        {
            await AttemptEmailAddAsync(this.EmailEntry?.Text);
        }

        private void EmailEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //var entry = sender as Entry;
            //var text = entry?.Text;

            //var isValid = !string.IsNullOrEmpty(text);

            //AddAccountButton.BackgroundColor = isValid ? (Color)Application.Current.Resources["ThemeAccentDarkColor"] : (Color)Application.Current.Resources["ThemeTextLightColor"];
            //AddAccountButton.TextColor = isValid ? (Color)Application.Current.Resources["ThemeBackgroundColor"] : (Color)Application.Current.Resources["ThemeTextColor"];
            //AddAccountButton.IsEnabled = isValid;
        }

        private async Task AttemptEmailAddAsync(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                return;

            var addedAccount = await ViewModelLocator.Main.AddAccount(emailAddress);

            if (addedAccount == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "The account was not added, try again", "OK");
            }

            await ((Application.Current.MainPage as RootPage)?.Detail as NavigationPage)?.PopAsync();
        }

        private async void CancelButton_OnClicked(object sender, EventArgs e)
        {
            await ((Application.Current.MainPage as RootPage)?.Detail as NavigationPage)?.PopAsync();
        }
    }
}
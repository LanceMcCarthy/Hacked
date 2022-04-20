using Hacked.Maui.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Threading.Tasks;
using Telerik.Maui.Controls;

namespace Hacked.Maui.Views;

public partial class AddAccountPage : ContentPage
{
    public AddAccountPage()
    {
        InitializeComponent();
    }

    private async void AddAccount_OnClicked(object sender, EventArgs e)
    {
        await AttemptEmailAddAsync(EmailEntry?.Text);
    }

    private async void EmailEntry_OnCompleted(object sender, EventArgs e)
    {
        await AttemptEmailAddAsync(EmailEntry?.Text);
    }

    private void EmailEntry_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        bool isValid = false;

        if (sender is RadEntry entry)
        {
            isValid = !string.IsNullOrEmpty(entry.Text);
        }

        AddAccountButton.BackgroundColor = isValid 
            ? (Color)Application.Current.Resources["ThemeAccentDarkColor"]
            : (Color)Application.Current.Resources["ThemeTextLightColor"];

        AddAccountButton.TextColor = isValid
            ? (Color)Application.Current.Resources["ThemeBackgroundColor"]
            : (Color)Application.Current.Resources["ThemeTextColor"];

        AddAccountButton.IsEnabled = isValid;
    }

    private async Task AttemptEmailAddAsync(string emailAddress)
    {
        if (string.IsNullOrEmpty(emailAddress))
            return;

        var addedAccount = await ViewModelLocator.Accounts.AddAccount(emailAddress);

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
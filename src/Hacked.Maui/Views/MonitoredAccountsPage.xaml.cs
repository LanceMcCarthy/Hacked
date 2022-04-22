using Hacked.Core.Models;
using Hacked.Maui.Common;
using Hacked.Maui.ViewModels;
using Telerik.Maui.Controls;
using Telerik.XamarinForms.DataControls;
using Telerik.XamarinForms.DataControls.ListView;

namespace Hacked.Maui.Views;

public partial class MonitoredAccountsPage : ContentPage
{
	public MonitoredAccountsPage()
	{
		InitializeComponent();
    }

    private async void AccountTapped(object? sender, ItemTapEventArgs e)
    {
        if (e.Item is MonitoredAccount account && sender is RadListView { IsSwipingInProgress: false })
        {
            ViewModelLocator.MonitoredAccounts.SelectedAccount = account;

            // TODO navigation https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/shell/navigation#absolute-routes
            //await ((Application.Current.MainPage as RootPage)?.Detail as NavigationPage)?.Navigation.PushModalAsync(new AccountDetailPage());
            await Shell.Current.GoToAsync("/accountdetails");
        }
    }

    private async void RefreshSwipeButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Performed by command now
            //if (sender is Button { BindingContext: MonitoredAccount account })
            //{
            //    var countBeforeUpdate = account.Breaches.Count;

            //    await ViewModelLocator.MonitoredAccounts.UpdateBreachesForAccountAsync(account);

            //    if (account.Breaches.Count <= countBeforeUpdate)
            //        return;
                
            //    ViewModelLocator.MonitoredAccounts.SaveAccounts();

            //    await Shell.Current.DisplayAlert("New breaches have been detected", "Alert", "close");
            //}
        }
        finally
        {
            if (sender is RadListView rlv)
                rlv.EndItemSwipe();
        }
    }

    private void DeleteSwipeButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Performed by command now
            //if (sender is Button { BindingContext: MonitoredAccount account })
            //{
            //    ViewModelLocator.MonitoredAccounts.RemoveAccount(account);
            //}
        }
        finally
        {
            if (sender is RadListView rlv)
                rlv.EndItemSwipe();
        }
    }

    private void AddAccountPopup_Clicked(object sender, EventArgs e)
    {
        popup.IsOpen = true;
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
        if (sender is RadEntry entry)
        {
            AddAccountButton.IsEnabled = !string.IsNullOrEmpty(entry.Text);
        }

        // This will get set by the VisualStateManager on the implicit RadButton style in TelerikStyles
        //AddAccountButton.BackgroundColor = isValid
        //    ? (Color)Application.Current.Resources["ThemeAccentDarkColor"]
        //    : (Color)Application.Current.Resources["ThemeTextLightColor"];
        //AddAccountButton.TextColor = isValid
        //    ? (Color)Application.Current.Resources["ThemeBackgroundColor"]
        //    : (Color)Application.Current.Resources["ThemeTextColor"];
        // AddAccountButton.IsEnabled = isValid;
    }

    private async Task AttemptEmailAddAsync(string emailAddress)
    {
        if (string.IsNullOrEmpty(emailAddress))
            return;

        var addedAccount = await ViewModelLocator.MonitoredAccounts.AddAccount(emailAddress);

        if (addedAccount == null)
        {
            await Shell.Current.DisplayAlert("Error", "The account was not added, try again", "OK");
        }

        popup.IsOpen = false;
    }

    private void CancelButton_OnClicked(object sender, EventArgs e)
    {
        popup.IsOpen = false;
    }

    private async void AccountsListView_OnRefreshRequested(object sender, PullToRefreshRequestedEventArgs e)
    {
        try
        {
            if (!ViewModelLocator.MonitoredAccounts.HasAccounts)
                return;

            await ViewModelLocator.MonitoredAccounts.FindAllAccountsBreachesAsync();

            //if first time, hide tip and persist via settings
            if (AccountRefreshTip.IsVisible)
            {
                await AccountRefreshTip.FadeTo(0, 500, Easing.CubicInOut);
                AccountRefreshTip.IsVisible = false;
                Settings.AccountRefreshShown = true;
            }
        }
        finally
        {
            if (sender is RadListView rlv)
                rlv.EndRefresh();
        }
    }

    private async void AccountsListView_OnItemSwipeCompleted(object sender, ItemSwipeCompletedEventArgs e)
    {
        if (e.Item is not MonitoredAccount account) 
            return;

        if (e.Offset > 201)
        {
            var lastCount = account.Breaches.Count;

            await ViewModelLocator.MonitoredAccounts.UpdateBreachesForAccountAsync(account);

            if (account.Breaches.Count > lastCount)
            {
                ViewModelLocator.MonitoredAccounts.SaveAccounts();

                await Shell.Current.DisplayAlert("New breaches have been detected", "Alert", "close");
            }
        }
        else if (e.Offset < -200)
        {
            await ViewModelLocator.MonitoredAccounts.RemoveAccount(account);
        }

        if (sender is RadListView rlv)
            rlv.EndRefresh();
    }

    private async void AccountsListView_OnItemTapped(object sender, ItemTapEventArgs e)
    {
        if (sender is RadListView { IsSwipingInProgress: true })
            return;

        if (e?.Item is MonitoredAccount account)
        {
            ViewModelLocator.MonitoredAccounts.SelectedAccount = account;

            // todo navigation https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/shell/navigation#absolute-routes
            //await ((Application.Current.MainPage as RootPage)?.Detail as NavigationPage)?.Navigation.PushAsync(new AccountDetailsPage());
            await Shell.Current.GoToAsync("accountdetails");
        }
    }

    private async void RefreshTipCheckBox_OnIsCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value == false)
        {
            await AccountRefreshTip.FadeTo(0, 300, Easing.CubicInOut);
            AccountRefreshTip.IsVisible = false;
            Settings.AccountRefreshShown = true;

            await SwipeTip.FadeTo(0, 300, Easing.CubicInOut);
            SwipeTip.IsVisible = false;
            Settings.SwipeTipShown = true;
        }
        else if (e.Value == true)
        {
            await AccountRefreshTip.FadeTo(1, 300, Easing.CubicInOut);
            AccountRefreshTip.IsVisible = true;
            Settings.AccountRefreshShown = false;

            await SwipeTip.FadeTo(1, 300, Easing.CubicInOut);
            SwipeTip.IsVisible = true;
            Settings.SwipeTipShown = false;
        }
    }
}
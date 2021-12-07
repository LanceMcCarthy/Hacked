using System;
using System.Threading.Tasks;
using Hacked.Core.Models;
using Hacked.Maui.Common;
using Hacked.Maui.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Telerik.XamarinForms.DataControls;
using Telerik.XamarinForms.DataControls.ListView;
using Telerik.XamarinForms.Input;

namespace Hacked.Maui.Views;

public partial class AccountsPage : ContentPage
{
    public AccountsPage()
    {
        InitializeComponent();
    }

    private async void AccountTapped(object? sender, ItemTapEventArgs e)
    {
        if (sender == null || AccountsListView.IsSwipingInProgress)
            return;

        if (e.Item is MonitoredAccount account)
        {
            ViewModelLocator.Accounts.SelectedAccount = account;

            await ((Application.Current.MainPage as RootPage)?.Detail as NavigationPage)?.Navigation.PushModalAsync(new AccountDetailPage());
        }
    }

    private async void RefreshSwipeButton_Clicked(object sender, EventArgs e)
    {
        if (AccountsListView == null)
            return;

        try
        {
            if (sender is Button button && button.BindingContext is MonitoredAccount account)
            {
                var lastCount = account.Breaches.Count;

                await ViewModelLocator.Accounts.UpdateBreachesForAccountAsync(account);

                if (account.Breaches.Count > lastCount)
                {
                    ViewModelLocator.Accounts.SaveAccounts();
                    await this.DisplayAlert("New breaches have been detected", "Alert", "close");
                }
            }
        }
        finally
        {
            //AccountsListView.EndItemSwipe();
        }
    }

    private void DeleteSwipeButton_Clicked(object sender, EventArgs e)
    {
        if (AccountsListView == null)
            return;

        try
        {
            if (sender is Button button && button?.BindingContext is MonitoredAccount account)
            {
                ViewModelLocator.Accounts.RemoveAccount(account);
            }
        }
        finally
        {
            //AccountsListView.EndItemSwipe();
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

        popup.IsOpen = false;
    }

    private async void CancelButton_OnClicked(object sender, EventArgs e)
    {
        popup.IsOpen = false;
    }

    //private async void AccountsListView_OnRefreshRequested(object sender, PullToRefreshRequestedEventArgs e)
    //{
    //    if (AccountsListView == null)
    //        return;

    //    try
    //    {
    //        if (!ViewModelLocator.Main.HasAccounts)
    //            return;

    //        await ViewModelLocator.Main.FindAllAccountsBreachesAsync();

    //        //if first time, hide tip and persist via settings
    //        if (AccountRefreshTip.IsVisible)
    //        {
    //            await AccountRefreshTip.FadeTo(0, 500, Easing.CubicInOut);
    //            AccountRefreshTip.IsVisible = false;
    //            Settings.AccountRefreshShown = true;
    //        }
    //    }
    //    finally
    //    {
    //        AccountsListView.EndRefresh();
    //    }
    //}

    private async void AccountsListView_OnItemSwipeCompleted(object sender, ItemSwipeCompletedEventArgs e)
    {
        var listView = sender as RadListView;

        if (e == null || listView == null)
            return;

        if (e.Item is MonitoredAccount account)
        {
            if (e.Offset > 201)
            {
                var lastCount = account.Breaches.Count;

                await ViewModelLocator.Accounts.UpdateBreachesForAccountAsync(account);

                if (account.Breaches.Count > lastCount)
                {
                    ViewModelLocator.Accounts.SaveAccounts();
                    await Application.Current.MainPage.DisplayAlert("New breaches have been detected", "Alert", "close");
                }

                listView?.EndItemSwipe();
            }
            else if (e.Offset < -200)
            {
                ViewModelLocator.Accounts.RemoveAccount(account);
                listView.EndItemSwipe();
            }
        }
    }

    //private async void AccountsListView_OnItemTapped(object sender, ItemTapEventArgs e)
    //{
    //    if (sender == null || AccountsListView.IsSwipingInProgress)
    //        return;

    //    if (e?.Item is MonitoredAccount account)
    //    {
    //        ViewModelLocator.Main.SelectedAccount = account;

    //        await ((Application.Current.MainPage as RootPage)?.Detail as NavigationPage)?.Navigation.PushAsync(new AccountDetailsPage());
    //    }
    //}

    //private async void RefreshTipCheckBox_OnIsCheckedChanged(object sender, CheckedChangedEventArgs e)
    //{
    //    if (e.Value == false)
    //    {
    //        await AccountRefreshTip.FadeTo(0, 300, Easing.CubicInOut);
    //        AccountRefreshTip.IsVisible = false;
    //        Settings.AccountRefreshShown = true;

    //        await SwipeTip.FadeTo(0, 300, Easing.CubicInOut);
    //        SwipeTip.IsVisible = false;
    //        Settings.SwipeTipShown = true;
    //    }
    //    else if (e.Value == true)
    //    {
    //        await AccountRefreshTip.FadeTo(1, 300, Easing.CubicInOut);
    //        AccountRefreshTip.IsVisible = true;
    //        Settings.AccountRefreshShown = false;

    //        await SwipeTip.FadeTo(1, 300, Easing.CubicInOut);
    //        SwipeTip.IsVisible = true;
    //        Settings.SwipeTipShown = false;
    //    }
    //}
}
#pragma warning disable CA1416

using Hacked.Core.Models;
using Hacked.Maui.Common;
using Hacked.Maui.ViewModels;
using Telerik.Maui.Controls;
using Telerik.Maui.Controls.Compatibility.DataControls;
using Telerik.Maui.Controls.Compatibility.DataControls.ListView;

namespace Hacked.Maui.Views;

public partial class MonitoredAccountsPage
{
    public MonitoredAccountsPage()
    {
        InitializeComponent();
    }

    public MonitoredAccountsPage(MonitoredAccountsViewModel vm)
	{
		InitializeComponent();
        this.BindingContext = vm;
    }

    //private async void AccountTapped(object? sender, ItemTapEventArgs e)
    //{
    //    if (e.Item is MonitoredAccount account && sender is RadListView { IsSwipingInProgress: false })
    //    {
    //        ViewModelLocator.MonitoredAccounts.SelectedAccount = account;

    //        await Shell.Current.GoToAsync("/accountdetails");
    //    }
    //}

    private void RefreshSwipeButton_Clicked(object sender, EventArgs e)
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

    private void ToggleAddAccountOverlay(bool show = true)
    {
        //popup.IsOpen = show;
        AddAccountOverlayBorder.IsVisible = show;
    }

    private void AddAccountPopup_Clicked(object sender, EventArgs e)
    {
        //popup.IsOpen = true;
        ToggleAddAccountOverlay();
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
    }

    private async Task AttemptEmailAddAsync(string emailAddress)
    {
        if (string.IsNullOrEmpty(emailAddress))
            return;
        
        var addedAccount = await (BindingContext as MonitoredAccountsViewModel).AddAccountAsync(emailAddress);

        if (addedAccount == null)
        {
            await Shell.Current.DisplayAlert("Error", "The account was not added, try again", "OK");
        }

        //popup.IsOpen = false;
        ToggleAddAccountOverlay(false);
    }

    private void CancelButton_OnClicked(object sender, EventArgs e)
    {
        //popup.IsOpen = false;
        ToggleAddAccountOverlay(false);
    }

    private async void AccountsListView_OnRefreshRequested(object sender, PullToRefreshRequestedEventArgs e)
    {
        try
        {
            if (!(BindingContext as MonitoredAccountsViewModel).HasAccounts)
                return;

            await (BindingContext as MonitoredAccountsViewModel).FindAllAccountsBreachesAsync();

            //if first time, hide tip and persist via settings
            //if (AccountRefreshTip.IsVisible)
            //{
            //    await AccountRefreshTip.FadeTo(0, 500, Easing.CubicInOut);

            //    AccountRefreshTip.IsVisible = false;
            //    Settings.AccountRefreshShown = true;
            //}
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

            await (BindingContext as MonitoredAccountsViewModel).UpdateBreachesForAccountAsync(account);

            if (account.Breaches.Count > lastCount)
            {
                await (BindingContext as MonitoredAccountsViewModel).Sa

                await Shell.Current.DisplayAlert("New breaches have been detected", "Alert", "close");
            }
        }
        else if (e.Offset < -200)
        {
            await (BindingContext as MonitoredAccountsViewModel).RemoveAccountAsync(account);
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
            (BindingContext as MonitoredAccountsViewModel).SelectedAccount = account;

            await Shell.Current.GoToAsync("accountdetails");
        }
    }

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
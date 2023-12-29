#pragma warning disable CA1416

using Hacked.Maui.ViewModels;
using Telerik.Maui.Controls;
using Telerik.Maui.Controls.Compatibility.Common.Data;

namespace Hacked.Maui.Views;

public partial class MonitoredAccountsPage
{
    private readonly MonitoredAccountsViewModel _viewModel;

    public MonitoredAccountsPage(MonitoredAccountsViewModel vm)
	{
		InitializeComponent();
        this.BindingContext = _viewModel = vm;
    }

    private void MonitoredAccountDataGrid_OnLoaded(object sender, EventArgs e)
    {
        if(!BreachCountColumn.AggregateDescriptors.Any())
        {
            BreachCountColumn.AggregateDescriptors.Add(new PropertyAggregateDescriptor
            {
                PropertyName = "Breaches.Count",
                Caption = "Average per account: ",
                Function = KnownFunction.Average
            });
        }
    }

    private void AddAccountPopup_Clicked(object sender, EventArgs e)
    {
        AddAccountOverlayBorder.IsVisible = true;
    }

    private async void AddAccount_OnCompleted(object sender, EventArgs e)
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

        var addedAccount = await _viewModel.AddAccountAsync(emailAddress);

        if (addedAccount == null)
        {
            await Shell.Current.DisplayAlert("Error", "The account was not added, try again", "OK");
        }
        else
        {
            EmailEntry.Text = string.Empty;
        }

        AddAccountOverlayBorder.IsVisible = false;
    }

    private void CancelButton_OnClicked(object sender, EventArgs e)
    {
        AddAccountOverlayBorder.IsVisible = false;
    }

    //private async void AccountsListView_OnRefreshRequested(object sender, PullToRefreshRequestedEventArgs e)
    //{
    //    try
    //    {
    //        if (!_viewModel.HasAccounts)
    //            return;

    //        await _viewModel.FindAllAccountsBreachesAsync();

    //        //if first time, hide tip and persist via settings
    //        //if (AccountRefreshTip.IsVisible)
    //        //{
    //        //    await AccountRefreshTip.FadeTo(0, 500, Easing.CubicInOut);

    //        //    AccountRefreshTip.IsVisible = false;
    //        //    Settings.AccountRefreshShown = true;
    //        //}
    //    }
    //    finally
    //    {
    //        if (sender is RadListView rlv)
    //            rlv.EndRefresh();
    //    }
    //}
}
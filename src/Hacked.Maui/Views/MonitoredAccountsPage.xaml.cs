#pragma warning disable CA1416
using System.Diagnostics;
using Hacked.Core.Models;
using Hacked.Maui.ViewModels;
using Telerik.Maui.Controls;
using Telerik.Maui.Controls.Data;

namespace Hacked.Maui.Views;

public partial class MonitoredAccountsPage
{
    public MonitoredAccountsPage(MonitoredAccountsViewModel vm)
	{
		InitializeComponent();
        this.BindingContext = vm;
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

    private async void MoreDetailsButton_OnClicked(object? sender, EventArgs e)
    {
        try
        {
            if (sender is not RadButton { CommandParameter: MonitoredAccount account })
                return;

            if (BindingContext is not MonitoredAccountsViewModel vm)
                return;

            vm.SelectedAccount = account;

            await Shell.Current.GoToAsync("///MonitoredAccounts/AccountDetails", new Dictionary<string, object>
            {
                { "SelectedAccount", account }
            });
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex);
        }
    }

    private void ClearNewButton_OnClicked(object? sender, EventArgs e)
    {
        if (sender is not RadButton { CommandParameter: MonitoredAccount account })
            return;

        if (BindingContext is not MonitoredAccountsViewModel vm)
            return;

        if (vm.ClearNewBreachesCommand is System.Windows.Input.ICommand command && command.CanExecute(account))
            command.Execute(account);
    }

    private void RefreshButton_OnClicked(object? sender, EventArgs e)
    {
        if (sender is not RadButton { CommandParameter: MonitoredAccount account })
            return;

        if (BindingContext is not MonitoredAccountsViewModel vm)
            return;

        if (vm.RefreshAccountCommand is System.Windows.Input.ICommand command && command.CanExecute(account))
            command.Execute(account);
    }

    private void RemoveButton_OnClicked(object? sender, EventArgs e)
    {
        if (sender is not RadButton { CommandParameter: MonitoredAccount account })
            return;

        if (BindingContext is not MonitoredAccountsViewModel vm)
            return;

        if (vm.RemoveAccountCommand is System.Windows.Input.ICommand command && command.CanExecute(account))
            command.Execute(account);
    }

    private void AddPendingItemButton_OnClicked(object? sender, EventArgs e)
    {
        if (BindingContext is not MonitoredAccountsViewModel vm)
            return;

        if (vm.AddPendingItemCommand is System.Windows.Input.ICommand command && command.CanExecute(null))
            command.Execute(null);
    }

    private void RemovePendingItemButton_OnClicked(object? sender, EventArgs e)
    {
        if (sender is not RadButton { CommandParameter: PendingAccount account })
            return;

        if (BindingContext is not MonitoredAccountsViewModel vm)
            return;

        if (vm.RemovePendingItemCommand is System.Windows.Input.ICommand command && command.CanExecute(account))
            command.Execute(account);
    }
}
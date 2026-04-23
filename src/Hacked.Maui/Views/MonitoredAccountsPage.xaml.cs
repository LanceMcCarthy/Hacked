#pragma warning disable CA1416

using Hacked.Core.Models;
using Hacked.Maui.ViewModels;
using Telerik.Maui.Controls;
using Telerik.Maui.Controls.Data;
using Telerik.Maui.Controls.DataGrid;

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

        ApplyGroupingPanelTheme();

        if (Application.Current != null)
            Application.Current.RequestedThemeChanged += OnThemeChanged;

        MonitoredAccountDataGrid.Unloaded += (_, _) =>
        {
            if (Application.Current != null)
                Application.Current.RequestedThemeChanged -= OnThemeChanged;
        };
    }

    private void OnThemeChanged(object? sender, AppThemeChangedEventArgs e) =>
        ApplyGroupingPanelTheme();

    private void ApplyGroupingPanelTheme()
    {
        var resources = Application.Current?.Resources;
        if (resources == null) return;

        bool isLight = Application.Current?.RequestedTheme != AppTheme.Dark;

        var servicePanel = FindVisualDescendant<DataGridServicePanel>(MonitoredAccountDataGrid);
        if (servicePanel != null)
        {
            servicePanel.BackgroundColor = isLight
                ? (Color)resources["Gray100"]
                : (Color)resources["Gray900"];
            servicePanel.BorderColor = isLight
                ? (Color)resources["Gray300"]
                : (Color)resources["Gray600"];
        }

        var groupingPanel = FindVisualDescendant<DataGridGroupingPanel>(MonitoredAccountDataGrid);
        if (groupingPanel != null)
        {
            var label = FindVisualDescendant<Label>(groupingPanel);
            if (label != null)
            {
                label.TextColor = isLight
                    ? (Color)resources["PrimaryHighContrast"]
                    : (Color)resources["Gray100"];
            }
        }
    }

    private static T? FindVisualDescendant<T>(IVisualTreeElement element) where T : class
    {
        foreach (var child in element.GetVisualChildren())
        {
            if (child is T result) return result;
            if (child is IVisualTreeElement ve)
            {
                var found = FindVisualDescendant<T>(ve);
                if (found != null) return found;
            }
        }
        return null;
    }

    private async void MoreDetailsButton_OnClicked(object? sender, EventArgs e)
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
}
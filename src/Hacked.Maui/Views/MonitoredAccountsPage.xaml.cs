#pragma warning disable CA1416

using Hacked.Maui.ViewModels;
using Telerik.Maui.Controls.Compatibility.Common.Data;

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
}
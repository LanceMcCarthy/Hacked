using Hacked.Maui.ViewModels;

namespace Hacked.Maui.Views;

public partial class AccountDetailsPage
{
	public AccountDetailsPage(MonitoredAccountsViewModel vm)
    {
        InitializeComponent();
        this.BindingContext = vm;
    }
}
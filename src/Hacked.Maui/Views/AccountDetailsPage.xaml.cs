using Hacked.Maui.ViewModels;

namespace Hacked.Maui.Views;

public partial class AccountDetailsPage : ContentPage
{
	public AccountDetailsPage()
	{
		InitializeComponent();
	}

	public AccountDetailsPage(MonitoredAccountsViewModel vm)
    {
        InitializeComponent();
        this.BindingContext = vm;
    }
}
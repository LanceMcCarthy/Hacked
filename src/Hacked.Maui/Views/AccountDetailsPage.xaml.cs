using Hacked.Maui.ViewModels;

namespace Hacked.Maui.Views;

public partial class AccountDetailsPage
{
    private readonly AccountDetailsViewModel _viewModel;

    public AccountDetailsPage(AccountDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _viewModel = vm;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        _viewModel.SelectedAccount ??= this.SelectedAccount;
    }
}
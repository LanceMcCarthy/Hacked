using Hacked.Maui.ViewModels;

namespace Hacked.Maui.Views;

public partial class AccountDetailsPage
{
    private readonly AccountDetailsViewModel _viewModel;

    public AccountDetailsPage(AccountDetailsViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        this.BindingContext = _viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (_viewModel.SelectedAccount == null)
        {
            _viewModel.SelectedAccount = this.SelectedAccount;
        }
    }
}
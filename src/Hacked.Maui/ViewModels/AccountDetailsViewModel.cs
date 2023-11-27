using CommonHelpers.Common;
using Hacked.Core.Models;

namespace Hacked.Maui.ViewModels;

public class AccountDetailsViewModel : ViewModelBase
{
    private MonitoredAccount _selectedAccount;

    public AccountDetailsViewModel()
    {
    }

    public MonitoredAccount SelectedAccount
    {
        get => _selectedAccount;
        set => SetProperty(ref _selectedAccount, value);
    }
}
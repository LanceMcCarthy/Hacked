using Hacked.Services.Interfaces;
using Hacked.Core.Common;
using System.Net;

namespace Hacked.ViewModels;

public partial class MonitoredAccountsViewModel : ObservableObject
{
    private readonly IAccountsService _accountsService;
    private readonly IPwndBreachService _breachService;
    private readonly INavigator _navigator;
    private readonly ILogger<MonitoredAccountsViewModel> _logger;

    [ObservableProperty]
    private ObservableCollection<MonitoredAccount> _accounts = new();

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isEmpty = true;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public MonitoredAccountsViewModel(
        IAccountsService accountsService,
        IPwndBreachService breachService,
        INavigator navigator,
        ILogger<MonitoredAccountsViewModel> logger)
    {
        _accountsService = accountsService;
        _breachService = breachService;
        _navigator = navigator;
        _logger = logger;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            await _accountsService.LoadAccountsAsync();
            Accounts = _accountsService.CurrentAccounts;
            Accounts.CollectionChanged += (_, _) => IsEmpty = Accounts.Count == 0;
            IsEmpty = Accounts.Count == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load accounts");
        }
    }

    [RelayCommand]
    private void RemoveAccount(MonitoredAccount account)
    {
        Accounts.Remove(account);
        _accountsService.CurrentAccounts.Remove(account);
        _ = _accountsService.SaveAccountsAsync();
    }

    [RelayCommand]
    private async Task CheckAllAccounts()
    {
        if (IsBusy) return;
        IsBusy = true;
        StatusMessage = "Checking all accounts...";
        try
        {
            foreach (var account in Accounts.ToList())
            {
                await CheckAccount(account);
            }
            await _accountsService.SaveAccountsAsync();
            StatusMessage = "Check complete.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking all accounts");
            StatusMessage = "Error checking accounts.";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task CheckSingleAccount(MonitoredAccount account)
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            await CheckAccount(account);
            await _accountsService.SaveAccountsAsync();
        }
        finally { IsBusy = false; }
    }

    private async Task CheckAccount(MonitoredAccount account)
    {
        account.IsUpdating = true;
        try
        {
            var existingNames = account.Breaches.Select(b => b.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            ObservableCollection<Breach> results;
            try
            {
                results = await _breachService.CheckForBreachesAsync(account);
            }
            catch (PwnedApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                results = new ObservableCollection<Breach>();
            }
            account.Breaches.Clear();
            foreach (var breach in results)
            {
                breach.IsNew = !existingNames.Contains(breach.Name);
                account.Breaches.Add(breach);
            }
            account.LastUpdated = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking account {Address}", account.Address);
        }
        finally { account.IsUpdating = false; }
    }

    [RelayCommand]
    private async Task NavigateToAddAccount()
    {
        await _navigator.NavigateViewModelAsync<AddAccountViewModel>(this);
    }

    [RelayCommand]
    private async Task NavigateToAccountDetails(MonitoredAccount account)
    {
        await _navigator.NavigateViewModelAsync<AccountDetailsViewModel>(this, data: account);
    }
}

using Hacked.Services.Interfaces;
using Hacked.Core.Common;
using System.Net;

namespace Hacked.ViewModels;

public partial class AddAccountViewModel : ObservableObject
{
    private readonly IAccountsService _accountsService;
    private readonly IPwndBreachService _breachService;
    private readonly INavigator _navigator;

    [ObservableProperty]
    private string _emailAddress = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public AddAccountViewModel(
        IAccountsService accountsService,
        IPwndBreachService breachService,
        INavigator navigator)
    {
        _accountsService = accountsService;
        _breachService = breachService;
        _navigator = navigator;
    }

    [RelayCommand]
    private async Task AddAccount()
    {
        var email = EmailAddress?.Trim();
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            StatusMessage = "Please enter a valid email address.";
            return;
        }

        var existing = _accountsService.CurrentAccounts
            .Any(a => string.Equals(a.Address, email, StringComparison.OrdinalIgnoreCase));
        if (existing)
        {
            StatusMessage = "This account is already being monitored.";
            return;
        }

        IsBusy = true;
        StatusMessage = "Checking for breaches...";
        try
        {
            var account = new MonitoredAccount { Address = email };
            ObservableCollection<Breach> breaches;
            try
            {
                breaches = await _breachService.CheckForBreachesAsync(account);
            }
            catch (PwnedApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                breaches = new ObservableCollection<Breach>();
            }

            foreach (var breach in breaches)
            {
                breach.IsNew = true;
                account.Breaches.Add(breach);
            }
            account.LastUpdated = DateTime.UtcNow;

            _accountsService.CurrentAccounts.Add(account);
            await _accountsService.SaveAccountsAsync();

            await _navigator.GoBack(this);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await _navigator.GoBack(this);
    }
}

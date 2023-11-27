using System.Diagnostics;
using System.Net;
using CommonHelpers.Common;
using CommunityToolkit.Mvvm.Messaging;
using Hacked.Core.Common;
using Hacked.Core.Models;
using Hacked.Maui.Common.Commands;
using Hacked.Maui.Services;
using Hacked.Services.Interfaces;

namespace Hacked.Maui.ViewModels;

public class AccountDetailsViewModel : ViewModelBase
{
    private readonly IPwndBreachService _apiService;
    private readonly AccountsService _accountsService;
    private MonitoredAccount _selectedAccount;

    public AccountDetailsViewModel(IPwndBreachService srv, AccountsService accountsService)
    {
        _apiService = srv;
        _accountsService = accountsService;

        RefreshAccountCommand = new AsyncCommand<MonitoredAccount>(UpdateBreachesForAccountAsync);
    }

    public MonitoredAccount SelectedAccount
    {
        get => _selectedAccount;
        set => SetProperty(ref _selectedAccount, value);
    }

    public AsyncCommand<MonitoredAccount> RefreshAccountCommand { get; set; }

    public async Task UpdateBreachesForAccountAsync(MonitoredAccount account)
    {
        if (account == null)
        {
            Debug.WriteLine("Account to check for new breaches is null", "UpdateBreachesForAccountAsync");
            return;
        }

        IsBusy = true;
        IsBusyMessage = $"Checking {account.Address} for breaches...";
        account.IsUpdating = true;

        try
        {
            var result = await _apiService.CheckForBreachesAsync(account);

            //compare old list against new list to see if anything is new
            foreach (var breach in result)
            {
                if (!account.Breaches.Contains(breach))
                {
                    breach.IsNew = true;
                }
            }

            //replace old list with new one
            account.Breaches = result;

            await _accountsService.SaveAccountsAsync();
        }
        catch (PwnedApiException ex)
        {
            switch (ex.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    account.Breaches = new();

                    WeakReferenceMessenger.Default.Send(new MessagingCenterAlert
                    {
                        Title = "Good news",
                        Message = $"No breaches found for {account.Address}.",
                        Cancel = "OK"
                    });
                    break;
                case HttpStatusCode.Forbidden:
                    {
                        WeakReferenceMessenger.Default.Send(new MessagingCenterError{ Caller = "UpdateBreachesForAccountAsync", Exception = ex });
                        break;
                    }
            }
        }
        catch (Exception ex)
        {
            WeakReferenceMessenger.Default.Send(new MessagingCenterError{ Caller = "UpdateBreachesForAccountAsync", Exception = ex });
        }
        finally
        {
            IsBusy = false;
            IsBusyMessage = "";
            account.IsUpdating = false;
            account.LastUpdated = DateTime.Now;
        }
    }
}
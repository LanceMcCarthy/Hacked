using CommonHelpers.Common;
using CommonHelpers.Maui.Commands;
using CommonHelpers.Messaging;
using CommunityToolkit.Mvvm.Messaging;
using Hacked.Core.Common;
using Hacked.Core.Models;
using Hacked.Services.Interfaces;
using System.Diagnostics;
using System.Net;

namespace Hacked.Maui.ViewModels;

public class AccountDetailsViewModel : ViewModelBase
{
    private readonly IPwndBreachService _apiService;
    private readonly IAccountsService _accountsService;
    private MonitoredAccount _selectedAccount;

    public AccountDetailsViewModel(IPwndBreachService srv, IAccountsService accountsService)
    {
        _apiService = srv;
        _accountsService = accountsService;

        RefreshAccountCommand = new AsyncCommand<MonitoredAccount>(UpdateBreachesForAccountAsync);
        ClearNewBreachesCommand = new AsyncCommand<MonitoredAccount>(ClearNewBreachesAsync);
    }

    public MonitoredAccount SelectedAccount
    {
        get => _selectedAccount;
        set => SetProperty(ref _selectedAccount, value);
    }

    public AsyncCommand<MonitoredAccount> RefreshAccountCommand { get; set; }

    public AsyncCommand<MonitoredAccount> ClearNewBreachesCommand { get; set; }

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

            account.LastUpdated = DateTime.Now;

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
        }
    }

    private Task ClearNewBreachesAsync(MonitoredAccount account)
    {
        WeakReferenceMessenger.Default.Send(new MessagingCenterQuestion
        {
            Title = "Clear New Breaches?",
            Message = "Remove the flag from new breaches.",
            Okay = "yes",
            OnOkay = async () =>
            {
                foreach (var breach in account.Breaches)
                {
                    if (breach.IsNew)
                        breach.IsNew = false;
                }

                account.NewBreachCount = 0;
                account.HasNewBreaches = false;

                await _accountsService.SaveAccountsAsync();
            }
        });

        return Task.CompletedTask;
    }
}
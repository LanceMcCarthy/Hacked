using CommunityToolkit.Mvvm.Messaging;
using Hacked.Core.Common;
using Hacked.Core.Models;
using Hacked.Maui.Common.Commands;
using Hacked.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using Telerik.Maui.Controls.Compatibility.DataGrid;

namespace Hacked.Maui.ViewModels;

public class MonitoredAccountsViewModel : PageViewModelBase
{
    private readonly IPwndBreachService _apiService;
    private readonly IAccountsService _accountsService;
    private ObservableCollection<CategoricalChartData> _accountTotalsChartData;
    private MonitoredAccount _selectedAccount;
    private Breach _selectedBreach;
    private bool _areAccountsLoaded;
    private bool _hasAccounts;
    private int _newBreachesTotal;

    public MonitoredAccountsViewModel(IPwndBreachService srv, IAccountsService accountsService)
    {
        _apiService = srv;
        _accountsService = accountsService;

        RemoveAccountCommand = new AsyncCommand<MonitoredAccount>(RemoveAccountAsync);
        FindAllAccountBreachesCommand = new AsyncCommand(FindAllAccountsBreachesAsync);
        GoToSettingsCommand = new AsyncCommand(GoToSettingsAsync);
        ViewDetailsCommand = new AsyncCommand<MonitoredAccount>(GoToAccountDetailsAsync);
        RefreshAccountCommand = new AsyncCommand<MonitoredAccount>((a) => UpdateBreachesForAccountAsync(a, false));
        CellTapCommand = new AsyncCommand<object>(DataGridCellTappedAsync);
        FindSelectedAccountBreachesCommand = new AsyncCommand(
            () => UpdateBreachesForAccountAsync(SelectedAccount),
            () => SelectedAccount != null,
            ex =>
            {
                if (ex.Message.Contains("404") || ex.Message.Contains("net_http_message_not_success_statuscode"))
                {
                    SelectedAccount.Breaches = new();
                }
            });

        Accounts.CollectionChanged += (s, e) =>
        {
            HasAccounts = Accounts.Count > 0;
        };
    }

    #region Properties

    public ObservableCollection<MonitoredAccount> Accounts => _accountsService.CurrentAccounts;

    public ObservableCollection<CategoricalChartData> AccountTotalsChartData
    {
        get => _accountTotalsChartData ??= new();
        set => SetProperty(ref _accountTotalsChartData, value);
    }

    public MonitoredAccount SelectedAccount
    {
        get => _selectedAccount;
        set => SetProperty(ref _selectedAccount, value);
    }

    public Breach SelectedBreach
    {
        get => _selectedBreach;
        set => SetProperty(ref _selectedBreach, value);
    }

    public bool AreAccountsLoaded
    {
        get => _areAccountsLoaded;
        set => SetProperty(ref _areAccountsLoaded, value);
    }

    public bool HasAccounts
    {
        get => _hasAccounts;
        set => SetProperty(ref _hasAccounts, value);
    }

    public int NewBreachesTotal
    {
        get => _newBreachesTotal;
        set => SetProperty(ref _newBreachesTotal, value);
    }

    #endregion

    #region Commands

    public AsyncCommand FindSelectedAccountBreachesCommand { get; set; }

    public AsyncCommand<MonitoredAccount> RemoveAccountCommand { get; set; }

    public AsyncCommand FindAllAccountBreachesCommand { get; set; }

    public AsyncCommand GoToSettingsCommand { get; set; }

    public AsyncCommand<MonitoredAccount> ViewDetailsCommand { get; set; }

    public AsyncCommand<MonitoredAccount> RefreshAccountCommand { get; set; }

    public AsyncCommand<object> CellTapCommand { get; set; }

    #endregion

    #region Methods

    public async Task<MonitoredAccount> AddAccountAsync(string address)
    {
        MonitoredAccount account = null;

        try
        {
            if (string.IsNullOrEmpty(address))
                return null;

            IsBusy = true;
            IsBusyMessage = "Adding monitored account...";

            account = new MonitoredAccount
            {
                Address = address,
                Breaches = new(),
                IsUpdating = true
            };

            IsBusyMessage = $"Checking {account.Address} for breaches...";

            var result = await _apiService.CheckForBreachesAsync(account);

            foreach (var breach in result)
            {
                breach.IsNew = true;
                account.Breaches.Add(breach);
            }

            account.IsUpdating = false;
            account.LastUpdated = DateTime.Now;

            Accounts.Add(account);

            await _accountsService.SaveAccountsAsync();
        }
        catch (Exception ex)
        {
            WeakReferenceMessenger.Default.Send(new MessagingCenterError { Caller = "AddAccountAsync", Exception = ex });
        }
        finally
        {
            SelectedAccount = Accounts.LastOrDefault();

            HasAccounts = Accounts.Count > 0;

            UpdateStatistics();

            IsBusy = false;
            IsBusyMessage = "";
        }

        return account;
    }

    public Task RemoveAccountAsync(MonitoredAccount account)
    {
        if (account == null)
        {
            Debug.WriteLine("Account to remove is null", "RemoveAccount");

            WeakReferenceMessenger.Default.Send(new MessagingCenterError
            {
                Caller = "RemoveAccount",
                Exception = new NullReferenceException("This account is not in the list, so it does not need to be removed.")
            });

            return Task.CompletedTask;
        }

        WeakReferenceMessenger.Default.Send(new MessagingCenterQuestion
        {
            Message = "Are you sure you want to remove this monitored account?",
            Okay = "yes, remove it",
            OnOkay = (async () =>
            {
                try
                {
                    IsBusy = true;
                    IsBusyMessage = $"removing {account.Address}";

                    //check to see if I need to change the SelectedAccount after deleting
                    var wasSelectedAccount = SelectedAccount == account;

                    if (Accounts.Contains(account))
                        Accounts.Remove(account);

                    if (Accounts.Any())
                    {
                        if (wasSelectedAccount)
                            SelectedAccount = Accounts.LastOrDefault();
                    }
                    else
                    {
                        SelectedAccount = null;
                    }

                    await _accountsService.SaveAccountsAsync();

                    HasAccounts = Accounts.Count > 0;
                }
                catch (Exception ex)
                {
                    WeakReferenceMessenger.Default.Send(new MessagingCenterError{ Caller = "RemoveAccount", Exception = ex });
                }
                finally
                {
                    UpdateStatistics();

                    IsBusy = false;
                    IsBusyMessage = "";
                }
            })
        });

        return Task.CompletedTask;
    }

    public async Task FindAllAccountsBreachesAsync()
    {
        try
        {
            foreach (var monitoredAccount in Accounts)
            {
                await UpdateBreachesForAccountAsync(monitoredAccount, false);
            }

            await _accountsService.SaveAccountsAsync();
        }
        finally
        {
            UpdateStatistics();
        }
    }

    public async Task UpdateBreachesForAccountAsync(MonitoredAccount account, bool showSuccessMessage = true)
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
        }
        catch (PwnedApiException ex)
        {
            switch (ex.StatusCode)
            {
                case HttpStatusCode.NotFound when showSuccessMessage:
                    account.Breaches = new();

                    WeakReferenceMessenger.Default.Send(new MessagingCenterAlert
                    {
                        Title = "Good news",
                        Message = $"No breaches found for {account.Address}.",
                        Cancel = "OK"
                    });
                    break;
                case HttpStatusCode.NotFound:
                    account.Breaches = new();
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
            UpdateStatistics();

            IsBusy = false;
            IsBusyMessage = "";
            account.IsUpdating = false;
            account.LastUpdated = DateTime.Now;
        }
    }

    public void UpdateStatistics()
    {
        AccountTotalsChartData.Clear();

        var groupedAccounts = this.Accounts.GroupBy(a => a.Address).ToList();

        foreach (var account in groupedAccounts)
        {
            var category = account.Key;
            var count = account.Sum(a => a.Breaches.Count);

            AccountTotalsChartData.Add(new CategoricalChartData
            {
                Category = category,
                Value = count
            });

            Debug.WriteLine($"GroupByAccountTotals = Category: {category}, Count: {count}");
        }

        NewBreachesTotal = Accounts.Sum(a => a.NewBreachCount);
    }

    #endregion

    #region Navigation

    public override async void OnAppearing()
    {
        try
        {
            IsBusy = true;
            IsBusyMessage = "loading accounts from file...";

            await _accountsService.LoadAccountsAsync();

            AreAccountsLoaded = true;

            Debug.WriteLine($"--- {Accounts?.Count} accounts loaded from json file ---");

            HasAccounts = Accounts?.Count > 0;

            if (HasAccounts)
                SelectedAccount = Accounts?.FirstOrDefault();

            UpdateStatistics();
        }
        catch (Exception ex)
        {
            WeakReferenceMessenger.Default.Send(new MessagingCenterError{ Caller = "OnAppearing", Exception = ex });
        }
        finally
        {
            IsBusy = false;
            IsBusyMessage = "";

        }
    }

    private async Task GoToSettingsAsync()
    {
        await Shell.Current.GoToAsync("/Settings");
    }

    private async Task GoToAccountDetailsAsync(MonitoredAccount account)
    {
        SelectedAccount = account;

        await Shell.Current.GoToAsync("/AccountDetails", new Dictionary<string, object>
        {
            {"SelectedAccount", account}
        });
    }

    private async Task DataGridCellTappedAsync(object parameter)
    {
        if (parameter is DataGridCellInfo { Item: MonitoredAccount account } info)
        {
            if (info.Column.HeaderText == "Options")
                return;

            await GoToAccountDetailsAsync(account);
        }
    }

    #endregion
}
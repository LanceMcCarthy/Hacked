using CommonHelpers.Common;
using Hacked.Core.Models;
using Hacked.Maui.Common.Commands;
using Hacked.Maui.Common.Extensions;
using Hacked.Services.Interfaces;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using Hacked.Core.Common;
using Hacked.Maui.Services;
using Telerik.Maui.Controls.Compatibility.DataGrid;

namespace Hacked.Maui.ViewModels;

public class MonitoredAccountsViewModel : ViewModelBase
{
    #region fields

    private IPwndBreachService _apiService;
    private AccountsService _accountsService;

    private ObservableCollection<MonitoredAccount> _accounts;
    private ObservableCollection<CategoricalChartData> _accountTotalsChartData;
    private Breach _selectedBreach;
    private MonitoredAccount _selectedAccount;
    private bool _areAccountsLoaded;
    private bool _hasAccounts;
    private int _newBreachesTotal;

    #endregion
    
    public MonitoredAccountsViewModel(IPwndBreachService srv, AccountsService accountsService)
    {
        _apiService = srv;
        _accountsService = accountsService;

        InitCommands();

        InitData();
            
        Accounts.CollectionChanged += (s,e) => HasAccounts = Accounts.Count > 0;
    }

    #region Properties

    public ObservableCollection<MonitoredAccount> Accounts
    {
        get => _accounts ??= new ObservableCollection<MonitoredAccount>();
        set => SetProperty(ref _accounts, value);
    }

    public ObservableCollection<CategoricalChartData> AccountTotalsChartData
    {
        get => _accountTotalsChartData ??= new ObservableCollection<CategoricalChartData>();
        set => SetProperty(ref _accountTotalsChartData, value);
    }

    public Breach SelectedBreach
    {
        get => _selectedBreach;
        set => SetProperty(ref _selectedBreach, value);
    }

    public MonitoredAccount SelectedAccount
    {
        get => _selectedAccount;
        set => SetProperty(ref _selectedAccount, value);
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

    #region Account Management Methods
        
    public async Task<MonitoredAccount> AddAccountAsync(string address)
    {
        try
        {
            if (string.IsNullOrEmpty(address))
                return null;

            IsBusy = true;
            IsBusyMessage = "Adding monitored account...";
            
            var account = new MonitoredAccount
            {
                Address = address,
                Breaches = new ObservableCollection<Breach>(),
                IsUpdating = true
            };

            try
            {
                IsBusyMessage = $"Checking {account.Address} for breaches...";

                var result = await _apiService.CheckForBreachesAsync(account);

                foreach (var breach in result)
                {
                    breach.IsNew = true;
                    account.Breaches.Add(breach);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage("AddAccountAsync", ex);
            }
            finally
            {
                IsBusy = false;
                IsBusyMessage = "";

                account.IsUpdating = false;
                account.LastUpdated = DateTime.Now;
            }

 
            Accounts.Add(account);

            //SaveAccounts();
            await _accountsService.SaveAccountsAsync();
                
            SelectedAccount = Accounts.LastOrDefault();

            HasAccounts = Accounts.Count > 0;

            return account;
        }
        catch (Exception ex)
        {
            App.ShowExceptionMessage("AddAccountAsync", ex);
            return null;
        }
        finally
        {
            IsBusy = false;
            IsBusyMessage = "";
        }
    }

    public async Task RemoveAccountAsync(MonitoredAccount account)
    {
        if (account == null)
        {
            Debug.WriteLine("Account to remove is null", "RemoveAccount");
            App.ShowExceptionMessage("RemoveAccount", new NullReferenceException("Account to remove is null"));
        }

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

            //SaveAccounts();
            await _accountsService.SaveAccountsAsync();

            HasAccounts = Accounts.Count > 0;
        }
        catch (Exception ex)
        {
            App.ShowExceptionMessage("RemoveAccount", ex);
        }
        finally
        {
            IsBusy = false;
            IsBusyMessage = "";
        }
    }

    public async Task FindAllAccountsBreachesAsync()
    {
        foreach (var monitoredAccount in Accounts)
        {
            await UpdateBreachesForAccountAsync(monitoredAccount, false);
        }

        //SaveAccounts();
        await _accountsService.SaveAccountsAsync();
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
                    App.ShowMessage("Good news", $"No breaches found for {account.Address}.");
                    account.Breaches = new ObservableCollection<Breach>();
                    break;
                case HttpStatusCode.NotFound:
                    account.Breaches = new ObservableCollection<Breach>();
                    break;
                case HttpStatusCode.Forbidden:
                {
                    App.ShowExceptionMessage("UpdateBreachesForAccountAsync", ex);
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            App.ShowExceptionMessage("UpdateBreachesForAccountAsync", ex);
        }
        finally
        {
            IsBusy = false;
            IsBusyMessage = "";
            account.IsUpdating = false;
            account.LastUpdated = DateTime.Now;
        }
    }
    
    #endregion

    #region File Operation Methods

    public void SaveAccounts2()
    {
        IsBusy = true;
        IsBusyMessage = "saving accounts to file...";

        try
        {
            _accountsService.SaveAccountsAsync().RunSynchronously();

            Debug.WriteLine($"--- {_accounts.Count} Accounts Saved ---");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"*****Accounts json file not saved***** Error: {ex.Message}");
            App.ShowExceptionMessage("SaveAccountsAsync", ex);
        }
        finally
        {
            IsBusy = false;
            IsBusyMessage = "";
        }
    }

    public ObservableCollection<MonitoredAccount> LoadAccounts()
    {
        try
        {
            IsBusy = true;
            IsBusyMessage = "loading accounts from file...";

            var savedAccounts = _accountsService.LoadAccountsAsync().Result;

            if (savedAccounts == null)
            {
                Debug.WriteLine("Accounts json file not found");
                return new ObservableCollection<MonitoredAccount>();
            }

            AreAccountsLoaded = true;

            Debug.WriteLine($"--- {savedAccounts?.Count} accounts loaded from json file ---");

            HasAccounts = savedAccounts?.Count > 0;

            return savedAccounts;

        }
        catch (FileNotFoundException)
        {
            Debug.WriteLine("Accounts json file not found");
            return new ObservableCollection<MonitoredAccount>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"*****Accounts json file not loaded***** Error: {ex.Message}");
            App.ShowExceptionMessage("LoadAccountsAsync", ex);
            return new ObservableCollection<MonitoredAccount>();
        }
        finally
        {
            IsBusy = false;
            IsBusyMessage = "";
        }
    }
    
    #endregion

    #region Navigation Methods

    private async Task GoToSettingsAsync()
    {
        await Shell.Current.GoToAsync("/Settings");
    }
        
    private async Task ViewDetailsAsync(MonitoredAccount account)
    {
        SelectedAccount = account;

        await Shell.Current.GoToAsync("/AccountDetails");
    }

    private async Task DataGridCellTappedAsync(object parameter)
    {
        if (parameter is DataGridCellInfo {Item: MonitoredAccount account})
        {
            SelectedAccount = account;
            
            await Shell.Current.GoToAsync("/AccountDetails");
        }
    }
    
    #endregion

    #region Statistic Methods

    public void UpdateStatistics()
    {
        AccountTotalsChartData = GroupByAccountTotals();
        NewBreachesTotal = Accounts.Sum(a => a.NewBreachCount);
    }

    private ObservableCollection<CategoricalChartData> GroupByAccountTotals()
    {
        var list = new ObservableCollection<CategoricalChartData>();

        var groupedAccounts = this.Accounts.GroupBy(a => a.Address).ToList();

        foreach (var account in groupedAccounts)
        {
            var category = account.Key;
            var count = account.Sum(a => a.Breaches.Count);

            Debug.WriteLine($"GroupByAccountTotals = Category: {category}, Count: {count}");

            list.Add(new CategoricalChartData
            {
                Category = category,
                Value = count
            });
        }

        return list;
    }

    #endregion

    #region Initialization Methods

    private void InitCommands()
    {
        FindSelectedAccountBreachesCommand = new AsyncCommand(
            () => UpdateBreachesForAccountAsync(SelectedAccount), 
            () => SelectedAccount != null,
            ex =>
            {
                if (ex.Message.Contains("404") || ex.Message.Contains("net_http_message_not_success_statuscode"))
                {
                    SelectedAccount.Breaches = new ObservableCollection<Breach>();
                }
            });

        RemoveAccountCommand = new AsyncCommand<MonitoredAccount>(RemoveAccountAsync);

        FindAllAccountBreachesCommand = new AsyncCommand(FindAllAccountsBreachesAsync);

        GoToSettingsCommand = new AsyncCommand(GoToSettingsAsync);

        ViewDetailsCommand = new AsyncCommand<MonitoredAccount>(ViewDetailsAsync);

        RefreshAccountCommand= new AsyncCommand<MonitoredAccount>((a)=>UpdateBreachesForAccountAsync(a, false));

        CellTapCommand= new AsyncCommand<object>(DataGridCellTappedAsync);
    }

    private void InitData()
    {
        // Add loaded accounts instead of replacing entire collection
        Accounts.Clear();

        foreach (var loadedAccount in LoadAccounts())
            Accounts.Add(loadedAccount);

        // Once accounts are loaded, update stats
        UpdateStatistics();

        // Check for any
        HasAccounts = Accounts.Any();

        // Select first if there are any
        if (HasAccounts)
            SelectedAccount = Accounts.FirstOrDefault();
    }

    #endregion
}
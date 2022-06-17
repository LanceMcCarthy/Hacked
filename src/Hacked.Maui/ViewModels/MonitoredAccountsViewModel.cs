using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using Hacked.Core.Models;
using Hacked.Maui.Common.Extensions;
using Hacked.Services.Apis;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Hacked.Maui.Common.Commands;
using Telerik.Maui.Controls.Compatibility.DataControls.ListView.Commands;
using Telerik.Maui.Controls.Compatibility.DataGrid;

namespace Hacked.Maui.ViewModels;

public class MonitoredAccountsViewModel : ViewModelBase
{
    #region fields

    private BeenPwnedService _apiService;
    private ObservableCollection<MonitoredAccount> _accounts;
    private ObservableCollection<CategoricalChartData> _accountTotalsChartData;
    private Breach _selectedBreach;
    private MonitoredAccount _selectedAccount;
    private bool _areAccountsLoaded;
    private bool _hasAccounts;
    private int _newBreachesTotal;

    #endregion
    
    public MonitoredAccountsViewModel()
    {
        FindSelectedAccountBreachesCommand = new AsyncCommand(() => UpdateBreachesForAccountAsync(SelectedAccount), onException:HandleException);
        RemoveAccountCommand = new DelegateCommand<MonitoredAccount>(RemoveAccount);
        FindAllAccountBreachesCommand = new DelegateCommand(FindAllAccountsBreachesAsync);
        GoToSettingsCommand = new DelegateCommand(GoToSettingsAsync);
        ViewDetailsCommand = new DelegateCommand<ItemTapCommandContext>(ViewDetails);
        RefreshAccountCommand= new AsyncCommand<MonitoredAccount>(UpdateBreachesForAccountAsync);
        CellTapCommand= new DelegateCommand<object>(DataGridCellTapped);

        InitData();
            
        Accounts.CollectionChanged += Accounts_CollectionChanged;
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

    public AsyncCommand FindSelectedAccountBreachesCommand { get; }

    public DelegateCommand<MonitoredAccount> RemoveAccountCommand { get; }

    public DelegateCommand FindAllAccountBreachesCommand { get; }

    public DelegateCommand GoToSettingsCommand { get; }

    public DelegateCommand<ItemTapCommandContext> ViewDetailsCommand { get; }

    public AsyncCommand<MonitoredAccount> RefreshAccountCommand { get; }

    public DelegateCommand<object> CellTapCommand { get; }

    private void DataGridCellTapped(object parameter)
    {
        if (parameter is DataGridCellInfo {Item: MonitoredAccount account})
        {
            SelectedAccount = account;
            
            Shell.Current.GoToAsync("/accountdetails");
        }
    }

    #endregion

    #region Methods

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
        
    public void SaveAccounts()
    {
        IsBusy = true;
        IsBusyMessage = "saving accounts to file...";

        try
        {
            var json = JsonConvert.SerializeObject(_accounts);
            
            Hacked.Maui.Common.Extensions.FileExtensions.SaveTextToFile(json, "AccountsJsonData.txt");

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

            var json = FileExtensions.LoadTextFromFile("AccountsJsonData.txt");

            if (string.IsNullOrEmpty(json))
            {
                Debug.WriteLine("Accounts json file not found");
                return new ObservableCollection<MonitoredAccount>();
            }
                
            var savedAccounts = JsonConvert.DeserializeObject<ObservableCollection<MonitoredAccount>>(json);

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
        
    public async Task<MonitoredAccount> AddAccount(string address)
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
                _apiService ??= new BeenPwnedService();
                
                IsBusyMessage = $"Checking {account.Address} for breaches...";

                var result = await _apiService.CheckForBreachesAsync(account);

                foreach (var breach in result)
                {
                    breach.IsNew = true;
                    account.Breaches.Add(breach);
                }
            }
            catch (HttpRequestException ex)
            {
                // Important: a 404 is EXPECTED response from API if there are no results.
                if (ex.Message.Contains("404") || ex.Message.Contains("net_http_message_not_success_statuscode"))
                {
                    await Shell.Current.DisplayAlert("Good news!", "No known breaches found for this email address.", "close");
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

 
            Accounts.Add(account);

            SaveAccounts();
                
            SelectedAccount = Accounts.LastOrDefault();

            HasAccounts = Accounts.Count > 0;

            return account;
        }
        catch (Exception ex)
        {
            App.ShowExceptionMessage("AddAccount", ex);
            return null;
        }
        finally
        {
            IsBusy = false;
            IsBusyMessage = "";
        }
    }

    public void RemoveAccount(MonitoredAccount account)
    {
        if (account == null)
        {
            Debug.WriteLine("Account to remove is null", "RemoveAccount");
            return;
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

            SaveAccounts();

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
            _apiService ??= new BeenPwnedService();

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
        catch (HttpRequestException ex)
        {
            if (ex.Message.Contains("404") || ex.Message.Contains("net_http_message_not_success_statuscode"))
            {
                //if no results, update with empty list
                account.Breaches = new ObservableCollection<Breach>();
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
    

    public async void FindAllAccountsBreachesAsync()
    {
        foreach (var monitoredAccount in Accounts)
        {
            await UpdateBreachesForAccountAsync(monitoredAccount);
        }

        SaveAccounts();
    }
        
    private async void GoToSettingsAsync()
    {
        await Shell.Current.GoToAsync("/settings");
    }
        
    private async void ViewDetails(ItemTapCommandContext context)
    {
        SelectedAccount = context.Item as MonitoredAccount;

        await Shell.Current.GoToAsync("/accountdetails");
    }
    
    // Stats

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



    private void HandleException(Exception ex)
    {
        if (ex.Message.Contains("404") || ex.Message.Contains("net_http_message_not_success_statuscode"))
        {
//if no results, update with empty list
            account.Breaches = new ObservableCollection<Breach>();

            if (showSuccessMessage)
            { 
                Shell.Current.DisplayAlert("Good news!", "No known breaches found for this email address.", "close");
            }
        }
    }

    #endregion

    #region Event Handlers

    private void Accounts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        HasAccounts = Accounts.Count > 0;
    }

    #endregion
}
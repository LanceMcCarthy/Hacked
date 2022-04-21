using CommonHelpers.Common;
using Hacked.Core.Models;
using Hacked.Maui.Common.Commands;
using Hacked.Services.Apis;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Hacked.Maui.Common.Extensions;

namespace Hacked.Maui.ViewModels;

public class AccountsViewModel : ViewModelBase
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
    private AsyncCommand _findSelectedAccountBreachesCommand;
    private AsyncCommand<MonitoredAccount> _removeAccountCommand;
    private AsyncCommand _findAllAccountBreachesCommand;
    private AsyncCommand _goToSettingsCommand;
    private AsyncCommand _goToAddAccountCommand;
        
    #endregion

    public AccountsViewModel()
    {
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

    public AsyncCommand FindSelectedAccountBreachesCommand => _findSelectedAccountBreachesCommand ??= new AsyncCommand(() => UpdateBreachesForAccountAsync(SelectedAccount));

    public AsyncCommand<MonitoredAccount> RemoveAccountCommand => _removeAccountCommand ??= new AsyncCommand<MonitoredAccount>(RemoveAccount, account => account != null);

    public AsyncCommand FindAllAccountBreachesCommand => _findAllAccountBreachesCommand ??= new AsyncCommand(FindAllAccountsBreachesAsync);

    public AsyncCommand GoToSettingsCommand => _goToSettingsCommand ??= new AsyncCommand(GoToSettingsAsync);

    public AsyncCommand GoToAddAccountCommand => _goToAddAccountCommand ??= new AsyncCommand(GoToAddAccountAsync);

    #endregion

    #region Methods

    private void InitData()
    {
        // load accounts
        var loadedAccounts = LoadAccounts();

        // add loaded accounts instead of replacing entire collection
        foreach (var loadedAccount in loadedAccounts)
            Accounts.Add(loadedAccount);

        // once accounts are loaded, update stats
        UpdateStatistics();

        //check for any
        HasAccounts = Accounts.Any();

        //select first if there are any
        if (HasAccounts)
            SelectedAccount = Accounts[0];
            
    }
        
    public void SaveAccounts()
    {
        IsBusy = true;
        IsBusyMessage = "saving accounts to file...";

        try
        {
            var json = JsonConvert.SerializeObject(_accounts);

            Hacked.Maui.Common.Extensions.FileExtensions.SaveTextToFile(json, "AccountsJsonData.txt");

            Debug.WriteLine($"--- {_accounts.Count} Accounts Saved via Json ---");
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
            IsBusy = true;
            IsBusyMessage = "Adding monitored account...";

            if (string.IsNullOrEmpty(address))
                return null;

            var account = new MonitoredAccount {Address = address};

            await UpdateBreachesForAccountAsync(account, true);
                
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

    public Task<bool> RemoveAccount(MonitoredAccount account)
    {
        if (!RemoveAccountCommand.CanExecute(account))
            return Task.FromResult(false);

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

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            App.ShowExceptionMessage("RemoveAccount", ex);
            return Task.FromResult(false);
        }
        finally
        {
            IsBusy = false;
            IsBusyMessage = "";
        }
    }

    public async Task UpdateBreachesForAccountAsync(MonitoredAccount account, bool showSuccessMessage = false)
    {
        IsBusy = true;
        IsBusyMessage = $"Checking {account.Address} for breaches...";
        account.IsUpdating = true;

        try
        {
            if (_apiService == null)
                _apiService = new BeenPwnedService();

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

                if (showSuccessMessage)
                {
                    await Application.Current.MainPage.DisplayAlert("Good news!", "No known breaches found for this email address.", "close");
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

    public async Task FindAllAccountsBreachesAsync()
    {
        foreach (var monitoredAccount in Accounts)
        {
            await UpdateBreachesForAccountAsync(monitoredAccount, false);
        }

        SaveAccounts();
    }
        
    private async Task GoToSettingsAsync()
    {
        // TODO navigation
        //await ((Application.Current.MainPage as RootPage).Detail as NavigationPage).Navigation.PushAsync(new SettingsPage());
    }
        
    private async Task GoToAddAccountAsync()
    {
        // TODO navigation
        //await ((Application.Current.MainPage as RootPage).Detail as NavigationPage).Navigation.PushAsync(new AddAccountPage());
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
        
    #endregion

    #region event handlers
        
    private void Accounts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        //update the has accounts flag
        HasAccounts = Accounts.Count > 0;
    }

    #endregion
}
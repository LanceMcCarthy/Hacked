using CommonHelpers.Maui.Commands;
using CommonHelpers.Maui.Mvvm;
using CommonHelpers.Messaging;
using CommunityToolkit.Mvvm.Messaging;
using Hacked.Core.Common;
using Hacked.Core.Models;
using Hacked.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using Telerik.Maui.Controls.DataGrid;

namespace Hacked.Maui.ViewModels;

public class MonitoredAccountsViewModel : PageViewModelBase
{
    private readonly IPwndBreachService apiService;
    private readonly IAccountsService accountsService;
    private ObservableCollection<CategoricalChartData> accountTotalsChartData = new();
    private MonitoredAccount? selectedAccount;
    private Breach? selectedBreach;
    private bool areAccountsLoaded;
    private bool hasAccounts;
    private int newBreachesTotal;
    private bool isOverlayVisible;
    private bool isAddEnabled;

    public MonitoredAccountsViewModel(IPwndBreachService srv, IAccountsService accService)
    {
        apiService = srv;
        accountsService = accService;

        RemoveAccountCommand = new AsyncCommand<MonitoredAccount?>(RemoveAccountAsync);
        FindAllAccountBreachesCommand = new AsyncCommand(FindAllAccountsBreachesAsync);
        GoToSettingsCommand = new AsyncCommand(GoToSettingsAsync);
        ViewDetailsCommand = new AsyncCommand<MonitoredAccount?>(GoToAccountDetailsAsync);
        RefreshAccountCommand = new AsyncCommand<MonitoredAccount?>(a => UpdateBreachesForAccountAsync(a, showSuccessMessage: false, saveUpdate: true));
        CellTapCommand = new AsyncCommand<object?>(DataGridCellTappedAsync);
        FindSelectedAccountBreachesCommand = new AsyncCommand(
            () => UpdateBreachesForAccountAsync(SelectedAccount, showSuccessMessage: true, saveUpdate: true),
            () => SelectedAccount != null,
            ex =>
            {
                if (ex.Message.Contains("404") || ex.Message.Contains("net_http_message_not_success_statuscode"))
                {
                    if (SelectedAccount != null)
                        SelectedAccount.Breaches = new();
                }
            });
        ClearNewBreachesCommand = new AsyncCommand<MonitoredAccount?>(ClearNewBreachesAsync);
        AddPendingItemCommand = new Command(InvokeAddPendingItem);
        RemovePendingItemCommand = new Command<PendingAccount?>(InvokeRemovePendingItem);
        AddAccountsCommand = new AsyncCommand(InvokeAddAccounts);
        CancelAddAccountsCommand = new Command(InvokeCancelAddAccounts);
        ToggleOverlayCommand = new Command(InvokeToggleOverlay);

        PendingAdditions.CollectionChanged += PendingAdditions_CollectionChanged;
        foreach (var pendingAccount in PendingAdditions)
        {
            pendingAccount.PropertyChanged += PendingAccount_PropertyChanged;
        }

        UpdateIsAddEnabled();

        Accounts.CollectionChanged += (s, e) => { HasAccounts = Accounts.Count > 0; };
    }

    #region Properties

    public ObservableCollection<MonitoredAccount> Accounts => accountsService.CurrentAccounts;

    public ObservableCollection<CategoricalChartData> AccountTotalsChartData
    {
        get => accountTotalsChartData;
        set => SetProperty(ref accountTotalsChartData, value);
    }

    public ObservableCollection<PendingAccount> PendingAdditions { get; } = [new PendingAccount()];

    public MonitoredAccount? SelectedAccount
    {
        get => selectedAccount;
        set => SetProperty(ref selectedAccount, value);
    }

    public Breach? SelectedBreach
    {
        get => selectedBreach;
        set => SetProperty(ref selectedBreach, value);
    }

    public bool AreAccountsLoaded
    {
        get => areAccountsLoaded;
        set => SetProperty(ref areAccountsLoaded, value);
    }

    public bool HasAccounts
    {
        get => hasAccounts;
        set => SetProperty(ref hasAccounts, value);
    }

    public int NewBreachesTotal
    {
        get => newBreachesTotal;
        set => SetProperty(ref newBreachesTotal, value);
    }

    public bool IsOverlayVisible
    {
        get => isOverlayVisible;
        set => SetProperty(ref isOverlayVisible, value);
    }

    public bool IsAddEnabled
    {
        get => isAddEnabled;
        set => SetProperty(ref isAddEnabled, value);
    }

    #endregion

    #region Commands

    public AsyncCommand FindSelectedAccountBreachesCommand { get; set; }

    public AsyncCommand<MonitoredAccount?> RemoveAccountCommand { get; set; }

    public AsyncCommand FindAllAccountBreachesCommand { get; set; }

    public AsyncCommand GoToSettingsCommand { get; set; }

    public AsyncCommand<MonitoredAccount?> ViewDetailsCommand { get; set; }

    public AsyncCommand<MonitoredAccount?> RefreshAccountCommand { get; set; }

    public AsyncCommand<object?> CellTapCommand { get; set; }

    public AsyncCommand<MonitoredAccount?> ClearNewBreachesCommand { get; set; }

    public Command AddPendingItemCommand { get; set; }

    public Command RemovePendingItemCommand { get; set; }
    
    public Command ToggleOverlayCommand { get; set; }

    public AsyncCommand AddAccountsCommand { get; set; }

    public Command CancelAddAccountsCommand { get; set; }

    #endregion

    #region Methods

    public async Task<MonitoredAccount?> AddAccountAsync(string? address)
    {
        MonitoredAccount? account = null;

        try
        {
            if (string.IsNullOrWhiteSpace(address))
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

            var result = await apiService.CheckForBreachesAsync(account);

            foreach (var breach in result)
            {
                breach.IsNew = true;
                account.Breaches.Add(breach);
            }

            account.IsUpdating = false;
            account.LastUpdated = DateTime.Now;

            Accounts.Add(account);

            await accountsService.SaveAccountsAsync();
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

    public Task RemoveAccountAsync(MonitoredAccount? account)
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
            Cancel = "cancel",
            OnOkay = async () =>
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

                    await accountsService.SaveAccountsAsync();

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
            }
        });

        return Task.CompletedTask;
    }

    public async Task FindAllAccountsBreachesAsync()
    {
        try
        {
            foreach (var monitoredAccount in Accounts)
            {
                await UpdateBreachesForAccountAsync(monitoredAccount, showSuccessMessage: false, saveUpdate: false);
            }

            await accountsService.SaveAccountsAsync();
        }
        finally
        {
            UpdateStatistics();
        }
    }

    public async Task UpdateBreachesForAccountAsync(MonitoredAccount? account, bool showSuccessMessage = true, bool saveUpdate = true)
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
            var result = await apiService.CheckForBreachesAsync(account);

            //compare old list against new list to see if anything is new
            foreach (var breach in result)
            {
                if (!account.Breaches.Contains(breach))
                {
                    breach.IsNew = true;
                }
            }

            // Performance improvement, replace old list with new one instead of clear+add
            account.Breaches = result;

            account.IsUpdating = false;
            account.LastUpdated = DateTime.Now;

            if(saveUpdate)
                await accountsService.SaveAccountsAsync();
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
            account.IsUpdating = false;

            UpdateStatistics();

            IsBusy = false;
            IsBusyMessage = "";
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

    private void InvokeAddPendingItem()
    {
        PendingAdditions.Add(new PendingAccount());
    }

    private void InvokeRemovePendingItem(PendingAccount? item)
    {
        if (item == null)
            return;

        item.IsFocused = false;
        PendingAdditions.Remove(item);
    }

    private void PendingAdditions_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (PendingAccount item in e.OldItems)
            {
                item.PropertyChanged -= PendingAccount_PropertyChanged;
            }
        }

        if (e.NewItems != null)
        {
            foreach (PendingAccount item in e.NewItems)
            {
                item.PropertyChanged += PendingAccount_PropertyChanged;
            }
        }

        UpdateIsAddEnabled();
    }

    private void PendingAccount_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PendingAccount.Address))
        {
            UpdateIsAddEnabled();
        }
    }

    private void UpdateIsAddEnabled()
    {
        IsAddEnabled = PendingAdditions.Any(a => !string.IsNullOrWhiteSpace(a.Address));
    }

    private async Task InvokeAddAccounts()
    {
        IsBusy = true;

        foreach (var addition in PendingAdditions)
        {
            if (string.IsNullOrWhiteSpace(addition.Address))
            {
                continue;

            }

            IsBusyMessage = $"adding {addition.Address}...";

            var result = await AddAccountAsync(addition.Address);

            addition.AddSuccessful = result != null;
        }

        IsBusyMessage = string.Empty;
        IsBusy = false;

        var failedItems = PendingAdditions
            .Where(i => i.AddSuccessful == false && !string.IsNullOrWhiteSpace(i.Address))
            .Select(i => i.Address!)
            .ToList();

        if (failedItems.Count > 0)
        {
            await Shell.Current.DisplayAlertAsync("Done!", $"The operation completed, but some items were not added: {string.Join(", ", failedItems)}.", "ok");
        }

        IsOverlayVisible = false;

        PendingAdditions.Clear();
    }

    private void InvokeCancelAddAccounts()
    {
        IsOverlayVisible = false;

        // reset the collection
        PendingAdditions.Clear();
        PendingAdditions.Add(new PendingAccount());
    }

    private void InvokeToggleOverlay()
    {
        IsOverlayVisible = !IsOverlayVisible;
    }

    #endregion

    #region Navigation

    public override async void OnAppearing()
    {
        try
        {
            IsBusy = true;
            IsBusyMessage = "loading accounts from file...";

            await accountsService.LoadAccountsAsync();

            AreAccountsLoaded = true;

            Debug.WriteLine($"--- {Accounts.Count} accounts loaded from json file ---");

            HasAccounts = Accounts.Count > 0;

            if (HasAccounts)
                SelectedAccount = Accounts.FirstOrDefault();

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

    private static async Task GoToSettingsAsync()
    {
        await Shell.Current.GoToAsync("/Settings");
    }

    private async Task GoToAccountDetailsAsync(MonitoredAccount? account)
    {
        if (account == null)
            return;

        SelectedAccount = account;

        await Shell.Current.GoToAsync("///MonitoredAccounts/AccountDetails", new Dictionary<string, object>
        {
            {"SelectedAccount", account}
        });
    }

    private Task ClearNewBreachesAsync(MonitoredAccount? account)
    {
        if (account == null)
            return Task.CompletedTask;

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

                await accountsService.SaveAccountsAsync();
            }
        });

        return Task.CompletedTask;
    }

    private async Task DataGridCellTappedAsync(object? parameter)
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
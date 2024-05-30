using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using Hacked.Core.Common;
using Hacked.Core.Comparers;
using Hacked.Core.Models;
using Hacked.Helpers;
using Hacked.Services.Apis;
//using Microsoft.AppCenter.Analytics;
//using Microsoft.AppCenter.Crashes;
using Microsoft.Services.Store.Engagement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.Services.Store;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Popups;

namespace Hacked.ViewModels;

public class MainViewModel : ViewModelBase
{
    #region fields

    private bool isAppInitialized;

    private readonly ApplicationDataContainer roamingSettings;
    private readonly StorageFolder localFolder;

    private readonly BeenPwnedService apiService;
    private bool hasAccounts;
    private bool areAccountsLoaded;
    private bool areAdsRemoved;
    private string appVersion;

    private ObservableCollection<MonitoredAccount> accounts;
    private MonitoredAccount selectedAccount;
    private Breach selectedBreach;
    private DelegateCommand findAllAccountBreachesCommand;
    private DelegateCommand findSelectedAccountBreachesCommand;
    private DelegateCommand<MonitoredAccount> removeAccountCommand;
    private DelegateCommand showKudosCommand;
    private bool isIapSubscriber = true;
    private bool isKudoSelectorOpen;

    #endregion

    public MainViewModel()
    {
        if (DesignMode.DesignModeEnabled || DesignMode.DesignMode2Enabled)
        {
            Accounts = DesignTimeData.GenerateSampleAccounts();
            SelectedAccount = Accounts.FirstOrDefault();
            SelectedBreach = SelectedAccount?.Breaches?.FirstOrDefault();
            HasAccounts = true;
            AreAdsRemoved = true;
            AppVersion = "2.0.0";
            return;
        }

        roamingSettings = ApplicationData.Current.RoamingSettings;
        localFolder = ApplicationData.Current.LocalFolder;

        apiService = new BeenPwnedService();
    }

    #region Properties

    public ObservableCollection<MonitoredAccount> Accounts
    {
        get => accounts ??= new ObservableCollection<MonitoredAccount>();
        set
        {
            SetProperty(ref accounts, value);

            if (accounts != null)
            {
                HasAccounts = accounts.Any();
            }
        }
    }

    public Breach SelectedBreach
    {
        get => selectedBreach;
        set => SetProperty(ref selectedBreach, value);
    }

    public MonitoredAccount SelectedAccount
    {
        get => selectedAccount;
        set => SetProperty(ref selectedAccount, value);
    }

    public bool AreAccountsLoaded
    {
        get => areAccountsLoaded;
        set => SetProperty(ref areAccountsLoaded, value);
    }

    public string AppVersion
    {
        get
        {
            var nameHelper = Package.Current.Id;
            appVersion = nameHelper.Version.Major + "." + nameHelper.Version.Minor + "." + nameHelper.Version.Build;
            return appVersion;
        }
        private set => SetProperty(ref appVersion, value);
    }

    public bool HasAccounts
    {
        get => hasAccounts;
        set => SetProperty(ref hasAccounts, value);
    }

    public bool AreAdsRemoved
    {
        get
        {
            //return true;
            if (roamingSettings != null && roamingSettings.Values.TryGetValue(Constants.AreAdsRemovedSettingsKey, out object val))
            {
                areAdsRemoved = (bool)val;
            }

            return areAdsRemoved;
        }
        set
        {
            if (roamingSettings != null)
                roamingSettings.Values[Constants.AreAdsRemovedSettingsKey] = value;

            SetProperty(ref areAdsRemoved, value);
        }
    }

    public bool IsIapSubscriber
    {
        get => isIapSubscriber;
        set => SetProperty(ref isIapSubscriber, value);
    }

    public bool IsKudoSelectorOpen
    {
        get => isKudoSelectorOpen;
        set => SetProperty(ref isKudoSelectorOpen, value);
    }

    #endregion

    #region Commands

    public DelegateCommand FindSelectedAccountBreachCommand => findSelectedAccountBreachesCommand ?? (findSelectedAccountBreachesCommand = new DelegateCommand(async () =>
    {
        SelectedAccount.Breaches = await CheckForBreachesAsync(SelectedAccount);
        await SaveAccountsAsync();
    }));

    public DelegateCommand<MonitoredAccount> RemoveAccountCommand => removeAccountCommand ?? (removeAccountCommand = new DelegateCommand<MonitoredAccount>(async (account) =>
    {
        var md = new MessageDialog($"Do you really want to remove {account.Address}", "Delete?");

        md.Commands.Add(new UICommand("remove", async (args) =>
        {
            await RemoveAccount(account);
        }));

        md.Commands.Add(new UICommand("cancel"));

        await md.ShowAsync();
    }));

    public DelegateCommand FindAllAccountBreachesCommand => findAllAccountBreachesCommand ?? (findAllAccountBreachesCommand = new DelegateCommand(async () =>
    {
        await FindAllAccountsBreachesAsync();
    }));

    public DelegateCommand ShowKudosCommand => showKudosCommand ??= new DelegateCommand(() =>
    {
        IsKudoSelectorOpen = !IsKudoSelectorOpen;
    });

    #endregion

    #region methods

    public async Task InitializeApp()
    {
        if (isAppInitialized)
            return;

        Accounts = await LoadAccountsAsync();

        if (Accounts.Any())
        {
            SelectedAccount = Accounts.FirstOrDefault();
        }

        isAppInitialized = true;
    }

    public async Task<ObservableCollection<Breach>> CheckForBreachesAsync(MonitoredAccount account, bool showSuccessMessage = true)
    {
        IsBusy = true;
        IsBusyMessage = $"Checking {account.Address} for breaches...";
        account.IsUpdating = true;

        var result = new ObservableCollection<Breach>();

        try
        {
            result = await apiService.CheckForBreachesAsync(account);
        }
        catch (PwnedApiException ex)
        {
            switch (ex.StatusCode)
            {
                case HttpStatusCode.NotFound when showSuccessMessage:
                    await new MessageDialog($"The breaches database has returned 'no results' for this account.", "Good news!").ShowAsync();
                    break;
                case HttpStatusCode.Forbidden:
                    {
                        if (ApiInformation.IsTypePresent("Microsoft.Services.Store.Engagement.StoreServicesCustomEventLogger"))
                        {
                            StoreServicesCustomEventLogger.GetDefault().Log("HIBP API Forbidden");
                        }

                        // The result was a 403 or 404
                        DisplayMessageHelpers.ShowExceptionMessageOnUiThread("CheckForBreachesAsync", ex);
                        break;
                    }
            }
        }
        catch (Exception ex) // any other kind of exception, e.g. from HttpClient
        {
            // Retry/backoff logic causes this
            if (ex.Message.ToLower() == "the request message was already sent. cannot send the same request message multiple times.")
            {
                if (ApiInformation.IsTypePresent("Microsoft.Services.Store.Engagement.StoreServicesCustomEventLogger"))
                {
                    StoreServicesCustomEventLogger.GetDefault().Log("Rate Limited");
                }

                DisplayMessageHelpers.ShowExceptionMessageOnUiThread(
                    "Too Many Requests", 
                    new Exception("We're sorry, the API is limiting how many checks this app can do within a minute. Try again after 1 or 2 minutes."));
            }
            else
            {
                DisplayMessageHelpers.ShowExceptionMessageOnUiThread("CheckForBreachesAsync", ex);
            }
        }
        finally
        {
            IsBusy = false;
            IsBusyMessage = "";
            account.IsUpdating = false;
        }

        return result;
    }

    public async Task FindAllAccountsBreachesAsync()
    {
        foreach (var monitoredAccount in Accounts)
        {
            monitoredAccount.Breaches = await CheckForBreachesAsync(monitoredAccount, false);
            monitoredAccount.LastUpdated = DateTime.Now;
        }

        await SaveAccountsAsync();
    }

    // account management

    public async Task AddAccount(string address)
    {
        try
        {
            if (string.IsNullOrEmpty(address))
                return;

            IsBusy = true;

            var account = new MonitoredAccount { Address = address };

            account.Breaches = await CheckForBreachesAsync(account);

            account.LastUpdated = DateTime.Now;

            //NOTE we still want to add to monitored accounts even if no results at first
            Accounts.Add(account);

            // Just for analytic, see what users need the most help with
            var accountType = "";

            try
            {
                if (RegexHelpers.ValidateEmail(address))
                {
                    accountType = "email";
                }
                else if (RegexHelpers.ValidatePhoneNumber(address))
                {
                    accountType = "phonenumber";
                }
                else
                {
                    accountType = "username";
                }
            }
            catch
            {
                accountType = "unknown";
            }

            StoreServicesCustomEventLogger.GetDefault().Log($"Account Added, Account Type: {accountType}");

            //Analytics.TrackEvent("Account Added", new Dictionary<string, string>
            //    {
            //        { "Account Type", accountType }
            //    });

            await SaveAccountsAsync();

            SelectedAccount = Accounts.LastOrDefault();

            HasAccounts = Accounts.Count > 0;
        }
        catch (Exception ex)
        {
            DisplayMessageHelpers.ShowExceptionMessageOnUiThread("AddAccount", ex);
        }
    }

    public async Task RemoveAccount(MonitoredAccount account)
    {
        try
        {
            var wasSelectedAccount = SelectedAccount == account;

            if (Accounts.Contains(account))
            {
                Accounts.Remove(account);
                //Analytics.TrackEvent("Account Removed");
                StoreServicesCustomEventLogger.GetDefault().Log("Account Removed");
            }

            if (Accounts.Any())
            {
                if (wasSelectedAccount)
                    SelectedAccount = Accounts.LastOrDefault();
            }
            else
            {
                SelectedAccount = null;
            }

            await SaveAccountsAsync();

            HasAccounts = Accounts.Count > 0;
        }
        catch (Exception ex)
        {
            DisplayMessageHelpers.ShowExceptionMessageOnUiThread("RemoveAccount", ex);
        }
    }

    public async Task SaveAccountsAsync()
    {
        IsBusy = true;
        IsBusyMessage = "saving accounts to file...";

        try
        {
            var file = await localFolder.CreateFileAsync(Constants.LocalAccountsFileName, CreationCollisionOption.ReplaceExisting);

            var json = JsonConvert.SerializeObject(Accounts);

            using var fileStream = await file.OpenStreamForWriteAsync();
            using var streamWriter = new StreamWriter(fileStream);
            await streamWriter.WriteAsync(json);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"*****Accounts json file not saved***** Error: {ex.Message}");
            DisplayMessageHelpers.ShowExceptionMessageOnUiThread("SaveAccountsAsync", ex);
        }
        finally
        {
            IsBusy = false;
            IsBusyMessage = "";
        }
    }

    public async Task<ObservableCollection<MonitoredAccount>> LoadAccountsAsync()
    {
        try
        {
            IsBusy = true;
            IsBusyMessage = "loading accounts from file...";

            var file = await localFolder.TryGetItemAsync(Constants.LocalAccountsFileName);

            if (file == null)
            {
                Debug.WriteLine("Accounts json file not found");
                return new ObservableCollection<MonitoredAccount>();
            }

            if (file is StorageFile jsonFile)
            {
                using var fs = await jsonFile.OpenStreamForReadAsync();
                using var streamReader = new StreamReader(fs);
                var json = await streamReader.ReadToEndAsync();

                var savedAccounts = JsonConvert.DeserializeObject<ObservableCollection<MonitoredAccount>>(json);

                AreAccountsLoaded = true;

                Debug.WriteLine($"--- {savedAccounts?.Count} accounts loaded from json file ---");

                HasAccounts = savedAccounts?.Count > 0;

                return savedAccounts;
            }

            return new ObservableCollection<MonitoredAccount>();
        }
        catch (FileNotFoundException fnfex)
        {
            Debug.WriteLine("Accounts json file not found");

            //StoreServicesCustomEventLogger.GetDefault().Log($"Crash {Constants.LocalAccountsFileName} not found.");

            //Crashes.TrackError(fnfex, new Dictionary<string, string>
            //    {
            //        { "LoadAccountsAsync", $"{Constants.LocalAccountsFileName} not found." }
            //    });

            return new ObservableCollection<MonitoredAccount>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"*****Accounts json file not loaded***** Error: {ex.Message}");
            DisplayMessageHelpers.ShowExceptionMessageOnUiThread("LoadAccountsAsync", ex);
            return new ObservableCollection<MonitoredAccount>();
        }
        finally
        {
            IsBusy = false;
            IsBusyMessage = "";
        }
    }

    public async Task<Tuple<bool, string>> ExportAccountsAsync()
    {
        try
        {
            StoreServicesCustomEventLogger.GetDefault().Log($"Export Accounts");
            //Analytics.TrackEvent("Export Accounts");

            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = $"AccountsBackup_{DateTime.Now.ToFileTimeUtc()}"
            };

            savePicker.FileTypeChoices.Add("Hacked Accounts File", new List<string> { ".hked" });

            StorageFile file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                // In case user is selecting a OneDrive/DropBox/etc location
                CachedFileManager.DeferUpdates(file);

                var json = JsonConvert.SerializeObject(Accounts);

                // write to file
                await FileIO.WriteTextAsync(file, json);

                // Tell OneDrive/Dropbox/etc we're done
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

                if (status == FileUpdateStatus.Complete)
                {
                    return new Tuple<bool, string>(true, $"{Accounts.Count} account(s) were exported to {file.Name}.");
                }
                else
                {
                    return new Tuple<bool, string>(false, $"{file.Name} couldn't be saved.");
                }
            }
            else
            {
                return new Tuple<bool, string>(false, "Save cancelled");
            }
        }
        catch (Exception ex)
        {
            DisplayMessageHelpers.ShowExceptionMessageOnUiThread("CopyAccountsToRoamingStorageAsync", ex);
            return new Tuple<bool, string>(false, $"Error: {ex.Message}");
        }
    }

    public async Task<Tuple<bool, string>> ImportAccountsAsync(IReadOnlyList<IStorageItem> launchFiles = null)
    {
        try
        {
            IsBusy = true;
            IsBusyMessage = "Importing accounts...";

            StorageFile file = null;

            // If the app was launched with file, then we already have the backup file
            if (launchFiles != null &&
                launchFiles.Any() &&
                launchFiles[0] is StorageFile sf)
            {
                if (sf.FileType.Contains("hked"))
                {
                    file = (StorageFile)launchFiles[0];
                }
                else
                {
                    return new Tuple<bool, string>(false, $"Import cancelled. {sf.Name} is not a valid Hacked backup file (*.hked).");
                }
            }
            else
            {
                // Otherwise, show the file picker.

                var picker = new FileOpenPicker { ViewMode = PickerViewMode.List, SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
                picker.FileTypeFilter.Add(".hked");

                file = await picker.PickSingleFileAsync();
            }

            if (file != null)
            {
                ObservableCollection<MonitoredAccount> backupFileAccounts = null;

                try
                {
                    var json = await FileIO.ReadTextAsync(file);

                    backupFileAccounts = JsonConvert.DeserializeObject<ObservableCollection<MonitoredAccount>>(json);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Import Deserialization failed: {ex.Message}");
                }

                if (backupFileAccounts == null)
                {
                    return new Tuple<bool, string>(false, $"{file.Name} couldn't be loaded. Please make sure you are using a Hacked backup file (*.hked).");
                }

                // Find the exclusions between the list. These ones need to be added to the Accounts 
                var accountsToAdd = backupFileAccounts.Except(Accounts, new MonitoredAccountEqualityComparer()).ToList();

                var backupFileTotal = backupFileAccounts.Count;
                var newTotal = accountsToAdd.Count;
                var existingTotal = backupFileTotal - newTotal;

                Debug.WriteLine($"--- IMPORT: {backupFileTotal} accounts found in backup file, {newTotal} new accounts present, {existingTotal} skipped. ---");

                foreach (var acct in accountsToAdd)
                {
                    Accounts.Add(acct);
                }

                await SaveAccountsAsync();

                if (Accounts.Any())
                {
                    SelectedAccount = Accounts.FirstOrDefault();
                    HasAccounts = true;
                }
                else
                {
                    HasAccounts = false;
                }

                //Analytics.TrackEvent("Accounts Restored from backup");
                StoreServicesCustomEventLogger.GetDefault().Log("Accounts Restored from backup");

                return new Tuple<bool, string>(true, "Import Complete:\r\n\n" +
                                                     $"Accounts in file: {backupFileTotal}\n" +
                                                     $"Imported: {newTotal}\n" +
                                                     $"Skipped: {existingTotal} (already present).\r\n\n" +
                                                     "Would you like to refresh all monitored accounts now?");
            }
            else
            {
                return new Tuple<bool, string>(false, "Import cancelled");
            }
        }
        catch (Exception ex)
        {
            //Crashes.TrackError(ex);
            DisplayMessageHelpers.ShowExceptionMessageOnUiThread("Import Error", ex);
            return new Tuple<bool, string>(false, $"There was an error during import or file selection. Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            IsBusyMessage = "";
        }
    }

    #endregion

    #region IAP

    //private async Task PurchaseAdUnlockAsync()
    //{
    //    try
    //    {
    //        //await new KudosDialog().ShowAsync();
    //        IsBusy = true;
    //        IsBusyMessage = "removing ads...";

    //        //AreAdsRemoved = await StoreHelpers.PurchaseAsync(StoreIds.RemoveAdsStoreId);
    //    }
    //    catch (Exception ex)
    //    {
    //        DisplayMessageHelpers.ShowExceptionMessageOnUiThread("PurchaseAdUnlockAsync", ex);
    //    }
    //    finally
    //    {
    //        IsBusy = false;
    //        IsBusyMessage = "";
    //    }
    //}

    public async Task RefreshPurchasesAsync()
    {
        try
        {
            IsBusy = true;
            IsBusyMessage = "refreshing purchases...";

            if (ApiInformation.IsTypePresent("Windows.Services.Store.StoreContext"))
            {
                var context = StoreContext.GetDefault();

                if (context == null)
                    return;

                var appLicense = await context.GetAppLicenseAsync();

                if (appLicense == null)
                    return;

                foreach (KeyValuePair<string, StoreLicense> item in appLicense.AddOnLicenses)
                {
                    var addOnLicense = item.Value;

                    switch (addOnLicense.SkuStoreId)
                    {
                        case StoreIds.RemoveAdsStoreId:
                            AreAdsRemoved = addOnLicense.IsActive;
                            break;
                        case StoreIds.RecurringKudos1StoreId:
                            IsIapSubscriber = addOnLicense.IsActive;
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DisplayMessageHelpers.ShowExceptionMessageOnUiThread("RefreshPurchasesAsync", ex);
        }
        finally
        {
            IsBusy = false;
            IsBusyMessage = "";
        }
    }

    #endregion
}
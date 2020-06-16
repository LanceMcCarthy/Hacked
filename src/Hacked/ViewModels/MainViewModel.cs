using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Foundation.Metadata;
using Windows.Services.Store;
using Windows.Storage;
using Windows.UI.Popups;
using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using Hacked.Core.Common;
using Hacked.Core.Models;
using Hacked.Helpers;
using Hacked.Services.Apis;
using Microsoft.HockeyApp;
using Microsoft.Services.Store.Engagement;
using Newtonsoft.Json;

#if DEBUG
using CurrentApp = Windows.ApplicationModel.Store.CurrentAppSimulator;
#else
using CurrentApp = Windows.ApplicationModel.Store.CurrentApp;
#endif

namespace Hacked.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region fields

        private bool isAppInitialized;

        private readonly ApplicationDataContainer roamingSettings;
        private readonly StorageFolder localFolder;
        private readonly StorageFolder roamingFolder;

        private BeenPwnedService apiService;
        private bool hasAccounts;
        private bool areAccountsLoaded;
        private bool areAdsRemoved;
        private string appVersion;


        private ObservableCollection<MonitoredAccount> accounts;
        private MonitoredAccount selectedAccount;
        private Breach selectedBreach;
        private DelegateCommand findAllAccountBreachesCommand;
        private DelegateCommand findSelectedAccountBreacheCommand;
        private DelegateCommand<MonitoredAccount> removeAccountCommand;
        private DelegateCommand purchaseAdUnlockCommand;

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
                AppVersion = "1.0.1";
                return;
            }

            roamingSettings = ApplicationData.Current.RoamingSettings;
            localFolder = ApplicationData.Current.LocalFolder;
            roamingFolder = ApplicationData.Current.RoamingFolder;

            //TODO think of a better way to manage this automatically, for now, the user will need ot use backup/restore buttons
            //ApplicationData.Current.DataChanged += RoamingStorage_DataChanged;
        }

        #region Properties

        public ObservableCollection<MonitoredAccount> Accounts
        {
            get => accounts ?? (accounts = new ObservableCollection<MonitoredAccount>());
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

        #endregion

        #region Commands

        public DelegateCommand FindSelectedAccountBreachCommand => findSelectedAccountBreacheCommand ?? (findSelectedAccountBreacheCommand = new DelegateCommand(async () =>
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

        public DelegateCommand PurchaseAdUnlockCommand => purchaseAdUnlockCommand ?? (purchaseAdUnlockCommand = new DelegateCommand(async () =>
        {
            await PurchaseAdUnlockAsync();
        }));

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

        //network calls

        public async Task<ObservableCollection<Breach>> CheckForBreachesAsync(MonitoredAccount account, bool showSuccessMessage = true)
        {
            IsBusy = true;
            IsBusyMessage = $"Checking {account.Address} for breaches...";
            account.IsUpdating = true;

            var result = new ObservableCollection<Breach>();

            try
            {
                if (apiService == null)
                {
                    apiService = new BeenPwnedService();
                }

                result = await apiService.CheckForBreachesAsync(account);
            }
            catch (PwnedApiException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound && showSuccessMessage)
                {
                    await new MessageDialog($"No known breaches found for this email address.", "Good news!").ShowAsync();
                }
                else if (ex.StatusCode == HttpStatusCode.Forbidden)
                {
                    if (ApiInformation.IsTypePresent("Microsoft.Services.Store.Engagement.StoreServicesCustomEventLogger"))
                    {
                        StoreServicesCustomEventLogger.GetDefault().Log("HIBP API Forbidden");
                    }

                    // The result was a 403 or 404
                    DisplayMessageHelpers.ShowExceptionMessageOnUiThread("CheckForBreachesAsync", ex);
                }
            }
            catch (Exception ex)
            {
                // Any other kind of exception
                DisplayMessageHelpers.ShowExceptionMessageOnUiThread("CheckForBreachesAsync", ex);
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

                HockeyClient.Current.TrackEvent("Account Added");

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
                    HockeyClient.Current.TrackEvent("Account Removed");
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

                using (var fileStream = await file.OpenStreamForWriteAsync())
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    await streamWriter.WriteAsync(json);
                }
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
                    using (var fs = await jsonFile.OpenStreamForReadAsync())
                    using (var streamReader = new StreamReader(fs))
                    {
                        var json = await streamReader.ReadToEndAsync();

                        var savedAccounts = JsonConvert.DeserializeObject<ObservableCollection<MonitoredAccount>>(json);

                        AreAccountsLoaded = true;

                        Debug.WriteLine($"--- {savedAccounts?.Count} accounts loaded from json file ---");

                        HasAccounts = savedAccounts?.Count > 0;

                        return savedAccounts;
                    }
                }

                return new ObservableCollection<MonitoredAccount>();
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("Accounts json file not found");
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

        public async Task<bool> BackupAccountsToRoamingStorageAsync()
        {
            try
            {
                var localAccountsStorageItem = await localFolder.TryGetItemAsync(Constants.LocalAccountsFileName);

                if (!(localAccountsStorageItem is StorageFile localAccountsFile))
                    return false;

                await localAccountsFile.CopyAsync(roamingFolder, Constants.RoamingAccountsBackupFileName, NameCollisionOption.ReplaceExisting);

                HockeyClient.Current.TrackEvent("Accounts Backedup");

                return true;
            }
            catch (Exception ex)
            {
                DisplayMessageHelpers.ShowExceptionMessageOnUiThread("CopyAccountsToRoamingStorageAsync", ex);
                return false;
            }
        }

        public async Task<bool> LoadMissingAccountsFromRoamingStorageAsync()
        {
            try
            {
                IsBusy = true;
                IsBusyMessage = "syncing data from backup...";

                var file = await roamingFolder.TryGetItemAsync(Constants.RoamingAccountsBackupFileName);

                if (file == null)
                {
                    Debug.WriteLine("Roaming accounts json file not found");
                    await new MessageDialog("You do not currently have a backup file in your roaming storage. \r\n\nImportant Note: Changes to this file can sometimes take several minutes to become available to all your devices.", "Backup file not present").ShowAsync();
                    return false;
                }

                if (file is StorageFile jsonFile)
                {
                    using (var fs = await jsonFile.OpenStreamForReadAsync())
                    using (var streamReader = new StreamReader(fs))
                    {
                        var json = await streamReader.ReadToEndAsync();

                        var syncedAccounts = JsonConvert.DeserializeObject<ObservableCollection<MonitoredAccount>>(json);

                        Debug.WriteLine($"--- {syncedAccounts?.Count} synced accounts found ---");

                        if (syncedAccounts == null || syncedAccounts.Count == 0)
                        {
                            return false;
                        }

                        var addedAnAccount = false;

                        foreach (var syncedAccount in syncedAccounts)
                        {
                            if (Accounts.All(a => a.Address != syncedAccount.Address))
                            {
                                Accounts.Add(syncedAccount);
                                addedAnAccount = true;
                            }
                        }

                        if (addedAnAccount)
                        {
                            await SaveAccountsAsync();
                        }

                        HockeyClient.Current.TrackEvent("Accounts Restored from backup");

                        return true;
                    }
                }

                return false;
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("synced accounts file not found");
                DisplayMessageHelpers.ShowUserMessageOnUiThread("You do not currently have a backup file or it has not been synced yet.", "Backup file not found");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*****Accounts json file not loaded***** Error: {ex.Message}");
                DisplayMessageHelpers.ShowExceptionMessageOnUiThread("LoadMissingAccountsFromRoamingStorageAsync", ex);
                return false;
            }
            finally
            {
                IsBusy = false;
                IsBusyMessage = "";
            }
        }

        public async Task<bool> DeleteBackupFileAsync()
        {
            try
            {
                var file = await roamingFolder.TryGetItemAsync(Constants.RoamingAccountsBackupFileName);

                if (file == null)
                {
                    Debug.WriteLine("Accounts json file not found");
                    return false;
                }

                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);

                HockeyClient.Current.TrackEvent("Backup file deleted");

                return true;
            }
            catch (Exception ex)
            {
                DisplayMessageHelpers.ShowExceptionMessageOnUiThread("DeleteBackupFileAsync", ex);
                return false;
            }
        }

        #endregion

        #region IAP

        private async Task PurchaseAdUnlockAsync()
        {
            try
            {
                IsBusy = true;
                IsBusyMessage = "removing ads...";

                if (ApiInformation.IsTypePresent("Windows.Services.Store.StoreContext"))
                {
                    StoreContext context = StoreContext.GetDefault();

                    var result = await context.RequestPurchaseAsync(Secrets.RemoveAdsStoreId);

#if !DEBUG
                    HockeyClient.Current.TrackEvent(result.Status.ToString("G"));
#endif

                    switch (result.Status)
                    {
                        case StorePurchaseStatus.Succeeded:
                        case StorePurchaseStatus.AlreadyPurchased:
                            AreAdsRemoved = true;
                            foreach (var account in Accounts)
                            {
                                // Removes any existing ads from list
                                var adItems = account.Breaches.Where(b => b.Title == "VUNGLE" || b.Title == "AD");
                                foreach (var item in adItems)
                                {
                                    account.Breaches.Remove(item);
                                }
                            }
                            break;
                        case StorePurchaseStatus.NotPurchased:
                        case StorePurchaseStatus.NetworkError:
                        case StorePurchaseStatus.ServerError:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    var result = await CurrentApp.RequestProductPurchaseAsync(Secrets.RemoveAdsProductId);

#if !DEBUG
                    HockeyClient.Current.TrackEvent(result.Status.ToString("G"));
#endif

                    switch (result.Status)
                    {
                        case ProductPurchaseStatus.Succeeded:
                        case ProductPurchaseStatus.AlreadyPurchased:
                            AreAdsRemoved = true;
                            break;
                        case ProductPurchaseStatus.NotFulfilled:
                            // let user know
                            break;
                        case ProductPurchaseStatus.NotPurchased:
                            // let user know
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayMessageHelpers.ShowExceptionMessageOnUiThread("Windows Store problem", ex);
                HockeyClient.Current.TrackException(ex);
            }
            finally
            {
                IsBusy = false;
                IsBusyMessage = "";
            }
        }

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

                        if (addOnLicense.SkuStoreId == Secrets.RemoveAdsStoreId)
                        {
                            AreAdsRemoved = addOnLicense.IsActive;
                        }
                    }

                }
                else // prior to 1607
                {
                    AreAdsRemoved = CurrentApp.LicenseInformation.ProductLicenses[Secrets.RemoveAdsProductId].IsActive;
                }
            }
            catch (Exception ex)
            {
                HockeyClient.Current.TrackException(ex);
            }
            finally
            {
                IsBusy = false;
                IsBusyMessage = "";
            }
        }

        #endregion

        #region Automatic roaming management

        //private async void RoamingStorage_DataChanged(ApplicationData sender, object args)
        //{
        //    Debug.WriteLine($"ApplicationData.Current.DataChanged fired at: {DateTime.Now}");

        //    await DispatcherTaskExtensions.CallOnUiThreadAsync(async () =>
        //    {
        //        await LoadAccountsFromRoamingStorageAsync();
        //    });
        //}

        //private async void Accounts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    Debug.WriteLine($"Accounts CollectionChanged Fired");

        //    if (sender is ObservableCollection<MonitoredAccount> oc)
        //    {
        //        await DispatcherTaskExtensions.CallOnUiThreadAsync(() =>
        //        {
        //            Debug.WriteLine($"Accounts CollectionChanged - Current Count :{oc.Count}");
        //            HasAccounts = oc.Any();
        //        });
        //    }
        //}

        #endregion

    }
}
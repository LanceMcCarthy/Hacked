using CommonHelpers.Mvvm;
using Hacked.Core.Common;
using Hacked.Core.Comparers;
using Hacked.Core.Models;
using Hacked.Services.Apis;
using Hacked.Wpf.Helpers;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.Services.Store;
using Windows.Storage;
using Windows.Storage.Pickers;
using Analytics = Microsoft.AppCenter.Analytics.Analytics;
using DelegateCommand = CommonHelpers.Mvvm.DelegateCommand;
using ViewModelBase = CommonHelpers.Common.ViewModelBase;

namespace Hacked.Wpf.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region fields

        private bool isAppInitialized;

        private readonly ApplicationDataContainer localSettings;
        private readonly StorageFolder localFolder;

        private BeenPwnedService apiService;
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
        private DelegateCommand exportAccountsCommand;
        private DelegateCommand importAccountsCommand;
        private bool isIapSubscriber = true;
        private bool isKudoSelectorOpen;

        #endregion

        public MainWindowViewModel()
        {
            if (DesignMode.DesignModeEnabled || DesignMode.DesignMode2Enabled)
            {
                Accounts = DesignTimeData.GenerateSampleAccounts();
                SelectedAccount = Accounts.FirstOrDefault();
                SelectedBreach = SelectedAccount?.Breaches?.FirstOrDefault();
                HasAccounts = true;
                AreAdsRemoved = true;
                AppVersion = "1.0.2";
                return;
            }

            localSettings = ApplicationData.Current.LocalSettings;
            localFolder = ApplicationData.Current.LocalFolder;
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
            set => SetProperty(ref appVersion, value);
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
                if (localSettings != null && localSettings.Values.TryGetValue(Constants.AreAdsRemovedSettingsKey, out object val))
                {
                    areAdsRemoved = (bool)val;
                }

                return areAdsRemoved;
            }
            set
            {
                if (localSettings != null)
                    localSettings.Values[Constants.AreAdsRemovedSettingsKey] = value;

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

        public DelegateCommand FindSelectedAccountBreachCommand => findSelectedAccountBreachesCommand ??= new DelegateCommand(async () =>
        {
            SelectedAccount.Breaches = await CheckForBreachesAsync(SelectedAccount);
            await SaveAccountsAsync();
        });

        public DelegateCommand<MonitoredAccount> RemoveAccountCommand => removeAccountCommand ??= new DelegateCommand<MonitoredAccount>(async (account) =>
        {
            var result = MessageBox.Show($"Do you really want to remove {account.Address}", "Delete?", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                await RemoveAccount(account);
            }
        });

        public DelegateCommand FindAllAccountBreachesCommand => findAllAccountBreachesCommand ??= new DelegateCommand(async () =>
        {
            await FindAllAccountsBreachesAsync();
        });

        public DelegateCommand ExportAccountsCommand => exportAccountsCommand ??= new DelegateCommand(async () =>
        {
            await ExportAccountsAsync();
        });
        public DelegateCommand ImportAccountsCommand => importAccountsCommand ??= new DelegateCommand(async () =>
        {
            await ImportAccountsAsync();
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
                    MessageBox.Show($"No known breaches found for this email address.", "Good news!");
                }
                else if (ex.StatusCode == HttpStatusCode.Forbidden)
                {
                    //if (ApiInformation.IsTypePresent("Microsoft.Services.Store.Engagement.StoreServicesCustomEventLogger"))
                    //{
                    //    StoreServicesCustomEventLogger.GetDefault().Log("HIBP API Forbidden");
                    //}

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

                Analytics.TrackEvent("Account Added", new Dictionary<string, string>
                {
                    { "Account Type", RegexHelpers.ValidateEmail(address) ? "email" : "username" }
                });

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
                    Analytics.TrackEvent("Account Removed");
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
                var file = localFolder.CreateFileAsync(Constants.LocalAccountsFileName, CreationCollisionOption.ReplaceExisting).GetResults();

                var json = JsonConvert.SerializeObject(Accounts);

                await File.WriteAllTextAsync(file.Path, json);
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

                var file = localFolder.TryGetItemAsync(Constants.LocalAccountsFileName).GetResults();
                
                if (file is StorageFile jsonFile)
                {
                    var json = await File.ReadAllTextAsync(jsonFile.Path);

                    var savedAccounts = JsonConvert.DeserializeObject<ObservableCollection<MonitoredAccount>>(json);

                    AreAccountsLoaded = true;

                    Debug.WriteLine($"--- {savedAccounts?.Count} accounts loaded from json file ---");

                    HasAccounts = savedAccounts?.Count > 0;

                    return savedAccounts;
                }
                else
                { 
                    Debug.WriteLine("Accounts json file not found");
                    return new ObservableCollection<MonitoredAccount>();
                }
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine("Accounts json file not found");

                Crashes.TrackError(ex, new Dictionary<string, string>
                {
                    { "LoadAccountsAsync", $"{Constants.LocalAccountsFileName} not found." }
                });

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
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Export Accounts");

                var saveFileDialog = new RadSaveFileDialog();
                saveFileDialog.Owner = App.Current.MainWindow;
                saveFileDialog.FileName = $"AccountsBackup_{DateTime.Now.ToFileTimeUtc()}";
                saveFileDialog.InitialDirectory = "";
                saveFileDialog.ShowDialog();

                if (saveFileDialog.DialogResult == true)
                {
                    var json = JsonConvert.SerializeObject(Accounts);
                    
                    await File.WriteAllTextAsync(saveFileDialog.FileName, json);

                    return new Tuple<bool, string>(true, $"{Accounts.Count} account(s) were exported to {saveFileDialog.FileName}.");
                }
                else
                {
                    return new Tuple<bool, string>(false, "Save cancelled");
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
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

                    file = picker.PickSingleFileAsync().GetResults();
                }

                if (file != null)
                {
                    ObservableCollection<MonitoredAccount> backupFileAccounts = null;

                    try
                    {
                        var json = FileIO.ReadTextAsync(file).GetResults();

                        backupFileAccounts = JsonConvert.DeserializeObject<ObservableCollection<MonitoredAccount>>(json);
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
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

                    Analytics.TrackEvent("Accounts Restored from backup");

                    return new Tuple<bool, string>(true, $"Import Complete:\r\n\n" +
                                                         $"Accounts in file: {backupFileTotal}\n" +
                                                         $"Imported: {newTotal}\n" +
                                                         $"Skipped: {existingTotal} (already present)");
                }
                else
                {
                    return new Tuple<bool, string>(false, "Import cancelled");
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                Debug.WriteLine($"ImportAccountsAsync Error: {ex.Message}");
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

                    var appLicense = context.GetAppLicenseAsync().GetResults();

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
}

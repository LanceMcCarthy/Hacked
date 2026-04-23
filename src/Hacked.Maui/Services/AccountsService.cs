using CommonHelpers.Messaging;
using CommunityToolkit.Mvvm.Messaging;
using Hacked.Core.Common;
using Hacked.Core.Comparers;
using Hacked.Core.Models;
using Hacked.Services.Interfaces;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;

//using System.Net;

namespace Hacked.Maui.Services;

public class AccountsService(IPwndBreachService apiService) : IAccountsService
{
    private readonly string _accountsFilePath = Path.Join(FileSystem.Current.AppDataDirectory, "AccountsJsonData.json");
    private static readonly string[] HackedExportFileExtension = [".json", ".hked"];

    // private static readonly FilePickerFileType PickerTypes = new(new Dictionary<DevicePlatform, IEnumerable<string>>
    // {
    //     { DevicePlatform.iOS, HackedExportFileExtension },
    //     { DevicePlatform.Android, HackedExportFileExtension },
    //     { DevicePlatform.WinUI, HackedExportFileExtension },
    //     { DevicePlatform.Tizen, HackedExportFileExtension },
    //     { DevicePlatform.macOS, HackedExportFileExtension },
    //     { DevicePlatform.Unknown, HackedExportFileExtension }
    // });

    public ObservableCollection<MonitoredAccount> CurrentAccounts { get; set; } = new();

    public async Task SaveAccountsAsync()
    {
        try
        {
            var json = JsonConvert.SerializeObject(CurrentAccounts);

            await File.WriteAllTextAsync(_accountsFilePath, json);

            Debug.WriteLine($"--- {CurrentAccounts.Count} Accounts Saved ---");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"*****Accounts json file not saved***** Error: {ex.Message}");
            WeakReferenceMessenger.Default.Send(new MessagingCenterError
            {
                Caller = "SaveAccountsAsync",
                Exception = ex
            });
        }
    }

    public async Task LoadAccountsAsync()
    {
        try
        {
            if (!File.Exists(_accountsFilePath))
            {
                Debug.WriteLine("Accounts json file not found, creating a new one.");
                return;
            }

            var json = await File.ReadAllTextAsync(_accountsFilePath);

            var savedAccounts = JsonConvert.DeserializeObject<ObservableCollection<MonitoredAccount>>(json);

            if(CurrentAccounts.Any())
                CurrentAccounts.Clear();

            if (savedAccounts != null)
            {
                foreach (var account in savedAccounts)
                    CurrentAccounts.Add(account);

                Debug.WriteLine($"--- {savedAccounts.Count} accounts loaded from json file ---");
            }
        }
        catch (FileNotFoundException)
        {
            Debug.WriteLine("Accounts json file not found");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"*****Accounts json file not loaded***** Error: {ex.Message}");

            WeakReferenceMessenger.Default.Send(new MessagingCenterError{ Caller = "LoadAccountsAsync", Exception = ex });
        }
    }

    public async Task<Tuple<bool, string>> ImportBackupAsync(bool updateBreaches)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new()
            {
                PickerTitle = "Select backup (.hked or .json)",
                //FileTypes = PickerTypes
            });

            if (result == null)
                throw new FileLoadException();

            var json = await File.ReadAllTextAsync(result.FullPath);

            var backupFileAccounts = JsonConvert.DeserializeObject<ObservableCollection<MonitoredAccount>>(json);

            var accountsToAdd = backupFileAccounts!.Except(CurrentAccounts, new MonitoredAccountEqualityComparer()).ToList();

            var backupFileTotal = backupFileAccounts!.Count;
            var newTotal = accountsToAdd.Count;
            var existingTotal = backupFileTotal - newTotal;

            Debug.WriteLine($"--- IMPORT: {backupFileTotal} accounts found in backup file, {newTotal} new accounts present, {existingTotal} skipped. ---");

            foreach (var acct in accountsToAdd)
            {
                if (updateBreaches)
                {
                    try
                    {
                        var importUpdateResult = await apiService.CheckForBreachesAsync(acct);

                        //compare old list against new list to see if anything is new
                        foreach (var breach in importUpdateResult)
                        {
                            if (!acct.Breaches.Contains(breach))
                            {
                                breach.IsNew = true;
                            }
                        }

                        acct.Breaches = importUpdateResult;
                    }
                    catch (PwnedApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        acct.Breaches = new();
                    }

                    acct.IsUpdating = false;
                    acct.LastUpdated = DateTime.Now;
                }

                CurrentAccounts.Add(acct);
            }

            await SaveAccountsAsync();

            return new Tuple<bool, string>(true, "Import Complete:\r\n\n" +
                                                 $"Accounts in file: {backupFileTotal}\n" +
                                                 $"Imported: {newTotal}\n" +
                                                 $"Skipped: {existingTotal} (already present).\r\n\n" +
                                                 "Would you like to refresh all monitored accounts now?");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"***** Import Failed ***** Error: {ex.Message}");

            return new Tuple<bool, string>(false, $"There was an error during import or file selection. Error: {ex.Message}");
        }
    }

    public async Task<Tuple<bool, string>> ExportBackupAsync()
    {
        try
        {
            // Might have an issue with permissions on iOS/macOS... will have to use cache directory instead (FileSystem.CacheDirectory)
            // https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/file-picker?view=net-maui-8.0&tabs=android
            // https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/file-system-helpers?view=net-maui-8.0&tabs=macios

            var result = await FilePicker.Default.PickAsync(new()
            {
                PickerTitle = "Select directory",
                //FileTypes = PickerTypes
            });
            
            if(result == null)
                throw new FileLoadException();

            // if it turns out to be a relative path, try 'new FileInfo(result.FullPath).Directory.FullName'
            var directory = Path.GetDirectoryName(result.FullPath);
            var fileName = $"AccountsBackup_{DateTime.Now.ToFileTimeUtc()}{HackedExportFileExtension[0]}";
            var savePath = Path.Join(directory, fileName);

            var json = JsonConvert.SerializeObject(CurrentAccounts);

            await File.WriteAllTextAsync(savePath, json);

            return new Tuple<bool, string>(true, $"{CurrentAccounts.Count} account(s) were exported to {savePath}.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"***** Export Failed ***** Error: {ex.Message}");

            return new Tuple<bool, string>(false, $"Error: {ex.Message}");
        }
    }
}

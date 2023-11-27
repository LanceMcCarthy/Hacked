using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using Hacked.Core.Common;
using Hacked.Core.Comparers;
using Hacked.Core.Models;
using Hacked.Services.Interfaces;
using Newtonsoft.Json;

namespace Hacked.Maui.Services;

public class AccountsService : IAccountsService
{
    private static readonly string AccountsFilePath = Path.Join(FileSystem.Current.AppDataDirectory, "AccountsJsonData.hked");
    private static readonly string[] HackedFileExtension = { ".hked" };

    private static readonly FilePickerFileType PickerTypes = new(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.iOS, HackedFileExtension },
        { DevicePlatform.Android, HackedFileExtension },
        { DevicePlatform.WinUI, HackedFileExtension },
        { DevicePlatform.Tizen, HackedFileExtension },
        { DevicePlatform.macOS, HackedFileExtension },
        { DevicePlatform.Unknown, HackedFileExtension }
    });

    public AccountsService()
    {
        //_accountsFilePath = Path.Join(FileSystem.Current.AppDataDirectory, "AccountsJsonData.hked");
    }

    public ObservableCollection<MonitoredAccount> CurrentAccounts { get; set; } = new();

    public async Task SaveAccountsAsync()
    {
        try
        {
            var json = JsonConvert.SerializeObject(CurrentAccounts);
            
            await File.WriteAllTextAsync(json, AccountsFilePath);

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
            var json = await File.ReadAllTextAsync(AccountsFilePath);

            if (string.IsNullOrEmpty(json))
            {
                Debug.WriteLine("Accounts json file not found");
                return;
            }
                
            var savedAccounts = JsonConvert.DeserializeObject<ObservableCollection<MonitoredAccount>>(json);
            
            Debug.WriteLine($"--- {savedAccounts?.Count} accounts loaded from json file ---");
        }
        catch (FileNotFoundException)
        {
            Debug.WriteLine("Accounts json file not found");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"*****Accounts json file not loaded***** Error: {ex.Message}");

            WeakReferenceMessenger.Default.Send(new MessagingCenterError
            {
                Caller = "LoadAccountsAsync",
                Exception = ex
            });
        }
    }

    public async Task<Tuple<bool, string>> ImportBackupAsync()
    {
        try
        {
            ObservableCollection<MonitoredAccount> backupFileAccounts = null;

            var result = await FilePicker.Default.PickAsync(new()
            {
                PickerTitle = "Select backup file",
                FileTypes = PickerTypes
            });
        
            var json = await File.ReadAllTextAsync(result.FullPath);

            backupFileAccounts = JsonConvert.DeserializeObject<ObservableCollection<MonitoredAccount>>(json);

            var accountsToAdd = backupFileAccounts.Except(CurrentAccounts, new MonitoredAccountEqualityComparer()).ToList();

            var backupFileTotal = backupFileAccounts.Count;
            var newTotal = accountsToAdd.Count;
            var existingTotal = backupFileTotal - newTotal;

            Debug.WriteLine($"--- IMPORT: {backupFileTotal} accounts found in backup file, {newTotal} new accounts present, {existingTotal} skipped. ---");

            foreach (var acct in accountsToAdd)
            {
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
            // Might have an issue with permissions on iOS/MacOS... will have to use cache directory instead (FileSystem.CacheDirectory)
            // https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/file-picker?view=net-maui-8.0&tabs=android
            // https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/file-system-helpers?view=net-maui-8.0&tabs=macios

            var result = await FilePicker.Default.PickAsync(new()
            {
                PickerTitle = "Select directory",
                FileTypes = PickerTypes
            });

            // if it turns out to be a relative path, try 'new FileInfo(result.FullPath).Directory.FullName'
            var directory = Path.GetDirectoryName(result.FullPath);
            var fileName = $"AccountsBackup_{DateTime.Now.ToFileTimeUtc()}.hked";
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

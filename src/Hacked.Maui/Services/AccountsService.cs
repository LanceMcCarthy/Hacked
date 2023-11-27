using System.Collections.ObjectModel;
using System.Diagnostics;
using Hacked.Core.Comparers;
using Hacked.Core.Models;
using Newtonsoft.Json;

namespace Hacked.Maui.Services;

public class AccountsService
{
    public ObservableCollection<MonitoredAccount> CurrentAccounts { get; set; }

    public async Task SaveAccountsAsync()
    {
        try
        {
            var json = JsonConvert.SerializeObject(CurrentAccounts);
            
            await File.WriteAllTextAsync(json, "AccountsJsonData.txt");

            Debug.WriteLine($"--- {CurrentAccounts.Count} Accounts Saved ---");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"*****Accounts json file not saved***** Error: {ex.Message}");
            App.ShowExceptionMessage("SaveAccountsAsync", ex);
        }
    }

    public async Task<ObservableCollection<MonitoredAccount>> LoadAccountsAsync()
    {
        try
        {
            var json = await File.ReadAllTextAsync("AccountsJsonData.txt");

            if (string.IsNullOrEmpty(json))
            {
                Debug.WriteLine("Accounts json file not found");
                return new ObservableCollection<MonitoredAccount>();
            }
                
            var savedAccounts = JsonConvert.DeserializeObject<ObservableCollection<MonitoredAccount>>(json);
            
            Debug.WriteLine($"--- {savedAccounts?.Count} accounts loaded from json file ---");

            this.CurrentAccounts = savedAccounts;
            
            return savedAccounts;
        }
        catch (FileNotFoundException)
        {
            Debug.WriteLine("Accounts json file not found");
            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"*****Accounts json file not loaded***** Error: {ex.Message}");
            App.ShowExceptionMessage("LoadAccountsAsync", ex);
            return null;
        }
    }

    public async Task<Tuple<bool, string>> ImportBackupAsync()
    {
        var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.iOS, new[] { ".hked" } },
            { DevicePlatform.Android, new[] { ".hked" } },
            { DevicePlatform.WinUI, new[] { ".hked" } },
            { DevicePlatform.Tizen, new[] { ".hked" } },
            { DevicePlatform.macOS, new[] { ".hked" } },
            { DevicePlatform.Unknown, new[] { ".hked" } }
        });

        PickOptions options = new()
        {
            PickerTitle = "Select Backup file",
            FileTypes = customFileType,
        };

        try
        {
            ObservableCollection<MonitoredAccount> backupFileAccounts = null;

            var result = await FilePicker.Default.PickAsync(options);
        
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
            Console.WriteLine(ex);
            //throw;
            return new Tuple<bool, string>(false, $"There was an error during import or file selection. Error: {ex.Message}");
        }
    }

    public async Task<Tuple<bool, string>> ExportBackupAsync()
    {
        try
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { ".hked" } },
                { DevicePlatform.Android, new[] { ".hked" } },
                { DevicePlatform.WinUI, new[] { ".hked" } },
                { DevicePlatform.Tizen, new[] { ".hked" } },
                { DevicePlatform.macOS, new[] { ".hked" } },
                { DevicePlatform.Unknown, new[] { ".hked" } }
            });

            PickOptions options = new()
            {
                PickerTitle = "Select Backup Location",
                FileTypes = customFileType
            };

            // Might have an issue with permissions on iOS/MacOS... will have to use cache directory instead (FileSystem.CacheDirectory)
            // https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/file-picker?view=net-maui-8.0&tabs=android
            // https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/file-system-helpers?view=net-maui-8.0&tabs=macios

            var result = await FilePicker.Default.PickAsync(options);
        

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
            Console.WriteLine(ex);
            return new Tuple<bool, string>(false, $"Error: {ex.Message}");
            //throw;
        }
    }
}

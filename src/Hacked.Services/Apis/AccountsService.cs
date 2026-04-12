using Hacked.Core.Models;
using Hacked.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hacked.Services.Apis;

public class AccountsService : IAccountsService
{
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private static string DataFilePath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LancelotSoftware", "Hacked", "accounts.json");

    public ObservableCollection<MonitoredAccount> CurrentAccounts { get; set; } = new();

    public async Task LoadAccountsAsync()
    {
        try
        {
            var filePath = DataFilePath;
            if (!File.Exists(filePath)) return;

            var json = await Task.Run(() => File.ReadAllText(filePath));
            if (string.IsNullOrWhiteSpace(json)) return;

            var loaded = JsonSerializer.Deserialize<ObservableCollection<MonitoredAccount>>(json, s_jsonOptions);
            if (loaded == null) return;

            CurrentAccounts.Clear();
            foreach (var account in loaded)
                CurrentAccounts.Add(account);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AccountsService] LoadAccounts failed: {ex.Message}");
        }
    }

    public async Task SaveAccountsAsync()
    {
        try
        {
            var filePath = DataFilePath;
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            var json = JsonSerializer.Serialize(CurrentAccounts, s_jsonOptions);
            await Task.Run(() => File.WriteAllText(filePath, json));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AccountsService] SaveAccounts failed: {ex.Message}");
        }
    }

    public async Task<Tuple<bool, string>> ExportBackupAsync()
    {
        try
        {
            var exportPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                $"HackedBackup_{DateTime.Now:yyyyMMdd_HHmmss}.json");

            var json = JsonSerializer.Serialize(CurrentAccounts, s_jsonOptions);
            await Task.Run(() => File.WriteAllText(exportPath, json));
            return Tuple.Create(true, exportPath);
        }
        catch (Exception ex)
        {
            return Tuple.Create(false, ex.Message);
        }
    }

    public async Task<Tuple<bool, string>> ImportBackupAsync(bool updateBreaches)
    {
        // File picking is platform-specific; the path is passed in via the string parameter
        // when called from a ViewModel that has used a platform file picker.
        // This overload is not directly usable without a path — use ImportFromPathAsync instead.
        return await Task.FromResult(Tuple.Create(false, "Use ImportFromPathAsync with a file path."));
    }

    public async Task<Tuple<bool, string>> ImportFromPathAsync(string filePath, bool updateBreaches)
    {
        try
        {
            if (!File.Exists(filePath))
                return Tuple.Create(false, "File not found.");

            var json = await Task.Run(() => File.ReadAllText(filePath));
            var imported = JsonSerializer.Deserialize<ObservableCollection<MonitoredAccount>>(json, s_jsonOptions);

            if (imported == null || imported.Count == 0)
                return Tuple.Create(false, "No accounts found in the backup file.");

            if (updateBreaches)
            {
                CurrentAccounts.Clear();
                foreach (var account in imported)
                    CurrentAccounts.Add(account);
            }
            else
            {
                foreach (var importedAccount in imported)
                {
                    var existing = FindAccount(importedAccount.Address);
                    if (existing == null)
                        CurrentAccounts.Add(importedAccount);
                }
            }

            await SaveAccountsAsync();
            return Tuple.Create(true, $"Imported {imported.Count} account(s) successfully.");
        }
        catch (Exception ex)
        {
            return Tuple.Create(false, ex.Message);
        }
    }

    private MonitoredAccount FindAccount(string address) 
    {
        foreach (var account in CurrentAccounts)
            if (string.Equals(account.Address, address, StringComparison.OrdinalIgnoreCase))
                return account;
        return null;
    }
}

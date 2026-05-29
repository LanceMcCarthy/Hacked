using Hacked.Core.Common;
using Hacked.Core.Models;
using Hacked.Services.Apis;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;

namespace Hacked.BackgroundTasks;

public sealed class MonitoringTask : IBackgroundTask
{
    public async void Run(IBackgroundTaskInstance taskInstance)
    {
        var deferral = taskInstance.GetDeferral();

        LogMessage($"BG-MONITORING-TASK: started: {DateTime.Now}");
        
        var apiService = new BeenPwnedService();

        try
        {
            var savedAccounts = await LoadAccountsAsync();

            if (savedAccounts == null || savedAccounts.Count < 1)
            {
                LogMessage($"No accounts loaded, cancelling lookup");
                return;
            }

            LogMessage($"{savedAccounts?.Count} accounts loaded");

            foreach (var account in savedAccounts)
            {
                LogMessage($"Checking {account.Address} for breaches...");

                try
                {
                    var incomingBreachList = await apiService.CheckForBreachesAsync(account);

                    if (incomingBreachList != null && incomingBreachList.Count > 0)
                    {
                        var alertText = "";

                        //NOTE - remember, checking count value wont work because a count total can still be the same

                        foreach (var breach in incomingBreachList)
                        {
                            // use the overriden Equals method on the Breach class
                            if (account.Breaches.Any(b => b.Equals(breach)))
                                continue;

                            //if there is a breach that was not in the stored list, alert user
                            breach.IsNew = true;

                            alertText += $"{breach.Title}:";
                        }

                        account.HasNewBreaches = incomingBreachList.Any(b => b.IsNew);

                        if (account.HasNewBreaches)
                        {
                            account.Breaches = incomingBreachList;

                            var toastTag = account.Address.Substring(0, 5);

                            ShowNotification(account.Address, alertText, toastTag); //need a unique tag for this account's toast, so I use substring
                        }
                    }

                    // If any of the accounts have new breaches, save them now
                    if (savedAccounts.Any(a => a.HasNewBreaches))
                    {
                        await SaveAccountsAsync(savedAccounts);
                    }

                    account.LastUpdated = DateTime.Now;

                    await Task.Delay(5000);
                }
                catch (PwnedApiException ex)
                {
                    LogMessage($"BG-MONITORING-TASK: PwnedApiException: {ex.StatusCode}");
                }
            }

            LogMessage($"BG-MONITORING-TASK: Completed: {DateTime.Now}");
        }
        catch (Exception ex)
        {
            LogMessage($"BG-MONITORING-TASK: Run Exception: {ex.Message}");
        }
        finally
        {
            apiService?.Dispose();
            deferral.Complete();
        }
    }

    private static async Task<ObservableCollection<MonitoredAccount>> LoadAccountsAsync()
    {
        try
        {
            LogMessage($"Loading accounts from file");

            //first try json file
            var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync(Constants.LocalAccountsFileName);

            if (file is StorageFile jsonFile)
            {
                using (var fs = await jsonFile.OpenStreamForReadAsync())
                using (var streamReader = new StreamReader(fs))
                {
                    var json = await streamReader.ReadToEndAsync();

                    var savedAccounts = JsonConvert.DeserializeObject<ObservableCollection<MonitoredAccount>>(json);

                    Debug.WriteLine($"--- {savedAccounts?.Count} accounts loaded from json file ---");

                    return savedAccounts;
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            LogMessage($"LoadAccountsAsync Exception: {ex.Message}");
            return null;
        }
    }

    private static async Task SaveAccountsAsync(ObservableCollection<MonitoredAccount> accounts)
    {
        LogMessage($"BG-MONITORING-TASK: Saving accounts to file... ---");

        try
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(Constants.LocalAccountsFileName, CreationCollisionOption.ReplaceExisting);

            var json = JsonConvert.SerializeObject(accounts);

            using (var fileStream = await file.OpenStreamForWriteAsync())
            using (var streamWriter = new StreamWriter(fileStream))
            {
                await streamWriter.WriteAsync(json);
                LogMessage($"BG-MONITORING-TASK:  {accounts.Count} Accounts Saved via Json ---");
            }
        }
        catch (Exception ex)
        {
            LogMessage($"BG-MONITORING-TASK: *****Accounts json file not saved***** Error: {ex}");
        }
    }

    private static void ShowNotification(string accountEmail, string breachList, string tag)
    {
        try
        {
            // UWPToolkit
            var visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                        {
                            new AdaptiveText()
                            {
                                Text = $"ALERT: {accountEmail} has been p@wned"
                            },
                            new AdaptiveText()
                            {
                                Text = $"Your account has been located in the following new breaches: {breachList}. Tap here to view all breaches.",
                                HintAlign = AdaptiveTextAlign.Left,
                                HintMaxLines = 3,
                                HintWrap = true
                            }
                        }
                }
            };

            var toastContent = new ToastContent()
            {
                Visual = visual,
            };

            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(toastContent.GetXml()) { Tag = tag });
        }
        catch (Exception ex)
        {
            LogMessage($"ShowNotification Exception: {ex.Message}");
        }
    }

    private static void LogMessage(string message)
    {
        var localSettings = ApplicationData.Current.LocalSettings;

        if (localSettings != null)
        {
            localSettings.Values[Constants.MonitoringStatusTaskSettingsKey] = message;
        }

        Debug.WriteLine("BG-MONITORING-TASK: " + message);
    }
}

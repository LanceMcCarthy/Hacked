using Hacked.BackgroundTasks;
using Hacked.Core.Common;
using Hacked.Helpers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Hacked;

public sealed partial class MainPage
{
    private async void BackgroundMonitoringSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (selectionMute)
            return;

        if (BackgroundMonitoringSwitch.IsOn)
        {
            await ConfigureBackgroundTaskAsync();
        }
        else
        {
            await DisableBackgroundTaskAsync();
        }
    }

    private void ConfigureTaskElements(bool isTaskActive)
    {
        BackgroundMonitoringSwitch.IsOn = isTaskActive;

        if (isTaskActive && localSettings != null)
        {
            if (localSettings.Values.TryGetValue(Hacked.Core.Common.Constants.MonitoringStatusTaskSettingsKey, out var val))
            {
                LastTaskStatusTextBlock.Text = val.ToString();
            }

            UpdateStatus("Monitoring is active");
        }
        else
        {
            UpdateStatus("Monitoring is not active", false);
        }
    }

    private void UpdateStatus(string status, bool isActive = true)
    {
        CurrentStatusTextBlock.Text = status;

        MonitorFlyoutButton.Background = CurrentStatusBorder.Background = isActive
            ? new SolidColorBrush(Colors.LimeGreen)
            : new SolidColorBrush(Colors.Red);
    }

    private async Task<bool> ConfigureBackgroundTaskAsync()
    {
        try
        {
            ViewModel.IsBusy = true;
            ViewModel.IsBusyMessage = "configuring Background Task";

            var accessStatus = await BackgroundExecutionManager.RequestAccessAsync();

            switch (accessStatus)
            {
                case BackgroundAccessStatus.AlwaysAllowed:
                case BackgroundAccessStatus.AllowedSubjectToSystemPolicy:
                    await BackgroundTaskHelpers.RegisterTaskAsync(Constants.MonitorTaskName, typeof(MonitoringTask).FullName, (uint)updateFrequency);
                    UpdateStatus($"Monitoring Task is running every {updateFrequency} minutes");
                    return true;
                case BackgroundAccessStatus.DeniedBySystemPolicy:
                    UpdateStatus("Monitoring Task task was DENIED access", false);
                    await new MessageDialog("The app has denied from adding a background task due to System Policy. This is usually because there are too many background tasks already. " + "r\n\nGo to Phone Settings > Background Apps and free up a couple slots.").ShowAsync();
                    break;
                case BackgroundAccessStatus.DeniedByUser:
                    UpdateStatus("Monitoring Task was DENIED access", false);
                    await new MessageDialog("You have previously denied the app from adding a background task." + "r\n\nGo to Phone Settings > Background Apps \r\n\nFind this app in the list and re-enable background tasks.").ShowAsync();
                    break;
                case BackgroundAccessStatus.Unspecified:
                    UpdateStatus("Monitoring Task is currently NOT running", false);
                    await new MessageDialog(content: "You did not make a choice. If you want to update your Band in the background, please try again.").ShowAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Background Task Config Error: {ex}");

            await new MessageDialog($"Something went wrong configuring the background task. Error: {ex.Message}").ShowAsync();

            return false;
        }
        finally
        {
            ViewModel.IsBusy = false;
            ViewModel.IsBusyMessage = "";
        }
    }

    private async Task DisableBackgroundTaskAsync()
    {
        try
        {
            ViewModel.IsBusy = true;
            ViewModel.IsBusyMessage = "removing background task...";

            // Unregister the task and confirm to user it was successful
            if (await BackgroundTaskHelpers.UnregisterTaskAsync(Constants.MonitorTaskName))
            {
                UpdateStatus($"Monitoring Task is currently NOT running", false);
            }
        }
        catch (Exception ex)
        {
            await new MessageDialog($"There was a problem disabling Monitoring Task. Error: {ex.Message}").ShowAsync();
        }
        finally
        {
            ViewModel.IsBusyMessage = "";
            ViewModel.IsBusy = false;
        }
    }
}

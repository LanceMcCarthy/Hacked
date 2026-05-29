using Hacked.Core.Models;
using Hacked.Dialogs;
using Microsoft.Services.Store.Engagement;
using System;
using System.Linq;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hacked;

public sealed partial class MainPage
{
    private async void AddAccountButton_Click(object sender, RoutedEventArgs e)
    {
        AddAccountOverlay.Visibility = Visibility.Visible;

        if (RootSplitView.IsPaneOpen && WindowStates.CurrentState?.Name == "NarrowState")
        {
            RootSplitView.IsPaneOpen = false;
        }

        await AddAccountOverlay.FocusTextBoxAsync(FocusState.Pointer);
    }

    private void BreachesListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        foreach (var breach in e.AddedItems.Cast<Breach>())
        {
            if (breach.Id == "AD")
                return;

            breach.IsSelected = true;
        }

        foreach (var breach in e.RemovedItems.Cast<Breach>())
        {
            if (breach.Id == "AD")
                return;

            breach.IsSelected = false;
        }
    }

    private void AccountsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        foreach (MonitoredAccount account in e.AddedItems)
        {
            if (account.Breaches.Count > 0)
            {
                ViewModel.SelectedBreach = ViewModel.SelectedAccount.Breaches.FirstOrDefault();
            }
        }

        if (RootSplitView.DisplayMode == SplitViewDisplayMode.Overlay ||
            RootSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
        {
            RootSplitView.IsPaneOpen = false;
        }
    }

    private async void BreachInfoButton_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.DataContext is Breach breachSource)
        {
            await new BreachDetailsDialog(breachSource).ShowAsync();
        }
    }
    
    private void MonitorFlyout_OnOpened(object sender, object e)
    {
        if (localSettings == null)
            return;

        if (localSettings.Values.TryGetValue("MonitoringTaskStatus", out var val))
        {
            LastTaskStatusTextBlock.Text = val.ToString();
        }
    }

    private void CloseNoticeButton2_OnClick(object sender, RoutedEventArgs e)
    {
        localSettings.Values[$"{ViewModel.AppVersion}NoticeShown"] = true;
        NoticeOverlay.Visibility = Visibility.Collapsed;
    }

    private async void ContactUsButton_OnClick(object sender, RoutedEventArgs e)
    {
        await SendEmail();
    }

    private async void ReviewButton_OnClickButton_OnClick(object sender, RoutedEventArgs args)
    {
        var sendEmail = false;

        var md = new MessageDialog(
            "Are you happy with the app so far? Click review to rate the app.\r\n\nIf you have a complaint please consider clicking the contact us button instead, I promise I answer quickly and work hard to ensure you have a great experience.",
            "Review or Complaint?");

        md.Commands.Add(new UICommand("review", (e) =>
        {
            sendEmail = true;
        }));

        md.Commands.Add(new UICommand("contact us", (e) =>
        {
            sendEmail = false;
        }));

        if (sendEmail)
        {
            await SendEmail();
        }
        else
        {
            await Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
        }
    }

    private async void BackupAccountsButton_OnClick(object sender, RoutedEventArgs e)
    {
        var result = await ViewModel.ExportAccountsAsync();

        if (result.Item1)
        {
            await new MessageDialog(result.Item2, "Success").ShowAsync();
        }
        else if (!result.Item1)
        {
            await new MessageDialog(result.Item2, "Incomplete").ShowAsync();
        }

        BackupRestoreButtonFlyout?.Hide();
    }

    private async void RestoreAccountsButton_OnClick(object sender, RoutedEventArgs e)
    {
        var result = await ViewModel.ImportAccountsAsync();

        if (result.Item1)
        {
            var successDialog = new MessageDialog(result.Item2, "Success!");

            successDialog.Commands.Add(new UICommand("refresh monitored accounts"));

            successDialog.Commands.Add(new UICommand("no"));

            var r = await successDialog.ShowAsync();

            if (r.Label == "refresh monitored accounts")
            {
                await ViewModel.FindAllAccountsBreachesAsync();
            }
        }
        else if (!result.Item1)
        {
            await new MessageDialog(result.Item2, "Incomplete").ShowAsync();
        }

        BackupRestoreButtonFlyout?.Hide();
    }

    private async void ClearNewHyperlinkButton_OnClick(object sender, RoutedEventArgs e)
    {
        var saveNeeded = false;

        if (ViewModel.SelectedAccount == null) 
            return;

        foreach (var breach in ViewModel.SelectedAccount.Breaches)
        {
            if (!breach.IsNew) 
                continue;

            breach.IsNew = false;
            saveNeeded = true;
        }

        if (saveNeeded)
        {
            await ViewModel.SaveAccountsAsync();
        }
    }

    private async void FeedbackHubButton_OnClick(object sender, RoutedEventArgs e)
    {
        await StoreServicesFeedbackLauncher.GetDefault().LaunchAsync();
    }
}

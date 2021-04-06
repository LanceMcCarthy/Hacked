using Hacked.BackgroundTasks;
using Hacked.Core.Args;
using Hacked.Core.Common;
using Hacked.Core.Models;
using Hacked.Core.Primitives;
using Hacked.Dialogs;
using Hacked.Helpers;
using Hacked.ViewModels;
using Microsoft.Services.Store.Engagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using VungleSDK;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Hacked
{
    public sealed partial class MainPage : Page
    {
        private readonly ApplicationDataContainer localSettings;

        private int updateFrequency = 720;
        private bool selectionMute;
        private FilterType filterType = FilterType.Name;

        private readonly VungleAd vungleSdk;
        private const string VungleAppId = "5e347706c28ba7001748f549";
        private const string VungleMainInterstitialPlacementId = "MAININTERSTITIAL-8569070";
        private const string VungleKudoPlacementId = "KUDOSAD-0259168";
        private const string VungleApiEndpoint = "https://ads.api.vungle.com";

        public MainPage()
        {
            InitializeComponent();

            if (!DesignMode.DesignMode2Enabled || !DesignMode.DesignModeEnabled)
            {
                localSettings = ApplicationData.Current.LocalSettings;
            }

            //https://publisher.vungle.com/applications/application/5e347706c28ba7001748f549
            //https://support.vungle.com/hc/en-us/articles/360003059331-Get-Started-with-Vungle-Windows-SDK-v-6

            vungleSdk = AdFactory.GetInstance(VungleAppId, new VungleSDKConfig { ApiEndpoint = new Uri(VungleApiEndpoint) });
            vungleSdk.OnInitCompleted += VungleSdk_OnInitCompleted;
            vungleSdk.Diagnostic += VungleSdk_Diagnostic;
            vungleSdk.OnAdPlayableChanged += VungleSdkOnAdPlayableChanged;
        }

        #region event handlers

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
            foreach (Breach breach in e.AddedItems)
            {
                if (breach.Id == "AD")
                    return;

                breach.IsSelected = true;
            }

            foreach (Breach breach in e.RemovedItems)
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
                await new MessageDialog(result.Item2, "Success").ShowAsync();
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

            foreach (var breach in ViewModel.SelectedAccount.Breaches)
            {
                if (breach.IsNew)
                {
                    breach.IsNew = false;
                    saveNeeded = true;
                }
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

        #endregion

        #region Methods

        private void ConfigureTaskElements(bool isTaskActive)
        {
            BackgroundMonitoringSwitch.IsOn = isTaskActive;

            if (isTaskActive && localSettings != null)
            {
                if (localSettings.Values.TryGetValue(Constants.MonitoringStatusTaskSettingsKey, out var val))
                {
                    LastTaskStatusTextBlock.Text = val.ToString();
                }

                UpdateStatus($"Monitoring is active");
            }
            else
            {
                UpdateStatus($"Monitoring is not active", false);
            }
        }

        private void UpdateStatus(string status, bool isActive = true)
        {
            CurrentStatusTextBlock.Text = status;

            MonitorFlyoutButton.Background = CurrentStatusBorder.Background = isActive
                ? new SolidColorBrush(Colors.LimeGreen)
                : new SolidColorBrush(Colors.Red);
        }

        private void NotifyUserOfUpdatesOrChanges()
        {
            var noticeShown = false;

            if (localSettings.Values.TryGetValue($"{ViewModel.AppVersion}NoticeShown", out var hasShownNoticeSetting))
            {
                noticeShown = (bool)hasShownNoticeSetting;
            }

            if (!noticeShown)
            {
                NoticeTitle.Text = $"Updated! v.{ViewModel.AppVersion}";

                NoticeFeaturesTextBlock.Text = 
                    "- Add support for using a phone number\n" +
                    "- Upgraded Microsoft Toolkit Controls\n" +
                    "- Export and Import! Backup your accounts list to a small file\n" +
                    "- More modernized Fluent Design elements throughout the app; shadow, light and blur effects\n" +
                    "- Global access, no restricted regions due to API abuse.\n";
                NoticeFixesTextBlock.Text =
                    "- Removed spell checking for new account input\n" +
                    "- Faster UI loading times\n" +
                    "- Many more smaller improvements";

                NoticeOverlay.Visibility = Visibility.Visible;
            }
        }

        private async Task SendEmail()
        {
            try
            {
                ViewModel.IsBusy = true;
                ViewModel.IsBusyMessage = "opening email...";

                var email = new EmailMessage();
                email.To.Add(new EmailRecipient("awesome.apps@outlook.com", "Lancelot Software"));
                email.Subject = $"Hacked {ViewModel.AppVersion}";
                email.Body = "[write your message here]\r\n\n";

                await EmailManager.ShowComposeNewEmailAsync(email);
            }
            catch (Exception ex)
            {
                await new MessageDialog(
                    $"Something went wrong trying to open your email application automatically. You can still manually send an email to awesome.apps@outlook.com. /r/n/nError: {ex.Message}")
                    .ShowAsync();
            }
            finally
            {
                ViewModel.IsBusy = false;
                ViewModel.IsBusyMessage = "";
            }
        }

        #endregion

        #region Background Task management

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
                        UpdateStatus($"Monitoring Task task was DENIED access", false);
                        await new MessageDialog("The app has denied from adding a background task due to System Policy. This is usually because there are too many background tasks already. " + "r\n\nGo to Phone Settings > Background Apps and free up a couple slots.").ShowAsync();
                        break;
                    case BackgroundAccessStatus.DeniedByUser:
                        UpdateStatus($"Monitoring Task was DENIED access", false);
                        await new MessageDialog("You have previously denied the app from adding a background task." + "r\n\nGo to Phone Settings > Background Apps \r\n\nFind this app in the list and re-enable background tasks.").ShowAsync();
                        break;
                    case BackgroundAccessStatus.Unspecified:
                        UpdateStatus($"Monitoring Task is currently NOT running", false);
                        await new MessageDialog(content: "You did not make a choice. If you want to update your Band in the background, please try again.").ShowAsync();
                        break;
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

        #endregion

        #region SplitView management

        public event TypedEventHandler<MainPage, Rect> TogglePaneButtonRectChanged;

        public Rect TogglePaneButtonRect { get; private set; }

        private void TogglePaneButton_Checked(object sender, RoutedEventArgs e)
        {
            this.CheckTogglePaneButtonSizeChanged();
        }

        private void CheckTogglePaneButtonSizeChanged()
        {
            if (this.RootSplitView.DisplayMode == SplitViewDisplayMode.Inline ||
                this.RootSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                var transform = this.TogglePaneButton.TransformToVisual(this);
                var rect = transform.TransformBounds(new Rect(0, 0, this.TogglePaneButton.ActualWidth, this.TogglePaneButton.ActualHeight));
                this.TogglePaneButtonRect = rect;
            }
            else
            {
                this.TogglePaneButtonRect = new Rect();
            }

            var handler = this.TogglePaneButtonRectChanged;
            handler?.DynamicInvoke(this, this.TogglePaneButtonRect);
        }

        #endregion

        #region Filtering

        private async void FilterTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            await FilterBreachesListAsync();
        }

        private void ClearFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            FilterTextBox.Text = "";
        }

        private async void CommitFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            await FilterBreachesListAsync();
        }

        private async Task FilterBreachesListAsync()
        {
            await DispatcherTaskExtensions.CallOnUiThreadAsync(() =>
            {
                var breaches = ((MainViewModel)DataContext)?.SelectedAccount?.Breaches;

                BreachesListView.ItemsSource = string.IsNullOrEmpty(FilterTextBox.Text)
                    ? breaches
                    : breaches?.Where(Filter);
            });
        }

        private bool Filter(object arg)
        {
            switch (filterType)
            {
                case FilterType.Name:
                    var name = ((Breach)arg).Name.ToLowerInvariant();
                    return name.Contains(FilterTextBox?.Text.ToLowerInvariant() ?? string.Empty);
                case FilterType.DataStolen:
                    var classesList = ((Breach)arg).DataClasses;
                    return classesList.Any(dataClass => dataClass.Contains(FilterTextBox?.Text.ToLowerInvariant() ?? string.Empty));
            }

            return false;
        }

        private void ClearFilterToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            var toggleButton = sender as ToggleButton;

            if (toggleButton?.IsChecked == null)
                return;

            if (toggleButton.IsChecked == true)
            {
                toggleButton.Content = new SymbolIcon(Symbol.Clear);
            }
            else
            {
                toggleButton.Content = new SymbolIcon(Symbol.Filter);
                FilterTextBox.Text = "";

                BreachesListView.ItemsSource = ((MainViewModel)DataContext)?.SelectedAccount?.Breaches;
            }
        }

        #endregion

        #region navigation

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await ViewModel.InitializeApp();

            selectionMute = true;

            ConfigureTaskElements(await BackgroundTaskHelpers.CheckBackgroundTasksAsync(Constants.MonitorTaskName));

            selectionMute = false;

            if (ViewModel.Accounts.Count == 0)
            {
                RootSplitView.IsPaneOpen = true;
            }

            //#if ADD_BACK

            //#else
            //            FeedbackHubButton.Visibility = Visibility.Collapsed;
            //#endif
            FeedbackHubButton.Visibility = StoreServicesFeedbackLauncher.IsSupported()
                ? Visibility.Visible
                : Visibility.Collapsed;

            // FILE ACTIVATION
            if (e.Parameter is IReadOnlyList<IStorageItem> launchFiles)
            {
                var result = await ViewModel.ImportAccountsAsync(launchFiles);

                if (result.Item1)
                {
                    await new MessageDialog(result.Item2, "Success").ShowAsync();
                }
                else if (!result.Item1)
                {
                    await new MessageDialog(result.Item2, "Incomplete").ShowAsync();
                }
            }

            NotifyUserOfUpdatesOrChanges();
        }

        #endregion

        #region Vungle Ads - Lifecycle and Event Handlers  

        private async void PlayAdButton_OnClick(object sender, RoutedEventArgs e)
        {
            await VungleAd1.PlayAdAsync();
        }

        // A better option might be to make this Ad AutoCached
        private void VungleSdk_OnInitCompleted(object sender, ConfigEventArgs e)
        {
            try
            {
                vungleSdk.LoadAd(VungleMainInterstitialPlacementId);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        private async void VungleSdkOnAdPlayableChanged(object sender, AdPlayableEventArgs e)
        {
            Trace.WriteLine($"Ad Changed - Placement: {e.Placement}, IsPlayable: {e.AdPlayable}");

            if (VungleMainInterstitialPlacementId.Equals(e.Placement))
            {
                if (e.AdPlayable)
                {
                    await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        try
                        {
                            var playable = vungleSdk.IsAdPlayable(e.Placement);

                            PlayAdButton.IsEnabled = playable;

                            if (!playable)
                            {
                                // Possible "sleep" code, try to Load Ad Again
                                vungleSdk.LoadAd(e.Placement);
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.ToString());
                        }
                    });
                }
                else
                {
                    vungleSdk.LoadAd(VungleMainInterstitialPlacementId);

                    await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        PlayAdButton.IsEnabled = false;
                    });
                }
            }

            if (VungleKudoPlacementId.Equals(e.Placement))
            {
                if (e.AdPlayable)
                {
                    await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        try
                        {
                            var playable = vungleSdk.IsAdPlayable(e.Placement);

                            // TODO work on disabling replay-ability too soon
                            //if (KudosCtrl.Kudoses.FirstOrDefault() is Kudos adKudo)
                            //{
                            //    adKudo.IsBusy = playable;
                            //}

                            if (!playable)
                            {
                                vungleSdk.LoadAd(e.Placement);
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.ToString());
                        }
                    });
                }
                else
                {
                    vungleSdk.LoadAd(VungleKudoPlacementId);

                    // TODO work on disabling replay-ability too soon
                    //await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    //{
                    //    if (KudosCtrl.Kudoses.FirstOrDefault() is Kudos adKudo)
                    //    {
                    //        adKudo.IsBusy = true;
                    //    }
                    //});
                }
            }
        }

        private async void VungleAd1_Start(object sender, AdEventArgs e)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                VungleAd1.IsHitTestVisible = true;
            });
        }

        private async void VungleAd1_End(object sender, AdEndEventArgs e)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                VungleAd1.IsHitTestVisible = false;
            });
        }

        private void VungleSdk_Diagnostic(object sender, DiagnosticLogEvent e)
        {
            if (e.Message != null && (e.Message.ToLower().Contains("exception") || e.Message.ToLower().Contains("error")))
            {
                Trace.WriteLine($"VungleAd1: {e.Message}");
            }
        }

        private async void KudoAdRequested(object sender, AdRequestedArgs e)
        {
            await vungleSdk.PlayAdAsync(new AdConfig { Placement = e.PlacementId }, e.PlacementId);
        }

        #endregion
    }
}
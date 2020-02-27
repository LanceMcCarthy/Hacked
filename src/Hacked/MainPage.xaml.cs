using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Storage;
using Windows.System;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Hacked.BackgroundTasks;
using Hacked.Core.Common;
using Hacked.Core.Models;
using Hacked.Core.Primitives;
using Hacked.Dialogs;
using Hacked.Helpers;
using Hacked.ViewModels;
using Microsoft.Advertising.WinRT.UI;
using Microsoft.Services.Store.Engagement;
using VungleSDK;

namespace Hacked
{
    public sealed partial class MainPage : Page
    {
        private readonly ApplicationDataContainer localSettings;
        private readonly MainViewModel vm;

        private int updateFrequency = 720;
        private bool selectionMute;
        private FilterType filterType = FilterType.Name;
        private VungleAd vungleSdk;
        private const string VungleAppId = "5e347706c28ba7001748f549";
        private const string VungleMainFeedPlacementId = "DEFAULT-2363264";
        private const string VungleMainInterstitialPlacementId = "MAININTERSTITIAL-8569070";

        public MainPage()
        {
            InitializeComponent();

            vm = DataContext as MainViewModel;

            if (!DesignMode.DesignMode2Enabled || !DesignMode.DesignModeEnabled)
            {
                localSettings = ApplicationData.Current.LocalSettings;
            }

            //https://publisher.vungle.com/applications/application/5e347706c28ba7001748f549
            //https://support.vungle.com/hc/en-us/articles/360003059331-Get-Started-with-Vungle-Windows-SDK-v-6

            VungleSDKConfig sdkConfig = new VungleSDKConfig();
            //sdkConfig.DisableAshwidTracking = true; 
            //sdkConfig.MinimumDiskSpaceForAd = 50 * 1024 * 1024; 
            //sdkConfig.MinimumDiskSpaceForInit = 50 * 1024 * 1024;

            vungleSdk = AdFactory.GetInstance(VungleAppId, sdkConfig);
            vungleSdk.OnAdPlayableChanged += VungleSdkOnAdPlayableChanged;
        }

        #region event handlers

        private void AddAccountButton_Click(object sender, RoutedEventArgs e)
        {
            AddAccountOverlay.Visibility = Visibility.Visible;

            if(RootSplitView.IsPaneOpen && WindowStates.CurrentState?.Name == "NarrowState")
                RootSplitView.IsPaneOpen = false;
        }

        private void BreachesListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Breach breach in e.AddedItems)
            {
                if(breach.Id == "AD")
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

        private async void AccountsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (MonitoredAccount account in e.AddedItems)
            {
                if (account.Breaches.Count > 0)
                {
                    NoKnownBreachesGrid.Visibility = Visibility.Collapsed;
                    vm.SelectedBreach = vm.SelectedAccount.Breaches.FirstOrDefault();

                    //if(vm.AreAdsRemoved)

                    //if(vm.SelectedAccount.Breaches.All(b => b.Id != "AD"))
                    //    vm.SelectedAccount.Breaches?.Insert(0, new Breach { Id = "AD" });
                }
                else
                {
                    NoKnownBreachesGrid.Visibility = Visibility.Visible;
                }
            }

            if (RootSplitView.DisplayMode == SplitViewDisplayMode.Overlay || RootSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
                RootSplitView.IsPaneOpen = false;

            await vungleSdk.PlayAdAsync(new AdConfig(), VungleMainInterstitialPlacementId);
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
                await DisableTaskAsync();
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
            localSettings.Values[$"{vm.AppVersion}NoticeShown"] = true;
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
            if (await vm.BackupAccountsToRoamingStorageAsync())
            {
                await
                    new MessageDialog("Your account list has been backed up.\r\n\nPlease note that this change make take several minutes to appear across all your devices.",
                        "Success").ShowAsync();
                BackupRestoreButtonFlyout?.Hide();
            }
        }

        private async void RestoreAccountsButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!await vm.LoadMissingAccountsFromRoamingStorageAsync())
                return;

            if (vm.Accounts.Any())
            {
                vm.SelectedAccount = vm.Accounts.FirstOrDefault();
                vm.HasAccounts = true;
            }
            else
            {
                vm.HasAccounts = false;
            }

            BackupRestoreButtonFlyout?.Hide();
        }

        private async void DeleteBackupHyperlinkButton_OnClick(object sender, RoutedEventArgs e)
        {
            var md = new MessageDialog("Confirm Delete!");
            var confirmationCommand = new UICommand("delete");
            md.Commands.Add(confirmationCommand);
            md.Commands.Add(new UICommand("cancel"));

            var selectedCommand = await md.ShowAsync();

            if (selectedCommand != confirmationCommand) return;

            if (await vm.DeleteBackupFileAsync())
            {
                var message = "You have deleted the backup file.\r\n\nPlease note that this change make take several minutes to appear across all your devices.";
                await new MessageDialog(message, "Deleted").ShowAsync();
            }
        }

        private async void ClearNewHyperlinkButton_OnClick(object sender, RoutedEventArgs e)
        {
            var saveNeeded = false;

            foreach (var breach in vm.SelectedAccount.Breaches)
            {
                if (breach.IsNew)
                {
                    breach.IsNew = false;
                    saveNeeded = true;
                }
            }

            if (saveNeeded)
            {
                await vm.SaveAccountsAsync();
            }
        }

        private async void FeedbackHubButton_OnClick(object sender, RoutedEventArgs e)
        {
            await StoreServicesFeedbackLauncher.GetDefault().LaunchAsync();
        }

        private void HelpButton_OnClick(object sender, RoutedEventArgs e)
        {
            HelpOverlay.Visibility = Visibility.Visible;
            AboutButtonFlyout?.Hide();
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

        private void NotifyUser()
        {
            var noticeShown = false;

            if (localSettings.Values.TryGetValue($"{vm.AppVersion}NoticeShown", out var hasShownNoticeSetting))
            {
                noticeShown = (bool) hasShownNoticeSetting;
            }

            NoticeOverlay.Visibility = noticeShown ? Visibility.Collapsed : Visibility.Visible;

            // TODO -REMOVE after 1.6 update. The user needed a forced refresh after updating to v3 API
            if (!noticeShown)
            {
                vm.FindAllAccountBreachesCommand.Execute(null);
            }
        }

        private async Task SendEmail()
        {
            try
            {
                vm.IsBusy = true;
                vm.IsBusyMessage = "opening email...";

                var email = new EmailMessage();
                email.To.Add(new EmailRecipient("awesome.apps@outlook.com", "Lancelot Software"));
                email.Subject = $"Hacked {vm.AppVersion}";
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
                vm.IsBusy = false;
                vm.IsBusyMessage = "";
            }
        }

        #endregion

        #region Background Task management

        private async Task<bool> ConfigureBackgroundTaskAsync()
        {
            try
            {
                vm.IsBusy = true;
                vm.IsBusyMessage = "configuring Background Task";

                var accessStatus = await BackgroundExecutionManager.RequestAccessAsync();

                switch (accessStatus)
                {
                    case BackgroundAccessStatus.AlwaysAllowed:
                    //case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity:
                    //case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity:
                    case BackgroundAccessStatus.AllowedSubjectToSystemPolicy:
                        await BackgroundTaskHelpers.RegisterTaskAsync(Constants.MonitorTaskName, typeof(MonitoringTask).FullName, (uint) updateFrequency);
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
                    //case BackgroundAccessStatus.Denied:
                    //    UpdateStatus($"Monitoring Task was DENIED access", false);
                    //    await new MessageDialog("You've denied the app from updating your Band in the background or you have too many background tasks already. " +
                    //                          "r\n\nGo to Phone Settings > Background Apps \r\n\nFind this app in the list and re-enable background tasks.").ShowAsync();
                    //    break;
                    case BackgroundAccessStatus.Unspecified:
                        UpdateStatus($"Monitoring Task is currently NOT running", false);
                        await new MessageDialog(content: "You did not make a choice. If you want to update your Band in the background, please try again.").ShowAsync();
                        break;
                }

                return false;
            }
            catch (Exception ex)
            {
                await new MessageDialog($"Something went wrong configuring the background task. Error: {ex.Message}").ShowAsync();
                return false;
            }
            finally
            {
                vm.IsBusy = false;
                vm.IsBusyMessage = "";
            }
        }

        private async Task DisableTaskAsync()
        {
            try
            {
                vm.IsBusy = true;
                vm.IsBusyMessage = "removing background task...";

                //unregister the task and confirm to user it was successful
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
                vm.IsBusyMessage = "";
                vm.IsBusy = false;
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

        #region filtering

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
                var filterText = FilterTextBox.Text;

                var breaches = ((MainViewModel) DataContext)?.SelectedAccount?.Breaches;

                if (string.IsNullOrEmpty(filterText))
                {
                    BreachesListView.ItemsSource = breaches;
                }

                IEnumerable<Breach> filteredList = breaches?.Where(Filter);

                BreachesListView.ItemsSource = filteredList;
            });
        }

        private bool Filter(object arg)
        {
            switch (filterType)
            {
                case FilterType.Name:
                    var name = ((Breach) arg).Name.ToLowerInvariant();
                    return name.Contains(FilterTextBox?.Text.ToLowerInvariant());
                case FilterType.DataStolen:
                    var classesList = ((Breach) arg).DataClasses;
                    return classesList.Any(dataClass => dataClass.Contains(FilterTextBox?.Text.ToLowerInvariant()));
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
                BreachesListView.ItemsSource = ((MainViewModel) DataContext)?.SelectedAccount?.Breaches;
            }
        }

        #endregion

        #region navigation
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            InitializeAdControls();
            
            await vm.InitializeApp();

            selectionMute = true;

            ConfigureTaskElements(await BackgroundTaskHelpers.CheckBackgroundTasksAsync(Constants.MonitorTaskName));

            selectionMute = false;

            if (vm.Accounts.Count == 0)
                RootSplitView.IsPaneOpen = true;

            FeedbackHubButton.Visibility = StoreServicesFeedbackLauncher.IsSupported()
                ? Visibility.Visible
                : Visibility.Collapsed;

            NotifyUser();

            vungleSdk.LoadAd(VungleMainFeedPlacementId);
            vungleSdk.LoadAd(VungleMainInterstitialPlacementId);
            //await vungleSdk.PlayAdAsync(new AdConfig(), VungleMainFeedPlacementId);
            //await vungleSdk.PlayAdAsync(new AdConfig(), VungleMainInterstitialPlacementId);
        }

        #endregion

        #region Vungle Ads

        private async void VungleSdkOnAdPlayableChanged(object sender, AdPlayableEventArgs e)
        {
            if (e.AdPlayable)
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PlayableAdChanged(e.Placement));
            }
        }

        private void PlayableAdChanged(string placementId)
        {
            Debug.WriteLine($"Ad Changed : {placementId}");
        }

        #endregion

        #region Msft Ads


        private void InitializeAdControls()
        {
            if (vm.AreAdsRemoved)
            {
                HideAdGrid();
            }
            else
            {
                ShowAdGrid();
            }

            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                MainPageAdControl.Width = 320;
                MainPageAdControl.Height = 50;
                MainPageAdControl.AdUnitId = "331318";
                MainPageAdControl.ApplicationId = "dc8ca494-6b18-4502-96c7-61d2cb36967d";
            }
            else
            {
                MainPageAdControl.Width = 728;
                MainPageAdControl.Height = 90;
                MainPageAdControl.AdUnitId = "331317";
                MainPageAdControl.ApplicationId = "574159d4-36b9-4e6a-97f1-e28c72e03673";
            }
        }

        private void MainPageAdControl_OnAdRefreshed(object sender, RoutedEventArgs e)
        {
            if (vm.AreAdsRemoved)
            {
                HideAdGrid();
            }
            else
            {
                ShowAdGrid();
            }
        }

        private void MainPageAdControl_OnErrorOccurred(object sender, AdErrorEventArgs e)
        {
            HideAdGrid();
        }

        private void HideAdGrid()
        {
            if(AdGrid.Visibility == Visibility.Visible)
                AdGrid.Visibility = Visibility.Collapsed;
        }

        private void ShowAdGrid()
        {
            if (AdGrid.Visibility == Visibility.Collapsed)
                AdGrid.Visibility = Visibility.Visible;
        }

        #endregion
    }
}
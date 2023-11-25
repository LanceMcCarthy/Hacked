using Hacked.Core.Common;
using Hacked.Core.Primitives;
using Hacked.Helpers;
using Microsoft.Services.Store.Engagement;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Hacked;

public sealed partial class MainPage : Page
{
    private readonly ApplicationDataContainer localSettings;

    private readonly int updateFrequency = 720;
    private bool selectionMute;
    private FilterType filterType = FilterType.Name;

    public MainPage()
    {
        InitializeComponent();

        if (!DesignMode.DesignMode2Enabled || !DesignMode.DesignModeEnabled)
        {
            localSettings = ApplicationData.Current.LocalSettings;
        }
    }
    
    #region navigation and splitview management

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

            switch (result.Item1)
            {
                case true:
                    await new MessageDialog(result.Item2, "Success").ShowAsync();
                    break;
                case false:
                    await new MessageDialog(result.Item2, "Incomplete").ShowAsync();
                    break;
            }
        }

        NotifyUserOfUpdatesOrChanges();
    }

    public event TypedEventHandler<MainPage, Rect> TogglePaneButtonRectChanged;

    public Rect TogglePaneButtonRect { get; private set; }

    private void TogglePaneButton_Checked(object sender, RoutedEventArgs e)
    {
        CheckTogglePaneButtonSizeChanged();
    }

    private void CheckTogglePaneButtonSizeChanged()
    {
        if (RootSplitView.DisplayMode == SplitViewDisplayMode.Inline ||
            RootSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
        {
            var transform = TogglePaneButton.TransformToVisual(this);
            var rect = transform.TransformBounds(new Rect(0, 0, TogglePaneButton.ActualWidth, TogglePaneButton.ActualHeight));
            TogglePaneButtonRect = rect;
        }
        else
        {
            TogglePaneButtonRect = new Rect();
        }

        var handler = TogglePaneButtonRectChanged;
        handler?.DynamicInvoke(this, TogglePaneButtonRect);
    }

    #endregion

}
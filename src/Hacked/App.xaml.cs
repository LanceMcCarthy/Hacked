using Hacked.ViewModels;
using Microsoft.Toolkit.Uwp.UI;
using System;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
//using Windows.ApplicationModel;
//using Microsoft.AppCenter;
//using Microsoft.AppCenter.Analytics;
//using Microsoft.AppCenter.Crashes;

namespace Hacked;

sealed partial class App : Application
{
    public App()
    {
        InitializeComponent();
        //Suspending += OnSuspending;

        //AppCenter.Start(
        //    "512602fa-5e3c-4e7e-b2ac-7f27af7bf073",
        //    typeof(Analytics),
        //    typeof(Crashes));
    }

    // Normal launch
    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        var rootFrame = CreateRootFrame(e.PreviousExecutionState);

        if (rootFrame.Content == null)
        {
            rootFrame.Navigate(typeof(MainPage), e.Arguments);
        }

        Window.Current.Activate();
    }

    // Launched from file activation
    protected override async void OnFileActivated(FileActivatedEventArgs e)
    {
        Frame rootFrame = CreateRootFrame(e.PreviousExecutionState);

        if (rootFrame.Content == null)
        {
            rootFrame.Navigate(typeof(MainPage), e.Files);
        }
        else if (rootFrame.Content is MainPage mainPage && mainPage.DataContext is MainViewModel vm)
        {
            await vm.ImportAccountsAsync(e.Files);
        }

        Window.Current.Activate();
    }

    private Frame CreateRootFrame(ApplicationExecutionState previousExecutionState)
    {
        if (Window.Current.Content is not Frame rootFrame)
        {
            rootFrame = new Frame();

            rootFrame.NavigationFailed += OnNavigationFailed;

            // Not needed now that data is saved after every change
            //if (previousExecutionState == ApplicationExecutionState.Terminated)
            //{
            //    //TODO: Load state from previously suspended application
            //}

            Window.Current.Content = rootFrame;
        }

        //NOTE - UWP Toolkit
        ImageCache.Instance.CacheDuration = TimeSpan.MaxValue;

        CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
        var titleBar = ApplicationView.GetForCurrentView().TitleBar;
        titleBar.ButtonHoverForegroundColor = ((SolidColorBrush)Application.Current.Resources["AppBarBackgroundThemeBrush"]).Color;
        titleBar.ButtonForegroundColor = ((SolidColorBrush)Application.Current.Resources["AppBarItemForegroundThemeBrush"]).Color;
        titleBar.ButtonBackgroundColor = Colors.Transparent;

        return rootFrame;
    }

    //private void OnSuspending(object sender, SuspendingEventArgs e)
    //{
    //    var deferral = e.SuspendingOperation.GetDeferral();

    //    // No longer needed because accounts list is always saved.
    //    //await ((Window.Current.Content as MainPage).DataContext as MainViewModel)?.SaveAccountsAsync();

    //    deferral.Complete();
    //}

    private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }
}

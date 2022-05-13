using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hacked.Maui.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    public App()
    {
        this.InitializeComponent();
        this.RequestedTheme = ApplicationTheme.Light;
            
        // Do this work in MauiProgram.cs instead.
        //WindowHandler.ElementMapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        //{
        //    // Native WinUI app
        //    var nativeApp = handler.PlatformView as Hacked.Maui.WinUI.App;

        //    // Maui app
        //    var app = view as Hacked.Maui.App;
        //});
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        // Doesnt work either
        //var viewId = args.UWPLaunchActivatedEventArgs.CurrentlyShownApplicationViewId;

        //IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(viewId);
        //WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);

        //AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        //appWindow.MoveAndResize(new Windows.Graphics.RectInt32(0, 0, 500, 500));
    }
}


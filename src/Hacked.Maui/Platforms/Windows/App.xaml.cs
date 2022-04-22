using Microsoft.Maui.Handlers;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace Hacked.Maui.WinUI
{
    public partial class App : MauiWinUIApplication
    {
        public App()
        {
            this.InitializeComponent();
            this.RequestedTheme = ApplicationTheme.Light;
            
            WindowHandler.ElementMapper.AppendToMapping(nameof(IWindow), (handler, view) =>
            {
                // Native WinUI app
                var nativeApp = handler.PlatformView as Hacked.Maui.WinUI.App;

                // Maui app
                var app = view as Hacked.Maui.App;

                // Doesnt work in RC1
                //IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(handler.PlatformView);
                //WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);

                //AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                //appWindow.MoveAndResize(new Windows.Graphics.RectInt32(0, 0, 500, 500));
            });
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
}
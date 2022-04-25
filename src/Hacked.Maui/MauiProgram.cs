using Microsoft.Maui.LifecycleEvents;
using Telerik.Maui.Controls.Compatibility;

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

namespace Hacked.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseTelerik()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("telerikfontexamples.ttf", "telerikfontexamples");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if WINDOWS
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(wndLifeCycleBuilder =>
                {
                    wndLifeCycleBuilder.OnWindowCreated(window =>
                    {
                        IntPtr nativeWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        WindowId win32WindowsId = Win32Interop.GetWindowIdFromWindow(nativeWindowHandle);
                        AppWindow winuiAppWindow = AppWindow.GetFromWindowId(win32WindowsId);

                        // Hard coded logic to center window on a 1920x1080 display, adjust as needed
                        const int width = 1200;
                        const int height = 800;
                        const int x = 1920 / 2 - width / 2;
                        const int y = 1080 / 2 - height / 2;

                        winuiAppWindow.MoveAndResize(new RectInt32(x, y, width, height));
                    });
                });
            });
#endif

            return builder.Build();
        }
    }
}
using Microsoft.Maui.LifecycleEvents;
using Telerik.Maui.Controls.Compatibility;

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;

#elif MACCATALYST
using AppKit;
using CoreGraphics;
using Foundation;
using UIKit;

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
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("telerikfontexamples.ttf", "telerikfontexamples");
                    fonts.AddFont("fa-solid-900.ttf", "Font Awesome 6 Free Regular");
                });


            builder.ConfigureLifecycleEvents(events =>
            {
#if WINDOWS
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
#elif MACCATALYST
                
                events.AddiOS(wndLifeCycleBuilder =>
                {
                    wndLifeCycleBuilder.SceneWillConnect((scene, session, options) =>
                    {
                        if (scene is UIWindowScene { SizeRestrictions: { } } windowScene)
                        {
                            windowScene.SizeRestrictions.MaximumSize = new CGSize(1200, 900);
                            windowScene.SizeRestrictions.MinimumSize = new CGSize(600, 400);
                        }
                    });

                });
#endif
            });


            return builder.Build();

            // var uiWindow = windowScene.KeyWindow;

            //windowScene.SizeRestrictions.MaximumSize = new CGSize(1200, 900);
            //windowScene.SizeRestrictions.MinimumSize = new CGSize(600, 400);
        }
    }
}
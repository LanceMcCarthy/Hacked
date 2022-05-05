using Microsoft.Maui.LifecycleEvents;
using Telerik.Maui.Controls.Compatibility;

#if WINDOWS10_0_17763_0_OR_GREATER
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using Hacked.Maui.Platforms.Windows;
using Microsoft.UI.Xaml;
using WinRT;

#elif MACCATALYST
using AppKit;
using CoreGraphics;
using Foundation;
using UIKit;

#elif IOS
#elif ANDROID
#elif TIZEN
// nothing special here, yet
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
#if WINDOWS10_0_17763_0_OR_GREATER
                
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


                        // *** For Mica support ** //
                        Microsoft.UI.Composition.SystemBackdrops.MicaController micaController;
                        Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration configurationSource;

                        if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
                        {
                            var dispatcherQueueHelper = new WindowsSystemDispatcherQueueHelper(); // in Platforms.Windows folder
                            dispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();

                            // Hooking up the policy object
                            configurationSource = new Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration();
                            
                            // Initial configuration state.
                            configurationSource.IsInputActive = true;

                            switch (((FrameworkElement)window.Content).ActualTheme)
                            {
                                case ElementTheme.Dark: configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark; break;
                                case ElementTheme.Light: configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light; break;
                                case ElementTheme.Default: configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default; break;
                            }

                            micaController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();

                            // Enable the system backdrop.
                            // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                            micaController.AddSystemBackdropTarget(window.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                            micaController.SetSystemBackdropConfiguration(configurationSource);

                            window.Activated += (object sender, WindowActivatedEventArgs args) =>
                            {
                                configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
                            };

                            window.Closed += (object sender, WindowEventArgs args) =>
                            {
                                // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
                                // use this closed window.
                                if (micaController != null)
                                {
                                    micaController.Dispose();
                                    micaController = null;
                                }

                                configurationSource = null;
                            };
                        }
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
        }
    }
}
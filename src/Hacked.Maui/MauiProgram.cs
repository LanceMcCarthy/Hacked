using Microsoft.Maui.LifecycleEvents;
using Telerik.Maui.Controls.Compatibility;

#if WINDOWS10_0_17763_0_OR_GREATER
using Hacked.Maui.Platforms.Windows;
//using Windows.Graphics;
//using Microsoft.UI.Composition;
//using Microsoft.UI.Composition.SystemBackdrops;
//using Microsoft.UI.Windowing;
//using Microsoft.UI.Xaml;
//using WinRT;
using WinUIEx;

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
                        // OPTION 1 - Use PInvoke to get monitor's details and place it center
                        // window.PlacementCenterWindowInMonitorWin32(); // see Platforms/Windows/WindowsHelpers.cs


                        // Dimensions for options 2 and 3 (hard coded for a 1920x1080 display)
                        const int width = 1200;
                        const int height = 800;
                        const int x = 1920 / 2 - width / 2;
                        const int y = 1080 / 2 - height / 2;

                        //// OPTION 2 - You can use winUIEx extension method (add the WinUIEx NuGet package)
                        window.MoveAndResize(x, y, width, height);

                        //// Option 3 - Get the AppWindow reference and call the window methods directly
                        //AppWindow winuiAppWindow = window.GetAppWindowForWinUI(); // see Platforms/Windows/WindowsHelpers.cs
                        //winuiAppWindow.MoveAndResize(new RectInt32(x, y, width, height));


                        // *** For Mica or Acrylic support ** //
                        window.TryMicaOrAcrylic();
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
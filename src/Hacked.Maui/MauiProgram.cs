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
                        //const int width = 1920;
                        //const int height = 1080;
                        //const int x = 3440 / 2 - width / 2;
                        //const int y = 1440 / 2 - height / 2;
                        //window.MoveAndResize(x, y, width, height);

                        var manager = WinUIEx.WindowManager.Get(window);
                        manager.PersistenceId = "MainWindowPersistanceId";
                        manager.MinWidth = 640;
                        manager.MinHeight = 480;

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
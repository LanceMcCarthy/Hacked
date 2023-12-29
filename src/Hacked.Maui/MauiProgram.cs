using Hacked.Maui.Services;
using Hacked.Maui.ViewModels;
using Hacked.Maui.Views;
using Hacked.Services.Apis;
using Hacked.Services.Interfaces;
using Microsoft.Maui.LifecycleEvents;
using Telerik.Maui.Controls.Compatibility;

#if WINDOWS10_0_17763_0_OR_GREATER
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;
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

namespace Hacked.Maui;

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
                fonts.AddFont("Raleway-Regular.ttf", "Raleway");
                fonts.AddFont("telerikfontexamples.ttf", "telerikfontexamples");
                fonts.AddFont("fa-solid-900.ttf", "Font Awesome 6 Free Regular");
            })
            .RegisterLifecycleEvents();

        builder.Services.AddSingleton<IPwndBreachService, BeenPwnedService>();
        builder.Services.AddSingleton<IPwndPasswordService, PwnedPasswordService>();
        builder.Services.AddSingleton<IAccountsService, AccountsService>();

        builder.Services.AddSingleton<MonitoredAccountsViewModel>();
        builder.Services.AddSingleton<MonitoredAccountsPage>();

        builder.Services.AddTransient<AccountDetailsViewModel>();
        builder.Services.AddTransient<AccountDetailsPage>();

        builder.Services.AddTransient<BreachDetailsViewModel>();
        builder.Services.AddTransient<BreachDetailsPage>();

        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<SettingsPage>();

        builder.Services.AddSingleton<AboutViewModel>();
        builder.Services.AddSingleton<AboutPage>();

        return builder.Build();
    }

    public static MauiAppBuilder RegisterLifecycleEvents(this MauiAppBuilder builder)
    {
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

                    window.SystemBackdrop = new MicaBackdrop { Kind = MicaKind.BaseAlt };
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

        return builder;
    }
}